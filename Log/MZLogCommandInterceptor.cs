using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Text;
using MegaZord.Library.Helpers;

namespace MegaZord.Library.Log
{
    public class MZLogCommandInterceptor : IDbCommandInterceptor, IDbInterceptor
    {
        private void LogIfError<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (interceptionContext.Exception != null)
            {
                var logInfo = GetCommandoTextToLog(command);
                LogCommand(logInfo, interceptionContext.Exception);
            }
        }

        private void LogIfNonAsync<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (!interceptionContext.IsAsync)
            {
                var logInfo = GetCommandoTextToLog(command);
                LogCommand(logInfo);

            }
        }

        private string GetCommandoTextToLog(DbCommand command)
        {
            var newL = Environment.NewLine;
            var parametros = new StringBuilder();
            foreach (DbParameter parameter in command.Parameters)
            {
                var paramValue = parameter.Value != null ? parameter.Value.ToString().Trim() : "null";
                parametros.AppendLine(string.Format("Nome:{0} - Valor: {1}", parameter.ParameterName.Trim(), paramValue));
            }
            return string.Format("Comando:{0}{1}{0}{2}", newL, command.CommandText.Trim(), parametros.ToString().Trim());
        }

        private void LogCommand(string command, Exception ex = null)
        {
            MZHelperInjection.MZGetLogFactory().CurrentSQLLog.Log(command, ex);
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            this.LogIfError(command, interceptionContext);
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            this.LogIfNonAsync(command, interceptionContext);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.LogIfError(command, interceptionContext);
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.LogIfNonAsync(command, interceptionContext);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            this.LogIfError(command, interceptionContext);
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            this.LogIfNonAsync(command, interceptionContext);
        }
    }
}

