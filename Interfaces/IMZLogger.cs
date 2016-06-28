using System;

namespace MegaZord.Library.Interfaces
{

    public interface IMZLogger
    {

        void Log(string message, Exception ex = null, params object[] args);

    }

    public interface IMZLoggerFactory
    {
        IMZLogger CurrentErrorLog { get; }
        IMZLogger CurrentDebugLog { get; }
        IMZLogger CurrentSQLLog { get; }
        IMZLogger CurrentAuditLog { get; }

        void GenericLog(string logtype, string logvalue);

    }
}
