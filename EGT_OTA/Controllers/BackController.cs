using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Models;
using EGT_OTA.Helper;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 后台
    /// </summary>
    public class BackController : BaseController
    {
        /// <summary>
        /// 登录审核
        /// </summary>
        public ActionResult CheckLogin()
        {
            try
            {
                var key = ZNRequest.GetString("key");
                if (string.IsNullOrWhiteSpace(key))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Number == key);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                if (user.UserRole == Enum_UserRole.Administrator || user.UserRole == Enum_UserRole.SuperAdministrator)
                {
                    return Json(new { result = false, message = "没有权限" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { result = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_CheckLogin:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        public ActionResult Article()
        {
            try
            {
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved && x.ArticlePower != Enum_ArticlePower.Myself);

                var title = SqlFilter(ZNRequest.GetString("Title"));
                if (!string.IsNullOrWhiteSpace(title))
                {
                    query.And("Title").Like("%" + title + "%");
                }
                var UserNumber = ZNRequest.GetString("UserNumber");
                if (!string.IsNullOrWhiteSpace(UserNumber))
                {
                    query = query.And("CreateUserNumber").IsEqualTo(UserNumber);
                }
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    return Json(new { result = true, records = 0, totalpage = 1 }, JsonRequestBehavior.AllowGet);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Article>();
                List<ArticleJson> newlist = ArticleListInfo(list, "");
                return Json(new { result = true, currpage = pager.Index, records = recordCount, totalpage = totalPage, list = newlist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_Article:" + ex.Message);
            }
            return Json(new { result = true, records = 0, totalpage = 1 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 文章删除
        /// </summary>
        public ActionResult ArticleDelete()
        {
            try
            {
                var key = ZNRequest.GetString("key");
                if (string.IsNullOrWhiteSpace(key))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Number == key);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                if (user.UserRole == Enum_UserRole.Administrator || user.UserRole == Enum_UserRole.SuperAdministrator)
                {
                    return Json(new { result = false, message = "没有权限" }, JsonRequestBehavior.AllowGet);
                }

                var id = ZNRequest.GetInt("ArticleID");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(provider).Set("Status").EqualTo(Enum_Status.DELETE).Where<Article>(x => x.ID == article.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleDelete:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

    }
}
