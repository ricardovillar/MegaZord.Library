using MegaZord.Library.EF.ViewCache;
using MegaZord.Library.Helpers;
using MegaZord.Library.Interfaces;
using MegaZord.Library.Log;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;


namespace MegaZord.Library.EF
{

    public abstract class MZEFDataContext : DbContext, IMZEFDataContext
    {
        protected MZEFDataContext(string connection)
            : base(connection)
        {
            if (MZInteractiveViews.CanAddMapViewCache(this))
            {
                var pathFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MZViews.xml");

                MZInteractiveViews.SetViewCacheFactory(this, new MZFileViewCacheFactory(pathFile));
            }
        }
        protected MZEFDataContext()
            : this(MZHelperConfiguration.MZConnectionString)
        { }

        public DbSet<MZParametro> Parametros { get; set; }

        #region métodos da inteface
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class, IMZEntity
        {

            return base.Set<TEntity>();
        }


        public DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return base.Database.SqlQuery<TElement>(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {

            return base.Database.ExecuteSqlCommand(sql, parameters);
        }

        public void RefreshEntity(RefreshMode refreshMode, IMZEntity entity)
        {
            ((IObjectContextAdapter)this).ObjectContext.Refresh(RefreshMode.StoreWins, entity);

        }

        public void ReloadReference<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property)
            where TEntity : class, IMZEntity
            where TProperty : class, IMZEntity
        {
            this.Entry<TEntity>(entity).Reference(property).Query();
        }

        public new DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class, IMZEntity
        {
            return base.Entry(entity);
        }
        #endregion


        #region métodos de configuração

        protected abstract void DetermineTables(DbModelBuilder modelBuilder);
        protected abstract void ModelConfiguration(DbModelBuilder modelBuilder);
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.Configuration.LazyLoadingEnabled = true;

            this.ModelConfiguration(modelBuilder);
            // this.DetermineTables(modelBuilder);
            DbInterception.Add(new MZLogCommandInterceptor());
            base.OnModelCreating(modelBuilder);
        }
        #endregion


        #region Savlar registros



        public DbContextTransaction BeginTrans(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted)
        {
            return base.Database.BeginTransaction(isolationLevel);
        }
        public void Rollback()
        {
            base.ChangeTracker.Entries().ToList().ForEach(entry => entry.State = EntityState.Unchanged);

        }
        public override int SaveChanges()
        {
            try
            {

                // Identifica as entidades que devem gerar registros em log.
                var entries = DetectEntries();

                // Cria lista para armazenamento temporário dos registros em log.
                var logs = entries.Select(GetLog).Where(log => log != null).ToList();


                //((IObjectContextAdapter)this).ObjectContext.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                var retorno = base.SaveChanges();
                foreach (var mzAuditLog in logs)
                {


                    GlobalDiagnosticsContext.Set("Action", mzAuditLog.Action);
                    GlobalDiagnosticsContext.Set("Data", mzAuditLog.Date.ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture));
                    GlobalDiagnosticsContext.Set("TableName", mzAuditLog.TableName);
                    GlobalDiagnosticsContext.Set("TableIdValue", mzAuditLog.TableIdValue.ToString(CultureInfo.InvariantCulture));
                    GlobalDiagnosticsContext.Set("UserId", mzAuditLog.UserId);
                    GlobalDiagnosticsContext.Set("OriginalValues", mzAuditLog.OriginalValues);
                    GlobalDiagnosticsContext.Set("SerializedOriginalObject", mzAuditLog.SerializedOriginalObject);
                    GlobalDiagnosticsContext.Set("NewValues", mzAuditLog.NewValues);
                    GlobalDiagnosticsContext.Set("SerializedNewObject", mzAuditLog.SerializedNewObject);

                    MZHelperInjection.MZGetLogFactory().CurrentAuditLog.Log(mzAuditLog.ToString());
                }

                return retorno;
            }
            catch (DbEntityValidationException e)
            {

                MZHelperInjection.MZGetLogFactory().CurrentErrorLog.Log("Erro DbEntityValidationException", e);
                throw;
            }
        }
        public void Commit()
        {
            this.SaveChanges();
        }
        #endregion


        #region Métodos Privado
        /// <summary>
        /// Identifica quais entidades devem ser gerar registros de log.
        /// </summary>
        private IEnumerable<DbEntityEntry> DetectEntries()
        {
            // Detecta as alterações existentes na instância corrente do DbContext.

            this.ChangeTracker.DetectChanges();
            return ChangeTracker.Entries().Where(e => (e.State == EntityState.Modified ||
                                                        e.State == EntityState.Added ||
                                                        e.State == EntityState.Deleted)
                && e.Entity.GetType() != typeof(MZAuditLog)).ToList();
        }

        /// <summary>
        /// BUsca a chave do objeto no contexto par alogar
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private long GetKeyValue(DbEntityEntry entry)
        {

            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            long id = 0;
            if (objectStateEntry.EntityKey.EntityKeyValues != null)
                id = Convert.ToInt64(objectStateEntry.EntityKey.EntityKeyValues[0].Value);

            return id;
        }

        /// <summary>
        /// Cria os registros de log.
        /// </summary>
        private MZAuditLog GetLog(DbEntityEntry entry)
        {

            MZAuditLog returnValue = null;
            var keyValue = GetKeyValue(entry);
            switch (entry.State)
            {
                case EntityState.Added:
                    returnValue = new MZInsertAuditLog(entry, keyValue);
                    break;
                case EntityState.Modified:
                    returnValue = new MZUpdateAuditLog(entry, keyValue);
                    break;
                case EntityState.Deleted:
                    returnValue = new MZDeleteAuditLog(entry, keyValue);
                    break;
            }

            return returnValue;
        }
        #endregion

    }
}

