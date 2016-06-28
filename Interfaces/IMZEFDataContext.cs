using MegaZord.Library.EF;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace MegaZord.Library.Interfaces
{
    public interface IMZEFDataContext 
    {

        /// <summary>
        /// Salva as atualizacoes no banco
        /// </summary>
        void Commit();

        /// <summary>
        /// Rever as atualizacoes
        /// </summary>
        void Rollback();

        /// <summary>
        /// Inicia uma nova transação
        /// </summary>
        /// <returns>Objeto da transação</returns>
        DbContextTransaction BeginTrans(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted);

        DbSet<TEntity> Set<TEntity>() where TEntity : class, IMZEntity;


        DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters);

        int ExecuteSqlCommand(string sql, params object[] parameters);
        //int ExecuteSqlCommand(TransactionalBehavior transactionalBehavior, string sql, params object[] parameters);


        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class, IMZEntity;

        void RefreshEntity(RefreshMode refreshMode, IMZEntity entity);

        void ReloadReference<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property)
            where TEntity : class, IMZEntity
            where TProperty : class, IMZEntity;
    }

}
