using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using EGT_OTA.Models;

namespace EGT_OTA
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            //默认返回 json  
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));
            //返回格式选择 datatype 可以替换为任何参数   
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "xml", "application/xml"));

            new WebApplication().Start(System.Web.HttpContext.Current);

            BasicRepository.UpdateDB();
            MusicRepository.UpdateDB();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //在应用程序关闭时运行的代码
            new WebApplication().End(System.Web.HttpContext.Current);
        }

        void Application_Error(object sender, EventArgs e)
        {
            //在出现未处理的错误时运行的代码
            new WebApplication().Error(System.Web.HttpContext.Current);
        }

        protected void Application_BeginRequest()
        {
            HttpContext.Current.Items["DefaultConnection"] = EGT_OTA.Models.BasicRepository.GetRepo("DefaultConnection");
            HttpContext.Current.Items["MusicConnection"] = EGT_OTA.Models.MusicRepository.GetRepo("MusicConnection");
        }

        protected void Application_EndRequest()
        {
            HttpContext.Current.Items.Remove("DefaultConnection");
            HttpContext.Current.Items.Remove("MusicConnection");
        }
    }
}