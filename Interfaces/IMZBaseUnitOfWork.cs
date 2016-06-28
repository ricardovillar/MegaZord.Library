using System;
using MegaZord.Library.EF;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;

namespace MegaZord.Library.Interfaces
{
    public interface IMZBaseUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
        void StartTrans(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted);

        IDbSet<TEntity> DBSet<TEntity>() where TEntity : class, IMZEntity;


        void RefreshEntity(RefreshMode refreshMode, IMZEntity entity);

        void ReloadReference<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property)
            where TEntity : class, IMZEntity
            where TProperty : class, IMZEntity;

        void SetStateInEntity<TEntity>(TEntity entity, EntityState state) where TEntity : class, IMZEntity;

        
        void ReloadEntity<TEntity>(TEntity entity) where TEntity : class, IMZEntity;

        DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters);


        int ExecuteSqlCommand(string sql, params object[] parameters);
    }
}
