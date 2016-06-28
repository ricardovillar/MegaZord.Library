using System;
using MegaZord.Library.Interfaces;
using MegaZord.Library.MVC;

namespace MegaZord.Library.Helpers
{
    public static class MZHelperInjection
    {

        private static T Get<T>()
        {
            
            return ((MZDependencyResolver)System.Web.Mvc.DependencyResolver.Current).Get<T>();
        }

        public static TReposity MZGetRepository<TReposity>() where TReposity : IMZRepository
        {

            return Get<TReposity>();

        }

        public static IMZLoggerFactory MZGetLogFactory()
        {
            return Get<IMZLoggerFactory>();
        }

        public static IMZUnitOfWork MZUnitOfWork()
        {
            return Get<IMZUnitOfWork>();
        }
    }
}
