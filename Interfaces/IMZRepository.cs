using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using MegaZord.Library.EF;


namespace MegaZord.Library.Interfaces
{

    public interface IMZRepository
    {

    }

    public interface IMZRepository<T> : IMZRepository where T : class, IMZEntity
    {

        
        void Delete(T entity, bool autoCommit = true);
        void Delete(Expression<Func<T, bool>> where, bool autoCommit = true);
        void Delete(long id, bool autoCommit = true);
        void Save(T entity, bool autoCommit = true, bool autorefresh = false);
        void Commit();
        void Rollback();
        void StartTrans(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted);

        T GetById(long id);
        
        IList<T> GetAll();
        
        IList<T> GetMany(Expression<Func<T, bool>> where);
        IList<T> GetMany(Expression<Func<T, bool>> where, int skip, int take);

        IMZPagedList<T> GetPagedList(int page = 1, int size = 10);
       
        
        T FirstOrDefault(Expression<Func<T, bool>> where);
        T FirstOrDefault();


        T LastOrDefault(Expression<Func<T, bool>> where);
        T LastOrDefault();
        
        bool Any(Expression<Func<T, bool>> where);

        long Count();
    }


    public partial interface IMZParametrosRepository : IMZRepository<MZParametro> { }
}
