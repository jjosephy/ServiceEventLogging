
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceEventLogging.Listener
{
    internal class ServiceEventListener : EventListener
    {
        const string ApplicationEventSource = "ServiceEventLogging";
        const string LogFileName = "_ServiceLog_{0}.log";
        const string UnknownServiceName = "UnknownService";
        const string UnexpectedCloseException = "Unexpected Exception trying to close and re-initialize file";
        const string ThreadAbortException = "Thread Abort Exception";
        const string WriteException = "Exception trying to write to file";
        const string ServiceLogKey = "_servicelog_";

        const uint MaxFileSize = 10485760; // (10 MB) is max // 524288(.5MB); // 1048576(1MB); //5242880 (5MB); 

        /// <summary>
        /// This field is used so that if there are multiple loggers initialized in a process space
        /// the listener will only be initialized once
        /// </summary>
        static bool listenerIsInitialized = false;

        readonly ReaderWriterLockSlim lockContext = new ReaderWriterLockSlim();
        readonly uint allowedFileSize;
        readonly string filePath;
        readonly string serviceLogFileName;
        volatile FileStream file;
        int currentFileId = 0;
        string currentFileName = string.Empty;

        /// <summary>
        /// Default Ctor
        /// </summary>
        public ServiceEventListener()
        {
            if (!listenerIsInitialized)
            {
                listenerIsInitialized = true;

                filePath = ConfigurationManager.AppSettings["LogFilePath"];
                if (uint.TryParse(ConfigurationManager.AppSettings["MaxFileSize"], out allowedFileSize))
                {
                    // Enforce a max of 10 MBs
                    if (allowedFileSize > MaxFileSize)
                    {
                        allowedFileSize = MaxFileSize;
                    }
                }
                else
                {
                    allowedFileSize = MaxFileSize;
                }

                this.serviceLogFileName =
                    string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ServiceLogName"]) ?
                    string.Concat(UnknownServiceName, LogFileName) :
                    string.Concat(ConfigurationManager.AppSettings["ServiceLogName"], LogFileName);

                FindLastLogFile();
                InitializeFile();
            }
        }

        /// <summary>
        /// This is called anytime an Event is written
        /// </summary>
        /// <param name="eventData"></param>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // File will be null if for some reason initialize fails.
            if (file != null)
            {
                Task.Run(() =>
                {
                    // Payload shouldnt be null but just to be safe
                    var payload = eventData.Payload.First();
                    if (payload != null)
                    {
                        WriteToFileAsync(payload.ToString());
                    }
                });
            }
        }

        /// <summary>
        /// Writes a log line to the current opened file
        /// </summary>
        /// <param name="logLine">The log line to write to file</param>
        private void WriteToFileAsync(string logLine)
        {
            try
            {
                lockContext.EnterWriteLock();
                var buffer = Encoding.UTF8.GetBytes(logLine + Environment.NewLine);
                this.file.Write(buffer, 0, buffer.Length);
                this.file.Flush();

                if (file.Length > allowedFileSize)
                {
                    try
                    {
                        this.file.Close();
                    }
                    catch
                    {
                        // Not sure if this will ever happen but it would be good to know if it does.
                        WriteToEventLog(
                            UnexpectedCloseException + logLine,
                            new Exception(UnexpectedCloseException));
                    }
                    finally
                    {
                        currentFileId = Interlocked.Increment(ref currentFileId);
                        InitializeFile();
                    }
                }
            }
            catch (ThreadAbortException threadEx)
            {
                WriteToEventLog(ThreadAbortException + logLine, threadEx);
            }
            catch (Exception ex)
            {
                WriteToEventLog(WriteException + logLine, ex);
            }
            finally
            {
                lockContext.ExitWriteLock();
            }
        }

        /// <summary>
        /// Checks to see if a log file already exists and chooses that one instead of creating new
        /// </summary>
        private void FindLastLogFile()
        {
            if (Directory.Exists(this.filePath))
            {
                var files = Directory.EnumerateFiles(this.filePath, "*.log", SearchOption.TopDirectoryOnly);
                if (files.Any())
                {
                    foreach (var file in files)
                    {
                        if (file.IndexOf(ServiceLogKey, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var first = file.LastIndexOf('_') + 1;
                            var parsed = file.Substring(first, file.IndexOf('.') - first);
                            int id = 0;
                            if (int.TryParse(parsed, out id))
                            {
                                if (id > this.currentFileId)
                                {
                                    this.currentFileId = id;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a file for writting
        /// </summary>
        private void InitializeFile()
        {
            // TODO: try to test all of the conditions below.
            try
            {
                this.currentFileName = string.Format(this.serviceLogFileName, currentFileId.ToString());
                var filePath = Path.Combine(this.filePath, this.currentFileName);
                this.file = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            }
            catch (ArgumentException argEx)
            {
                // if we are getting this exception something is wrong with the path
                const string message = "Argument Exception trying to log to file. Check configured file path.";
                WriteToEventLog(message, argEx);
            }
            catch (SecurityException secEx)
            {
                // if we are getting this exception it is because of security to the file or path
                throw secEx;
            }
            catch (DirectoryNotFoundException dirEx)
            {
                // if the directory configured in the path doesnt exist
                const string message = "Specified Directory does not exist {0}";
                WriteToEventLog(string.Format(message, filePath), dirEx);
            }
            catch (UnauthorizedAccessException noauthEx)
            {
                // if the write access to the file is not authorized
                throw noauthEx;
            }
            catch (Exception ex)
            {
                // this could happen if for some reason the file is locked by another process
                throw ex;
            }
        }

        /// <summary>
        /// General method for logging events to the event viewer. This is used to ensure that if things 
        /// start crashing we can get notice of it in the OS Event Log 
        /// </summary>
        /// <param name="message">The Message to Log</param>
        /// <param name="exception">The exception details</param>
        /// <param name="entryType">The LogEntryType of the event</param>
        private void WriteToEventLog(
            string message,
            Exception exception, 
            EventLogEntryType entryType = EventLogEntryType.Information)
        {
            EventLog.WriteEntry(
                ApplicationEventSource, 
                string.Format("{0} {1}:{2}", message, exception.Message, exception.StackTrace), 
                EventLogEntryType.Error);
        }
    }
}
