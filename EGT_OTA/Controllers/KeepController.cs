using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGT_OTA.Models;
using System.IO;
using Newtonsoft.Json;
using CommonTools;
using EGT_OTA.Helper;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 收藏管理
    /// </summary>
    public class KeepController : BaseController
    {
        /// <summary>
        /// 编辑
        /// </summary>
        public ActionResult Edit()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var articleID = ZNRequest.GetInt("ArticleID");
                if (articleID <= 0)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                Article article = new SubSonic.Query.Select(provider, "ID", "CreateUserNumber", "Number").From<Article>().Where<Article>(x => x.ID == articleID).ExecuteSingle<Article>();
                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                Keep model = db.Single<Keep>(x => x.CreateUserNumber == user.Number && x.ArticleNumber == article.Number);
                if (model == null)
                {
                    model = new Keep();
                    model.CreateDate = DateTime.Now;
                    model.CreateUserNumber = user.Number;
                    model.CreateIP = Tools.GetClientIP;
                }
                else
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
                model.ArticleNumber = article.Number;
                model.ArticleUserNumber = article.CreateUserNumber;
                var result = Tools.SafeInt(db.Add<Keep>(model)) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("KeepController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public ActionResult Delete()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                if (string.IsNullOrWhiteSpace(ArticleNumber))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var model = db.Single<Keep>(x => x.ArticleNumber == ArticleNumber && x.CreateUserNumber == user.Number);
                if (model == null)
                {
                    return Json(new { result = false, message = "数据不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = db.Delete<Keep>(model.ID) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("KeepController_Delete:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
