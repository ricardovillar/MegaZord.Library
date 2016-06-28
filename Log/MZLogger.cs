using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using MegaZord.Library.Common;
using MegaZord.Library.Helpers;
using MegaZord.Library.Interfaces;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System.Linq;
using NLog.Targets.Wrappers;

namespace MegaZord.Library.Log
{


    /// <summary>
    /// Implementation of contract <see cref="IMZLogger"/>
    /// using System.Diagnostics API.
    /// </summary>
    public sealed class MZLogger : IMZLogger
    {
        private readonly string _logName;
        private readonly string _fileName;
        private readonly LogType _type;



        public MZLogger(string logName, string fileName, LogType type)
        {
            if (string.IsNullOrEmpty(logName))
                logName = MZHelperConfiguration.MZAppName;

            if (string.IsNullOrEmpty(fileName))
                fileName = MZHelperConfiguration.MZAppName;

            _logName = logName;
            _type = type;
            _fileName = fileName;



        }

        private LogTarget LogTargets
        {
            get
            {

                var logTarget = LogTarget.File;
                switch (_type)
                {
                    case LogType.Debug:
                        logTarget = LogTarget.File;
                        break;
                    case LogType.Error:
                        logTarget = LogTarget.File | LogTarget.Email;
                        break;
                    case LogType.SQL:
                        logTarget = LogTarget.File;
                        break;
                    case LogType.Audit:
                        logTarget = LogTarget.File | LogTarget.DB;
                        break;
                    case LogType.Generic:
                        logTarget = LogTarget.File | LogTarget.DB;
                        break;

                }
                return logTarget;
            }
        }

        private LogLevel LogLevel
        {
            get
            {

                var loglevel = LogLevel.Debug;
                switch (_type)
                {
                    case LogType.Debug:
                        loglevel = LogLevel.Debug;
                        break;
                    case LogType.Error:
                        loglevel = LogLevel.Error;
                        break;
                    case LogType.Generic:
                        loglevel = LogLevel.Debug;
                        break;
                    case LogType.SQL:
                    case LogType.Audit:

                        loglevel = LogLevel.Info;
                        break;
                    default:
                        loglevel = LogLevel.Error;
                        break;

                }
                return loglevel;
            }

        }

        [Flags]
        internal enum LogTarget
        {
            File = 2,
            Email = 4,
            DB = 8
        }


        private FileTarget GetFileTarget()
        {
            var fileTarget = new FileTarget
            {
                FileName = string.Format(MZConsts.MZLogger.LoggerBaseFileName, _fileName),
                ArchiveFileName = string.Format(MZConsts.MZLogger.LoggerBaseOldFileName, _fileName, _logName),
                ArchiveDateFormat = MZConsts.MZLogger.LoggerArchiveDateFormat,
                Layout = FileLayout,
                ArchiveAboveSize = MZHelperConfiguration.MZLog.MZFileLogSize,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ArchiveEvery = FileArchivePeriod.Hour,
                AutoFlush = true,
                BufferSize = 1024,
                CreateDirs = false,
                KeepFileOpen = false,
                MaxArchiveFiles = 999999,
                ConcurrentWrites = true,
                ReplaceFileContentsOnEachWrite = false,
                EnableFileDelete = true,
                LineEnding = LineEndingMode.CRLF,
                Name = MZConsts.MZLogger.LoggerFileTargetName
            };

            return fileTarget;

        }

        private MailTarget GetEmailTarget()
        {
            var email = new MailTarget
            {

                SmtpAuthentication = SmtpAuthenticationMode.Basic,


                EnableSsl = true,
                From = MZHelperConfiguration.MZemail.MZNormalSend.MZFrom,
                SmtpUserName = MZHelperConfiguration.MZemail.MZNormalSend.MZUserName,
                SmtpPassword = MZHelperConfiguration.MZemail.MZNormalSend.MZPassword,
                SmtpPort = MZHelperConfiguration.MZemail.MZNormalSend.MZPort,
                SmtpServer = MZHelperConfiguration.MZemail.MZNormalSend.MZServer,
                Subject = string.Format(MZConsts.MZLogger.LoggerEmailTargetSubject, _logName, MZHelperConfiguration.MZAppName, MZHelperConfiguration.MZAppUrl),
                To = MZHelperConfiguration.MZemail.MZDefaultReceiver,
                Layout = EmailLayout,
                ReplaceNewlineWithBrTagInHtml = true,
                AddNewLines = true,
                Html = true,
                Name = MZConsts.MZLogger.LoggerEmailTargetName
            };
            return email;
        }


