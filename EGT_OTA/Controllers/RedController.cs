using EGT_OTA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;

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
            var number = ZNRequest.GetString("number");
            if (string.IsNullOrWhiteSpace(number))
            {
                return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
            }

            var red = db.Exists<Red>(x => x.ToUserNumber == number);
            if (red)
            {
                return Json(new { result = false, message = "已领取红包" }, JsonRequestBehavior.AllowGet);
            }

            //添加红包记录
            Red model = new Red();
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
            model.Status = Enum_Status.Approved;
            model.Number = Guid.NewGuid().ToString("N");
            model.RedType = Enum_RedType.Login;
            model.CreateDate = DateTime.Now;
            db.Add<Red>(model);

            //用户新增红包
            return Json(new { result = true, message = model.Price }, JsonRequestBehavior.AllowGet);
        }
    }
}
