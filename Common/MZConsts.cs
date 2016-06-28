namespace MegaZord.Library.Common
{
    public static class MZConsts
    {
        public static class MZLogger
        {
            public const string LoggerDefaultObjective = "O objetivo deste log é registrar informações.";
            public const string LoggerDebugObjective = "O objetivo deste log é registrar qualquer exception que ocorra na aplicação não é recomendado ativar por muito tempo.";
            public const string LoggerErrorObjective = "O objetivo deste log é registrar  exception não tratada que ocorra na aplicação  é recomendado ativar.";
            public const string LoggerSQLObjective = "O objetivo deste log é registrar qualquer comando SQL executado na infraestrutura oferecida.";
            public const string LoggerAuditObjective = "O objetivo deste log é auditar os objetos envolvidos em operações de Insert, Update, Delete.";
            public const string LoggerGenericObjective = "O objetivo deste log é ser generico, sempre loga qualquer informação não há como desligar, muito cuidado ao utilizar.";

            public const string LoggerBodyLayout = "Ocorreu em: ${date}${newline}${message}";
            public const string LoggerFooterLayout = "${newline}";

            public const string LoggerBaseFileName = "${{basedir}}logs\\{0}.txt";
            public const string LoggerBaseOldFileName = "${{basedir}}logs\\old\\{1}\\{0}_{{#####}}.txt";
            public const string LoggerArchiveDateFormat = "dd_MM_yyyy_hh_mm_ss";
            public const string LoggerFileTargetName = "MZFileTarget";


            public const string LoggerEmailTargetName = "MZEmailTarget";
            public const string LoggerEmailTargetSubject = "Log do tipo '{0}', na aplicação '{1}' na url '{2}'.";

            public const string LoggerDataBaseTargetName = "MZDataBaseTarget";

            public const string LoggerDataBaseInsertGeneric = @"INSERT INTO [MZ].[InformationLog] ([LogType], [LogValue]) VALUES (@logtype,@logvalue)";
            
            public const string LoggerDataBaseInsertAudit = @"INSERT INTO [MZ].[AuditLog]
           ([Action]
           ,[Data]
           ,[TableName]
           ,[TableIdValue]
           ,[UserId]
           ,[OriginalValues]
           ,[SerializedOriginalObject]
           ,[NewValues]
           ,[SerializedNewObject]
           ,[AllLog])
     VALUES
           (@action
           ,@Data
           ,@tablename
           ,@tableidvalue
           ,@userid
           ,@originalvalues
           ,@serializedoriginalobject
           ,@newvalues
           ,@serializednewobject
           ,@alllog)";

           

        }

        public static class MZControllerNamesConsts
        {
            public const string ViewDataErrorKey = "Error";
            public const string ViewNameAfterDelete = "List";
            public const string ViewNameToCreate = "Create";
            public const string ViewNameToEdit = "Edit";
            public const string ViewNameToIndex = "Index";
            public const string ViewNameToList = "List";
        }

        public static class MZDataBaseConfiguration
        {
            public const string ConnectioStringKey = "MZConnection";
            
        }

        public static class MZInfraConsts
        {
            public const string CulturePTBR = "pt-BR";
            public const string OcorreuUmErro = "Ocorreu um erro dentro de um controller";
            public const string ScriptRegister = "Script";
            public const string HtlBaseParameterName = "HTML_BASE";
            public const string CookieName = "MZFramework";
            public const string MatchEmailPattern = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                                     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                                       + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                                     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
        }        
    }
}
