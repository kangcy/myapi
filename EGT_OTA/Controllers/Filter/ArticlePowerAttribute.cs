using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Models;
using SubSonic.Repository;

namespace EGT_OTA.Controllers.Filter
{
    /// <summary>
    /// 文章操作权限过滤器
    /// </summary>
    public class ArticlePowerAttribute : ActionFilterAttribute
    {
        protected readonly SimpleRepository db = BasicRepository.GetRepo();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var id = ZNRequest.GetInt("ID");
            if (id == 0)
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.Result = new JsonResult() { Data = new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            User user = db.Single<User>(x => x.ID == id);
            if (user == null)
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.Result = new JsonResult() { Data = new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            var ArticleID = ZNRequest.GetInt("ArticleID");
            if (ArticleID <= 0)
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.Result = new JsonResult() { Data = new { result = false, message = "参数异常" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            Article article = db.Single<Article>(x => x.ID == ArticleID);
            if (article == null)
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.Result = new JsonResult() { Data = new { result = false, message = "文章信息异常" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            if (article.CreateUserNumber != user.Number)
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.Result = new JsonResult() { Data = new { result = false, message = "没有权限" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            base.OnActionExecuting(filterContext);
            return;
        }
    }
}