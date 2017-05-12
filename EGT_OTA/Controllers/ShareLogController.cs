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
    /// 分享记录管理
    /// </summary>
    public class ShareLogController : BaseController
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
                Article article = new SubSonic.Query.Select(provider, "ID", "Shares", "Number").From<Article>().Where<Article>(x => x.ID == articleID).ExecuteSingle<Article>();

                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }

                ShareLog model = new ShareLog();
                model.CreateDate = DateTime.Now;
                model.CreateUserNumber = user.Number;
                model.CreateIP = Tools.GetClientIP;
                model.ArticleNumber = article.Number;
                model.Source = ZNRequest.GetString("Source");
                var result = false;

                result = Tools.SafeInt(db.Add<ShareLog>(model)) > 0;
                //修改分享数
                if (result)
                {
                    result = new SubSonic.Query.Update<Article>(provider).Set("Shares").EqualTo(article.Shares + 1).Where<Article>(x => x.ID == articleID).Execute() > 0;
                }
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ShareLogController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
