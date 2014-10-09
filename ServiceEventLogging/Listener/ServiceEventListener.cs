using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceEventLogging.Listener
{
    internal class ServiceEventListener : EventListener
    {
        // this needs to be configured someway
        const string FileName = @"C:\logs\servicedata.log";
        readonly FileStream file;
        readonly SemaphoreSlim semaphore;

        public ServiceEventListener()
        {
            semaphore = new SemaphoreSlim(1);
            // Test for null
            var fileName = ConfigurationManager.AppSettings["LogFilePath"];

            try
            {
                file = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            }
            catch(ArgumentException argEx)
            {
                // if we are getting this exception something is wrong with the path
                throw argEx;
            }
            catch(SecurityException secEx)
            {
                // if we are getting this exception it is because of security to the file or path
                throw secEx;
            }
            catch(DirectoryNotFoundException dirEx)
            {
                // if the directory configured in the path doesnt exist
                throw dirEx;
            }
            catch(UnauthorizedAccessException noauthEx)
            {
                // if the write access to the file is not authorized
                throw noauthEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // make sure First() doesnt cause issues if null
            WriteToFileAsync(eventData.Payload.First().ToString());
        }

        private async void WriteToFileAsync(string logLine)
        {
            await semaphore.WaitAsync();
            var buffer = Encoding.UTF8.GetBytes(logLine);

            try
            {
                await file.WriteAsync(buffer, 0, buffer.Length);
            }
            catch
            {
                //what to do here? Event log i guess. this really shouldnt happen as exception should
                //be caught when we initialize. File IO is so unfun.
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
