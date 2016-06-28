using MegaZord.Library.Common;
using MegaZord.Library.Interfaces;
using System.Data.Entity;

namespace MegaZord.Library.EF
{
    public class MZUnitOfWork : MZDisposable, IMZUnitOfWork
    {

        private readonly IMZEFDataContext _dataContext;

        private DbContextTransaction _transaction;

        public MZUnitOfWork(IMZEFDataContext dbContext)
        {
            this._dataContext = dbContext;

        }

        public void StartTrans(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted)
        {
            lock (this)
            {
                if (_transaction == null)
                    _transaction = _dataContext.BeginTrans(isolationLevel);
            }
        }


        public void Commit()
        {
             lock (this)
            {
                _dataContext.Commit();
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction = null;
                }


            }

        }

        public void Rollback()
        {
            lock (this)
            {
                _dataContext.Rollback();
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction = null;
                }

            }


        }

        private DbContextTransaction Transaction
        {
            get
            {
                if (_transaction == null)
                    StartTrans();
                return _transaction;
            }
        }



        private IMZEFDataContext GetDataContext()
        {
            return this._dataContext;
        }




        public IDbSet<TEntity> DBSet<TEntity>() where TEntity : class, IMZEntity
        {
            return this._dataContext.Set<TEntity>();
        }


        public void RefreshEntity(System.Data.Entity.Core.Objects.RefreshMode refreshMode, IMZEntity entity)
        {
            this._dataContext.RefreshEntity(refreshMode, entity);
        }

        public void ReloadReference<TEntity, TProperty>(TEntity entity, System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property)
            where TEntity : class, IMZEntity
            where TProperty : class, IMZEntity
        {
            this._dataContext.ReloadReference(entity, property);
        }

        public System.Data.Entity.Infrastructure.DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this._dataContext.SqlQuery<TElement>(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return this._dataContext.ExecuteSqlCommand(sql, parameters);
        }


        public void SetStateInEntity<TEntity>(TEntity entity, EntityState state) where TEntity : class, IMZEntity
        {
            this._dataContext.Entry(entity).State = EntityState.Modified;
        }


        public void ReloadEntity<TEntity>(TEntity entity) where TEntity : class, IMZEntity
        {
            this._dataContext.Entry<TEntity>(entity).Reload();
        }
    }
}