        private DatabaseParameterInfo GetParameter(string name, string layout)
        {

            var paramDB = new DatabaseParameterInfo
                {
                    Name = name,
                    Layout = layout
                };
            return paramDB;

        }

        private DatabaseTarget GetDataBaseTarget()
        {

            var isAudit = _type == LogType.SQL ? true : false;
            DatabaseTarget target = new DatabaseTarget();

            target.DBProvider = "mssql";
            target.ConnectionString = MZHelperConfiguration.MZConnectionString;


            target.CommandText = isAudit ? MZConsts.MZLogger.LoggerDataBaseInsertAudit : MZConsts.MZLogger.LoggerDataBaseInsertGeneric;
            target.Name = MZConsts.MZLogger.LoggerDataBaseTargetName;


            if (isAudit)
            {
                target.Parameters.Add(GetParameter("@action", "${gdc:Action}"));
                target.Parameters.Add(GetParameter("@data", "${gdc:Data}"));
                target.Parameters.Add(GetParameter("@tableName", "${gdc:TableName}"));
                target.Parameters.Add(GetParameter("@tableIdValue", "${gdc:TableIdValue}"));
                target.Parameters.Add(GetParameter("@userId", "${gdc:UserId}"));
                target.Parameters.Add(GetParameter("@originalvalues", "${gdc:OriginalValues}"));
                target.Parameters.Add(GetParameter("@serializedoriginalobject", "${gdc:SerializedOriginalObject}"));
                target.Parameters.Add(GetParameter("@newvalues", "${gdc:NewValues}"));
                target.Parameters.Add(GetParameter("@serializednewobject", "${gdc:SerializedNewObject}"));
                target.Parameters.Add(GetParameter("@alllog", "${message}"));
            }
            else
            {
                target.Parameters.Add(GetParameter("@logtype", "${gdc:LogType}"));
                target.Parameters.Add(GetParameter("@logvalue", "${gdc:LogValue}"));
            }


            return target;

        }


        private AsyncTargetWrapper GetAsyncTargetWrapper(Target wrappedTarget)
        {
            var wrapper = new AsyncTargetWrapper
                {
                    WrappedTarget = wrappedTarget,
                    QueueLimit = 5000,
                    OverflowAction = AsyncTargetWrapperOverflowAction.Discard,

                };
            return wrapper;
        }

        #region Define a menssagem
        private string MessageToTrace(string message, Exception ex, params object[] args)
        {


            message = message.Replace("}", "}}").Replace("{", "{{");
            var messageToTrace = !string.IsNullOrEmpty(message) ? string.Format(CultureInfo.InvariantCulture, message, args) : string.Empty;

            var mzServError = ex != null ? new MZServerError(ex) : null;

            var exceptionErrorMsg = mzServError != null ? string.Format("{0}{1}", mzServError.FullMessageError, Environment.NewLine) : string.Empty;


            var msg = string.Format("{0}{0}Informação:{0}{1}{0}{0}{2}",
                                 System.Environment.NewLine,
                                 messageToTrace.Trim(),
                                 exceptionErrorMsg.Trim());
            return msg.Trim();
        }
        #endregion

        private string GetLoggerObjective
        {
            get
            {
                var retorno = MZConsts.MZLogger.LoggerDefaultObjective;
                switch (_type)
                {
                    case LogType.Debug:
                        retorno = MZConsts.MZLogger.LoggerDebugObjective;
                        break;
                    case LogType.Error:
                        retorno = MZConsts.MZLogger.LoggerErrorObjective;
                        break;
                    case LogType.SQL:
                        retorno = MZConsts.MZLogger.LoggerSQLObjective;
                        break;
                    case LogType.Audit:
                        retorno = MZConsts.MZLogger.LoggerAuditObjective;
                        break;
                    case LogType.Generic:
                        retorno = MZConsts.MZLogger.LoggerGenericObjective;
                        break;
                    default:
                        retorno = MZConsts.MZLogger.LoggerDefaultObjective;
                        break;

                }
                return retorno;
            }
        }

