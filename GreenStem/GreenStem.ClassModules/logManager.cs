using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static GreenStem.ClassModules.LogGuserLocked;

namespace GreenStem.ClassModules
{
    public static class LogManager
    {
        private const string LogsDeletedEventName = "LogsDeletedEvent";
        public static event EventHandler<List<LogData>> LogsToDeleteEvent;

        //public delegate void DataSelectedHandler(Dictionary<string, object> selectedData);
        //public event DataSelectedHandler DataSelected;
        public static void NotifyLogsDeleted(List<LogData> logsToDelete)
        {
            // Process the list of logs here
         

            // Create or open the named event
            using (EventWaitHandle logsDeletedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, LogsDeletedEventName))
            {
                // Signal the event to notify other instances
                logsDeletedEvent.Set();
            }
        }

        //private static void OnLogsToDelete(List<LogData> logs)
        //{
        //    LogsToDeleteEvent?.Invoke(null, logs);
        //}

        //public static void NotifyLogsDeleted(List<LogData> logsToDelete)
        //{
        //    // Process the list of logs here
        //    OnLogsToDelete(logsToDelete);
        //}
    }
}
