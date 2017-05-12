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
    /// 举报记录管理
    /// </summary>
    public class ReportController : BaseController
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
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                var summary = SqlFilter(ZNRequest.GetString("Summary"));
                if (string.IsNullOrWhiteSpace(ArticleNumber) || string.IsNullOrEmpty(summary))
                {
                    return Json(new { result = false, message = "信息异常" }, JsonRequestBehavior.AllowGet);
                }
                if (db.Exists<Report>(x => x.ArticleNumber == ArticleNumber && x.CreateUserNumber == user.Number && x.Status == Enum_Status.Audit))
                {
                    return Json(new { result = false, message = "正在处理中" }, JsonRequestBehavior.AllowGet);
                }
                Report model = new Report();
                model.CreateDate = DateTime.Now;
                model.CreateUserNumber = user.Number;
                model.CreateIP = Tools.GetClientIP;
                model.ArticleNumber = ArticleNumber;
                model.Summary = summary;
                model.Status = Enum_Status.Audit;
                var result = false;

                result = Tools.SafeInt(db.Add<Report>(model)) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ReportController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