        private LayoutWithHeaderAndFooter FileLayout
        {
            get
            {
                var layout = new LayoutWithHeaderAndFooter
                    {
                        Header = new SimpleLayout(GetLoggerObjective),
                        Layout = new SimpleLayout(MZConsts.MZLogger.LoggerBodyLayout),
                        Footer = new SimpleLayout(MZConsts.MZLogger.LoggerFooterLayout)
                    };
                return layout;
            }
        }

        private LayoutWithHeaderAndFooter EmailLayout
        {
            get
            {
                return FileLayout;
            }
        }

        private void AsyncTargetMark(LoggingConfiguration config, Target target, string targetName)
        {
            var wrapper = GetAsyncTargetWrapper(target);
            config.AddTarget(targetName, wrapper);
            var rule = new LoggingRule("*", LogLevel, wrapper);
            config.LoggingRules.Add(rule);

        }

        private LoggingConfiguration GetLogConfiguration()
        {
            var config = new LoggingConfiguration();

            if (LogTargets.HasFlag(LogTarget.File))
            {
                var fileTarget = GetFileTarget();
                AsyncTargetMark(config, fileTarget, MZConsts.MZLogger.LoggerFileTargetName);
            }

            if (LogTargets.HasFlag(LogTarget.Email))
            {
                var emailtarget = GetEmailTarget();
                AsyncTargetMark(config, emailtarget, MZConsts.MZLogger.LoggerEmailTargetName);
            }

            if (LogTargets.HasFlag(LogTarget.DB))
            {
                var dbTarget = GetDataBaseTarget();
                AsyncTargetMark(config, dbTarget, MZConsts.MZLogger.LoggerDataBaseTargetName);
            }

            return config;
        }

        private Logger GetLogger()
        {



            LogManager.Configuration = GetLogConfiguration();


            var logger = LogManager.GetLogger(_logName);

            var target = LogManager.Configuration.AllTargets.FirstOrDefault(x => x is FileTarget);
            if (target != null)
            {

                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                var fileName = ((FileTarget)target).FileName.Render(logEventInfo);
                var archivefile = ((FileTarget)target).ArchiveFileName.Render(logEventInfo);
                CreateDirectory(fileName);
                CreateDirectory(archivefile);
            }


            return logger;

        }

        private void CreateDirectory(string fileName)
        {
            var lastIndex = fileName.LastIndexOf("\\", StringComparison.Ordinal);
            var dir = fileName.Substring(0, lastIndex);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private bool CanLogInformation
        {
            get
            {
                var retorno = true;
                switch (_type)
                {
                    case LogType.Debug:
                        retorno = MZHelperConfiguration.MZLog.MZLogDebug;
                        break;
                    case LogType.Error:
                        retorno = MZHelperConfiguration.MZLog.MZLogError;
                        break;
                    case LogType.SQL:
                        retorno = MZHelperConfiguration.MZLog.MZLogSQL;
                        break;
                    case LogType.Audit:
                        retorno = MZHelperConfiguration.MZLog.MZLogAudit;
                        break;
                    case LogType.Generic:
                        retorno = true;
                        break;
                    default:
                        retorno = true;
                        break;

                }
                return retorno;



            }
        }

        private void LogInternal(string message, Exception ex, params object[] args)
        {
            if (CanLogInformation)
            {
                var messageToTrace = MessageToTrace(message, ex, args);
                var logger = GetLogger();
                logger.Log(LogLevel, messageToTrace);
            }

        }

        #region LogExternals

        public void Log(string message, Exception ex = null, params object[] args)
        {
            LogInternal(message, ex, args);
        }


        #endregion
    }
}
