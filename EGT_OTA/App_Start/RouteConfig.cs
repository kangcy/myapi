using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EGT_OTA
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //文章短链路由
            routes.MapRoute(
                name: "short",
                url: "{number}",
                defaults: new { controller = "Home", action = "Short" }
            );

            //用户短链路由
            routes.MapRoute(
                name: "ushort",
                url: "u/{number}",
                defaults: new { controller = "Home", action = "UserShort" }
            );

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
            );
        }
    }
}
