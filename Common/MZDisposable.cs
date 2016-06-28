using System;

namespace MegaZord.Library.Common
{

 

    public class MZDisposable : IDisposable
    {
        private bool _isDisposed;

        ~MZDisposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                DisposeCore();
            }

            _isDisposed = true;
        }

        protected virtual void DisposeCore()
        {
        }
    }   
}

