using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace carEVA.Utils
{
    enum LogLevels { full, warning };
    public static class evaLogUtils
    {
        private static LogLevels level = LogLevels.full;
        
        //error are always logged
        public static void logErrorMessage(string message, string origin)
        {
            //in theory starting a new thread this way will make use resources outside of the 
            // ASP thread pool. this way the service call can return even if the Log have not been written to the database
            //review http://stackoverflow.com/questions/8743067/do-asynchronous-operations-in-asp-net-mvc-use-a-thread-from-threadpool-on-net-4
            Thread th = new Thread(writeLogMessageInDatabase);
            th.Start(new evaLog() {
                level = "ERROR",
                message = message,
                caller = origin,
                date = DateTime.Now });
        }

        public static void logWarningMessage(string message, string origin)
        {
            if (level == LogLevels.full || level == LogLevels.warning)
            {
                Thread th = new Thread(writeLogMessageInDatabase);
                th.Start(new evaLog() {
                    level = "WARNING",
                    message = message,
                    caller = origin,
                    date = DateTime.Now });
            }
        }
        public static void logInfoMessage(string message, string origin)
        {
            if (level == LogLevels.full)
            {
                Thread th = new Thread(writeLogMessageInDatabase);
                th.Start(new evaLog() {
                    level = "INFO",
                    message = message,
                    caller = origin,
                    date = DateTime.Now });
            }
            
        }
        private static void writeLogMessageInDatabase(Object _message)
        {
            evaLog message;
            carEVAContext context = new carEVAContext();
            try
            {
                message = (evaLog)_message;
            }
            catch (InvalidCastException)
            {
                //the message is invalid, log this information
                context.evaLogs.Add(new evaLog() {
                    level = "ERROR",
                    message = "Exception while casting the response: " + _message.ToString(),
                    caller = "Thread Error",
                    date = DateTime.Now
                });
                context.SaveChanges();
                return;
            }
            //save the message in the database.
            context.evaLogs.Add(message);
            context.SaveChanges();
        }
    }
}