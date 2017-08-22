using EGT_OTA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Helper;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 红包
    /// </summary>
    public class RedController : BaseController
    {
        /// <summary>
        /// 新用户专享红包
        /// </summary>
        public ActionResult LoginIndex()
        {
            var key = ZNRequest.GetString("key");
            var nickname = "";
            var avatar = "";
            if (!string.IsNullOrWhiteSpace(key))
            {
                var user = db.Single<User>(x => x.Number == key);
                if (user != null)
                {
                    nickname = user.NickName;
                    avatar = user.Avatar;
                }
                else
                {
                    key = "";
                }
            }
            ViewBag.NickName = string.IsNullOrWhiteSpace(nickname) ? "您的好友" : nickname;
            ViewBag.Avatar = avatar;
            ViewBag.Number = key;
            return View();
        }

        /// <summary>
        /// 红包信息
        /// </summary>
        public ActionResult RedInfo()
        {
            string during = System.Web.Configuration.WebConfigurationManager.AppSettings["during"];
            string shareurl = System.Web.Configuration.WebConfigurationManager.AppSettings["shareurl"];
            string shareicon = System.Web.Configuration.WebConfigurationManager.AppSettings["shareicon"];
            var model = new { During = during, ShareUrl = shareurl, ShareIcon = shareicon };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登陆红包
        /// </summary>
        public ActionResult LoginRed()
        {
            try
            {
                var number = ZNRequest.GetString("number");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }

                var model = db.Single<Red>(x => x.ToUserNumber == number && x.RedType == Enum_RedType.Login);
                if (model != null)
                {
                    if (model.Status == Enum_Status.Approved)
                    {
                        return Json(new { result = false, message = "已领取红包" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (model == null)
                {
                    model = new Red();
                    model.ToUserNumber = number;
                    model.Price = new Random().Next(10, 500);
                    if (model.Price <= 0)
                    {
                        model.Price = 10;
                    }
                    if (model.Price > 500)
                    {
                        model.Price = 500;
                    }
                    model.Status = Enum_Status.Audit;
                    model.Number = Guid.NewGuid().ToString("N");
                    model.RedType = Enum_RedType.Login;
                    model.CreateDate = DateTime.Now;
                    db.Add<Red>(model);
                }
                //用户新增红包
                return Json(new { result = true, message = model.Price }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Red_LoginRed:" + ex.Message);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 打开登陆红包
        /// </summary>
        public ActionResult OpenLoginRed()
        {
            try
            {
                var number = ZNRequest.GetString("number");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }

                var model = db.Single<Red>(x => x.ToUserNumber == number && x.RedType == Enum_RedType.Login && x.Status == Enum_Status.Audit);
                if (model == null)
                {
                    return Json(new { result = false, message = "已领取红包" }, JsonRequestBehavior.AllowGet);
                }
                model.Status = Enum_Status.Approved;
                db.Update<Red>(model);
                return Json(new { result = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Red_OpenLoginRed:" + ex.Message);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 管理员打赏红包
        /// </summary>
        public ActionResult AdminRed()
        {
            try
            {
                var number = ZNRequest.GetString("number");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var exist = db.Find<Red>(x => x.ToUserNumber == number && x.RedType == Enum_RedType.Admin).ToList().Exists(x => x.CreateDate.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"));
                if (exist)
                {
                    return Json(new { result = false, message = "今日已打赏红包" }, JsonRequestBehavior.AllowGet);
                }
                var model = new Red();
                model.ToUserNumber = number;
                model.Price = ZNRequest.GetInt("price");
                if (model.Price <= 0 || model.Price > 500)
                {
                    model.Price = new Random().Next(10, 500);
                    if (model.Price <= 0)
                    {
                        model.Price = 10;
                    }
                    if (model.Price > 500)
                    {
                        model.Price = 500;
                    }
                }
                model.Status = Enum_Status.Audit;
                model.Number = Guid.NewGuid().ToString("N");
                model.RedType = Enum_RedType.Admin;
                model.CreateDate = DateTime.Now;
                db.Add<Red>(model);

                //推送消息

                //用户新增红包
                return Json(new { result = true, message = model.Price }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Red_AdminRed:" + ex.Message);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
