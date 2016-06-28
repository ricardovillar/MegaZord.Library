using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MegaZord.Library.MVC
{
    public class MZMVCResponseCapture : IDisposable
    {
        private readonly HttpResponseBase response;
        private readonly TextWriter originalWriter;
        private StringWriter localWriter;
        public MZMVCResponseCapture(HttpResponseBase response)
        {
            this.response = response;
            originalWriter = response.Output;
            localWriter = new StringWriter();
            response.Output = localWriter;
        }
        public override string ToString()
        {
            localWriter.Flush();
            return localWriter.ToString();
        }
        public void Dispose()
        {
            if (localWriter != null)
            {
                localWriter.Dispose();
                localWriter = null;
                response.Output = originalWriter;
            }
            GC.SuppressFinalize(this);
            
        }
    }
}
