using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MegaZord.Library.MVC
{
    public class MZDependencyResolver : IDependencyResolver
    {
        private readonly IResolutionRoot _resolutionRoot;

        public MZDependencyResolver(IResolutionRoot kernel)
        {
            _resolutionRoot = kernel;
        }

        public T Get<T>()
        {
            return _resolutionRoot.Get<T>();
        }

        public object GetService(Type serviceType)
        {
            return _resolutionRoot.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _resolutionRoot.GetAll(serviceType);

        }
    }
}