using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MegaZord.Library.Interfaces;
using MegaZord.Library.Helpers;
using System.Data.Entity.Core.Objects;
using MegaZord.Library.Exceptions;
using MegaZord.Library.Common;


namespace MegaZord.Library.EF
{

    public abstract class MZRepositoryBase<T> : IMZRepository<T> where T : MZEntity
    {

        #region Fields

        private readonly IMZUnitOfWork _unitOfWork;

        protected MZRepositoryBase(IMZUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        protected IDbSet<T> DBSet()
        {
            return DBSet<T>();

        }

        protected IDbSet<TEntity> DBSet<TEntity>() where TEntity : class, IMZEntity
        {
            return _unitOfWork.DBSet<TEntity>();
        }
        #endregion


        #region outros métodos



        /// <summary>
        /// Executa um comando (delete, update) na base
        /// </summary>
        /// <param name="sqlCommand">comando a ser executado</param>
        /// <param name="parameters">parâmetros da query</param>
        /// <returns>número de linhas de retorno</returns>
        protected int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return _unitOfWork.ExecuteSqlCommand(sqlCommand, parameters);
        }


        /// <summary>
        /// Faz o reload das informações de uma entidade
        /// </summary>
        /// <param name="entity"></param>
        protected void ReloadEntity<T2>(T2 entity) where T2 : class, IMZEntity
        {
            this._unitOfWork.ReloadEntity(entity);
        }
        /// <summary>
        /// Faz o reload das informações de uma entidade
        /// </summary>
        /// <param name="entity"></param>
        protected void RefreshEntity(T entity)
        {
            this._unitOfWork.RefreshEntity(RefreshMode.StoreWins, entity);
        }


        /// <summary>
        /// Faz o reload das informações de uma propriedade reference de uma entidade
        /// </summary>
        /// <param name="entity"></param>
        protected void ReloadReference<T2>(T entity, Expression<Func<T, T2>> property) where T2 : MZEntity
        {
            this._unitOfWork.ReloadReference(entity, property);

        }


        /// <summary>
        /// Executa uma query especifica que retorna as propriedades do objeto do tipo TEntity
        /// </summary>
        /// <typeparam name="TEntity">Tipo de objeto que será retornado</typeparam>
        /// <param name="sqlQuery">Query a ser executada</param>
        /// <param name="parameters">Parâmetro da query</param>
        /// <returns>Valor unico</returns>
        protected TEntity ExecuteScalarQuery<TEntity>(string sqlQuery, params object[] parameters)
        {
            return _unitOfWork.SqlQuery<TEntity>(sqlQuery, parameters).FirstOrDefault();
        }


        /// <summary>
        /// Executa uma query especifica que retorna as propriedades do objeto do tipo TEntity
        /// </summary>
        /// <typeparam name="TEntity">Tipo de objeto que será retornado</typeparam>
        /// <param name="sqlQuery">Query a ser executada</param>
        /// <param name="parameters">Parâmetro da query</param>
        /// <returns>Lista das entidades retornadas</returns>
        protected IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery, params object[] parameters)
        {
            return _unitOfWork.SqlQuery<TEntity>(sqlQuery, parameters);
        }





        /// <summary>
        /// Método que retorna um repository dentro de outro
        /// </summary>
        /// <typeparam name="TReposity">Tipo do repository quedeseja</typeparam>
        /// <returns>Instância do repository</returns>
        protected TReposity GetRepository<TReposity>() where TReposity : IMZRepository
        {

            return MZHelperInjection.MZGetRepository<TReposity>();
        }

        /// <summary>
        /// Busca o valor de um parâmetro
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetParameterValue(string key)
        {
            var lista = this.GetAnyList<MZParametro>(x => x.NomeParametro.ToUpper().Equals(key.ToUpper()));
            if (lista.Count == 0)
            {
                throw new MZZeroPrameterException(key);
            }

            if (lista.Count > 1)
            {
                throw new MZManyPrameterException(key);
            }

            return lista.First().ValorParametro;
        }

        private IList<T1> GetAnyList<T1>(Expression<Func<T1, bool>> where) where T1 : MZEntity
        {
            return this.DBSet<T1>().Where(where).ToList();
        }
        #endregion

        #region Metodos da Interface
        #region Metodos do CRUD

        private void Add(T entity)
        {

            DBSet().Add(entity);
        }

        private void Update(T entity)
        {
            DBSet().Attach(entity);
            _unitOfWork.SetStateInEntity<T>(entity, EntityState.Modified);
        }

        public virtual void Delete(T entity, bool autoCommit = true)
        {
            DBSet().Remove(entity);
            if (autoCommit)
                this._unitOfWork.Commit();
        }

        public virtual void Delete(long id, bool autoCommit = true)
        {
            T entity = Activator.CreateInstance<T>();
            entity.ID = id;
            this.DBSet().Attach(entity);
            Delete(entity, autoCommit);
        }

        public virtual void Delete(Expression<Func<T, bool>> where, bool autoCommit = true)
        {
            var objects = DBSet().Where(where).ToList();
            Parallel.ForEach(objects, o => Delete(o, autoCommit));
        }

        public void Save(T entity, bool autoCommit = true, bool autorefresh = false)
        {
            if (entity.IsNew)
                Add(entity);
            else
                Update(entity);

            if (autoCommit)
                this._unitOfWork.Commit();

            if (autorefresh)
            {


                this.ReloadEntity(entity);
                this.RefreshEntity(entity);
                if (entity.ID > 0)
                    entity = GetById(entity.ID);
                this.DBSet().Attach(entity);
            }
        }

        public virtual void StartTrans(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted)
        {
            _unitOfWork.StartTrans(isolationLevel);
        }

        public virtual void Commit()
        {
            _unitOfWork.Commit();
        }

        public virtual void Rollback()
        {
            _unitOfWork.Rollback();
        }

        #endregion

        #region Gets

        public virtual T GetById(long id)
        {

            return this.DBSet().FirstOrDefault(x => x.ID == id);
        }



        public virtual IList<T> GetAll()
        {
            return DBSet().AsQueryable().OrderBy(x => x.ID).ToList();

        }



        public virtual IMZPagedList<T> GetPagedList(int page = 1, int size = 10)
        {
            var asQueryable = DBSet().AsQueryable().OrderBy(x => x.ID);

            var pagedEntitys = asQueryable.ToPagedList(page, size);


            return pagedEntitys;

        }

        public virtual IList<T> GetMany(Expression<Func<T, bool>> where)
        {
            return DBSet().Where(where).ToList();
        }

        public virtual IList<T> GetMany(Expression<Func<T, bool>> where, int skip, int take)
        {
            return DBSet().Where(where).Skip(skip).Take(take).ToList();
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> where)
        {
            return DBSet().FirstOrDefault(where);
        }

        public virtual T FirstOrDefault()
        {
            return DBSet().FirstOrDefault();
        }

        public virtual T LastOrDefault(Expression<Func<T, bool>> where)
        {
            return DBSet().LastOrDefault(where);
        }
        public virtual T LastOrDefault()
        {
            return DBSet().LastOrDefault();
        }

        #endregion

        #region Apoio

        public virtual bool Any(Expression<Func<T, bool>> where)
        {
            return DBSet().Any(where);
        }


        public virtual long Count()
        {
            return DBSet().Count();
        }

        #endregion
        #endregion


        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

    }



    public class MZParametrosRepository : MZRepositoryBase<MZParametro>, IMZParametrosRepository
    {
        public MZParametrosRepository(IMZUnitOfWork unitOfWork) : base(unitOfWork) { }

    }
}
