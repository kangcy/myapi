using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace EGT_OTA.Controllers.Filter
{
    /// <summary>
    /// 响应压缩
    /// </summary>
    public class DeflateCompressionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            HttpResponse Response = HttpContext.Current.Response;
            Response.Filter = new System.IO.Compression.DeflateStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);
            Response.AppendHeader("Content-Encoding", "deflate");
            Response.AppendHeader("Content-Type", "application/json");
        }
    }
}