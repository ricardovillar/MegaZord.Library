using System;
using System.Web.Mvc;
using System.IO.Compression;
using MegaZord.Library.Helpers;
using System.IO;
using System.Text.RegularExpressions;

namespace MegaZord.Library.MVC.Filters
{

    internal class WhitespaceFilter : Stream
    {

        // using Mads Kristensen httpModule
        // http://madskristensen.net/post/A-whitespace-removal-HTTP-module-for-ASPNET-20.aspx

        private Stream os;
        private static readonly Regex RegExBaseWhiteSpaceRemove = new Regex(@"^\s+", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RegexBetweenTags = new Regex(@">(?! )\s+", RegexOptions.Compiled);
        private static readonly Regex RegexLineBreaks = new Regex(@"([\n\s])+?(?<= {2,})<", RegexOptions.Compiled);

        public WhitespaceFilter(Stream os)
        {
            this.os = os;
        }

        //methods that need to be overridden from stream
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            os.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        private long _position;
        public override long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return os.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return os.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            os.SetLength(value);
        }

        public override void Close()
        {
            os.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string html = System.Text.Encoding.Default.GetString(buffer);

            //remove whitespace
            html = RegExBaseWhiteSpaceRemove.Replace(html, string.Empty);
            html = RegexBetweenTags.Replace(html, ">");
            html = RegexLineBreaks.Replace(html, "<");

            byte[] outdata = System.Text.Encoding.Default.GetBytes(html.Trim());

            //write bytes to stream
            os.Write(outdata, 0, outdata.GetLength(0));
        }
    }

    public class MZCompressResponse : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
            if (!MZHelperConfiguration.MZEnableMinify) return;

            var response = filterContext.HttpContext.Response;

            // - COMPRESS
            var request = filterContext.HttpContext.Request;
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!String.IsNullOrEmpty(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToUpperInvariant();

                if (acceptEncoding.Contains("GZIP"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
                else if (acceptEncoding.Contains("DEFLATE"))
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }

            // - REMOVE WHITE SPACE
            //response.Filter = new WhitespaceFilter(response.Filter);

            

        }
    }

}

