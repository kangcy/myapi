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
        /// ShareType（0：文章,1：用户,2：新手红包）
        /// </summary>
        public ActionResult Edit()
        {
            try
            {
                var UserNumber = ZNRequest.GetString("UserNumber");
                var Number = ZNRequest.GetString("Number");
                var ShareType = ZNRequest.GetInt("ShareType");
                ShareLog model = new ShareLog();
                model.CreateDate = DateTime.Now;
                model.CreateUserNumber = UserNumber;
                model.CreateIP = Tools.GetClientIP;
                model.ArticleNumber = ShareType == 0 ? Number : "";
                model.UserNumber = ShareType == 1 ? Number : "";
                model.Source = ZNRequest.GetString("Source");
                var result = false;

                result = Tools.SafeInt(db.Add<ShareLog>(model)) > 0;
                if (result)
                {
                    if (ShareType == 0 && !string.IsNullOrWhiteSpace(Number))
                    {
                        Article article = new SubSonic.Query.Select(provider, "ID", "Shares", "Number").From<Article>().Where<Article>(x => x.Number == Number).ExecuteSingle<Article>();
                        if (article != null)
                        {
                            result = new SubSonic.Query.Update<Article>(provider).Set("Shares").EqualTo(article.Shares + 1).Where<Article>(x => x.ID == article.ID).Execute() > 0;
                        }
                    }
                }
                return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ShareLogController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
