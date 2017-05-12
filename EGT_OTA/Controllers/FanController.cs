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
    /// 关注、粉丝管理
    /// </summary>
    public class FanController : BaseController
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
                var number = ZNRequest.GetString("ToUserNumber");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return Json(new { result = false, message = "信息异常,请刷新重试" }, JsonRequestBehavior.AllowGet);
                }
                Fan model = db.Single<Fan>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == number);
                if (model == null)
                {
                    model = new Fan();
                    model.CreateUserNumber = user.Number;
                    model.ToUserNumber = number;
                }
                else
                {
                    return Json(new { result = true, message = "exist" }, JsonRequestBehavior.AllowGet);
                }
                model.CreateDate = DateTime.Now;
                model.CreateIP = Tools.GetClientIP;
                var result = false;
                if (model.ID == 0)
                {
                    result = Tools.SafeInt(db.Add<Fan>(model)) > 0;
                }
                else
                {
                    result = db.Update<Fan>(model) > 0;
                }
                if (result)
                {
                    return Json(new { result = true, message = "已关注" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("FanController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检测是否关注
        /// </summary>
        public ActionResult Check()
        {
            User user = GetUserInfo();
            if (user == null)
            {
                return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var message = string.Empty;
            var CreateUserNumber = ZNRequest.GetString("CreateUserNumber");
            var ToUserNumber = ZNRequest.GetString("ToUserNumber");
            if (string.IsNullOrWhiteSpace(CreateUserNumber) || string.IsNullOrWhiteSpace(ToUserNumber))
            {
                return Json(new { result = false, message = "信息异常,请刷新重试" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var result = db.Exists<Fan>(x => x.CreateUserNumber == CreateUserNumber && x.ToUserNumber == ToUserNumber);
                return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("FanController_Check:" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = false, message = message }, JsonRequestBehavior.AllowGet);
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
                var ToUserNumber = ZNRequest.GetString("ToUserNumber");
                var model = db.Single<Fan>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == ToUserNumber);
                if (model == null)
                {
                    return Json(new { result = false, message = "数据不存在" }, JsonRequestBehavior.AllowGet);
                }
                if (model.CreateUserNumber != user.Number)
                {
                    return Json(new { result = false, message = "没有权限" }, JsonRequestBehavior.AllowGet);
                }
                var result = db.Delete<Fan>(model.ID) > 0;
                if (result)
                {
                    return Json(new { result = true, message = string.Empty }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("FanController_Delete:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
