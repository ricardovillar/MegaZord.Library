
using MegaZord.Library.Common;
using MegaZord.Library.Interfaces;
using NLog;

namespace MegaZord.Library.Log
{
    /// <summary>
    /// Log Factory
    /// </summary>
    public class MZLoggerFactory : IMZLoggerFactory
    {
        public IMZLogger CurrentErrorLog
        {
            get { return new MZLogger("Error", "Log_Erro.txt", LogType.Error); }
        }
        public IMZLogger CurrentDebugLog
        {
            get { return new MZLogger("Debug", "Log_Debug.txt", LogType.Debug); }
        }
        public IMZLogger CurrentSQLLog
        {
            get { return new MZLogger("SQL", "Log_SQL.txt", LogType.SQL); }
        }
        public IMZLogger CurrentAuditLog
        {
            get { return new MZLogger("AUDIT", "Log_Audit.txt", LogType.Audit); }
        }
        private IMZLogger CurrentGenericLog
        {
            get { return new MZLogger("Generic", "Log_Generic.txt", LogType.Generic); }
        }
        public void GenericLog(string logtype, string logvalue)
        {
            GlobalDiagnosticsContext.Set("LogType", logtype);
            GlobalDiagnosticsContext.Set("LogValue", logvalue);
            CurrentGenericLog.Log(logtype + " _#####_ " + logvalue);
        }

    }
}
