using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using MegaZord.Library.Common;
using Newtonsoft.Json;
using System.Text;

namespace MegaZord.Library.Log
{
    public abstract class MZAuditLog
    {

        protected MZAuditLog(MZAuditLogOperation operation, DbEntityEntry entry, long keyValue)
        {
            Action = ((char)operation).ToString(CultureInfo.InvariantCulture);
            Date = DateTime.Now;
            UserId = System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.User.Identity.Name : "null";
            TableName = GetTableName(entry);
            TableIdValue = keyValue;

            if (operation == MZAuditLogOperation.InsertAction)
            {
                NewValues = GetAddedProperties(entry);
                SerializedNewObject = Serialize(entry.Entity);

            }
            else if (operation == MZAuditLogOperation.DeleteAction)
            {
                OriginalValues = GetDeletedProperties(entry);
                SerializedOriginalObject = Serialize(entry.GetDatabaseValues().ToObject());

            }
            else
            {

                var oldValues = new StringBuilder();
                var newValues = new StringBuilder();
                SetModifiedProperties(entry, oldValues, newValues);

                NewValues = newValues.ToString();
                SerializedNewObject = Serialize(entry.Entity);

                OriginalValues = oldValues.ToString();
                var dbValues = entry.GetDatabaseValues();

                SerializedOriginalObject = Serialize(dbValues != null ? dbValues.ToObject() : string.Empty);


            }


        }

        private string GetDeletedProperties(DbEntityEntry entry)
        {
            var data = new StringBuilder();
            var dbValues = entry.GetDatabaseValues();
            foreach (var propertyName in dbValues.PropertyNames)
            {
                var oldVal = dbValues[propertyName];
                if (oldVal != null)
                {
                    data.AppendFormat("{0}={1} || ", propertyName, oldVal);
                }
            }
            if (data.Length > 0)
                data = data.Remove(data.Length - 3, 3);
            return data.ToString();
        }

        private string GetAddedProperties(DbEntityEntry entry)
        {
            var data = new StringBuilder();
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var newVal = entry.CurrentValues[propertyName];
                if (newVal != null)
                {
                    data.AppendFormat("{0}={1} || ", propertyName, newVal);
                }
            }
            if (data.Length > 0)
                data = data.Remove(data.Length - 3, 3);
            return data.ToString();
        }

        private void SetModifiedProperties(DbEntityEntry entry, StringBuilder oldData, StringBuilder newData)
        {

            var dbValues = entry.GetDatabaseValues();
            if (dbValues == null)
                dbValues = entry.OriginalValues;
            foreach (var propertyName in entry.OriginalValues.PropertyNames)
            {
                if (dbValues != null)
                {
                    var oldVal = dbValues[propertyName];
                    var newVal = entry.CurrentValues[propertyName];
                    if (!Equals(oldVal, newVal))
                    {
                        newData.AppendFormat("{0}={1} || ", propertyName, newVal ?? "null");
                        oldData.AppendFormat("{0}={1} || ", propertyName, oldVal ?? "null");
                    }
                }
            }
            if (oldData.Length > 0)
                oldData.Remove(oldData.Length - 3, 3);
            if (newData.Length > 0)
                newData.Remove(newData.Length - 3, 3);
        }



        private string GetTableName(DbEntityEntry dbEntry)
        {
            var typeGetName = dbEntry.Entity.GetType();
            var tableAttr = typeGetName.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            if (tableAttr == null && typeGetName.BaseType != null)
                tableAttr = typeGetName.BaseType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            return tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

        }



        public string Action { get; private set; }
        public DateTime Date { get; private set; }
        public string TableName { get; private set; }
        public long TableIdValue { get; private set; }
        public string UserId { get; private set; }
        public string OriginalValues { get; private set; }
        public string SerializedOriginalObject { get; private set; }
        public string NewValues { get; private set; }
        public string SerializedNewObject { get; private set; }






        private static JsonSerializerSettings JsSetting()
        {
            return new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    MaxDepth = 10
                };
        }

        private string Serialize(object obj)
        {
            if (obj != null)
            {
                var jsSetting = JsSetting();
                return JsonConvert.SerializeObject(obj, Formatting.Indented, jsSetting);
            }
            else
            {
                return null;
            }
        }




        public static MZAuditLog Load(string value)
        {
            var jsSetting = JsSetting();
            return JsonConvert.DeserializeObject<MZAuditLog>(value, jsSetting);
        }

        public override string ToString()
        {
            return Serialize(this);

        }
    }

    public class MZInsertAuditLog : MZAuditLog
    {
        public MZInsertAuditLog(DbEntityEntry value, long keyValue)
            : base(MZAuditLogOperation.InsertAction, value, keyValue)
        {
        }
    }

    public class MZDeleteAuditLog : MZAuditLog
    {
        public MZDeleteAuditLog(DbEntityEntry value, long keyValue)
            : base(MZAuditLogOperation.DeleteAction, value, keyValue)
        {
        }
    }

    public class MZUpdateAuditLog : MZAuditLog
    {
        public MZUpdateAuditLog(DbEntityEntry value, long keyValue)
            : base(MZAuditLogOperation.UpdateAction, value, keyValue)
        {
        }
    }

}