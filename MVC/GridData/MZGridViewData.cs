using MegaZord.Library.Common;
using MegaZord.Library.Interfaces;
using System.Web.Mvc;

namespace MegaZord.Library.MVC
{
    public class MZGridViewData<T>
    {
        public IMZPagedList<T> PagedList
        {
            get;
            set;
        }

        public T EditItem
        {
            get;
            set;
        }
    }
}
