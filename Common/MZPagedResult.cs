using System.Collections.Generic;

namespace MegaZord.Library.Common
{
    /// <summary>
    /// CASTRO - Remover trocar para MZPagedList
    /// </summary>

    public class MZPagedResultBase
    {        
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

    }

    public class MZPagedResult<T> : MZPagedResultBase
    {
        public IList<T> Results { get; set; }
      

        public MZPagedResult()
        {
            this.Results = new List<T>();
        }
    
    }
}
