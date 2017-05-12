using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EGT_OTA
{
    /// <summary>
    /// Web API 路由
    /// </summary>
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //支持跨域
            //这是重点，从配置文件的appsettings节点中读取跨域的地址
            var cors = new EnableCorsAttribute(ConfigurationManager.AppSettings["origins"], "*", "*");
            config.EnableCors(cors);

            config.Routes.MapHttpRoute(
                name: "Api",
                routeTemplate: "Api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
