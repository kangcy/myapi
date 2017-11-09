using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Models;
using SubSonic.Repository;
using EGT_OTA.Helper;

namespace EGT_OTA.Controllers.Filter
{
    /// <summary>
    /// 后台操作权限过滤器
    /// </summary>
    public class BackPowerAttribute : ActionFilterAttribute
    {
        protected readonly SimpleRepository db = BasicRepository.GetRepo();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var key = ZNRequest.GetString("key");
            //if (string.IsNullOrWhiteSpace(key))
            //{
            //    filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    filterContext.Result = new JsonResult() { Data = new { result = false, message = "参数异常" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //    return;
            //}
            //User user = db.Single<User>(x => x.Number == key);
            //if (user == null)
            //{
            //    filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    filterContext.Result = new JsonResult() { Data = new { result = false, message = "用户不存在" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //    return;
            //}
            //if (user.UserRole != Enum_UserRole.Administrator && user.UserRole != Enum_UserRole.SuperAdministrator)
            //{
            //    filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //    filterContext.Result = new JsonResult() { Data = new { result = false, message = "没有权限", code = Enum_ErrorCode.NoPower }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //    return;
            //}

            var key = ZNRequest.GetString("key");
            var xwp = ZNRequest.GetString("xwp");
            var cookie = CookieHelper.GetCookieValue("Back");
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(xwp) || string.IsNullOrWhiteSpace(cookie))
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.Result = new RedirectResult("Login");
                return;
            }
            if (key != cookie)
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.Result = new RedirectResult("Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}