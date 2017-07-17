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
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 系统管理
    /// </summary>
    public class SystemController : BaseController
    {
        /// <summary>
        /// 检查更新
        /// </summary>
        public ActionResult CheckUpdate()
        {
            try
            {
                var client = ZNRequest.GetString("client");
                var version = ZNRequest.GetString("version");

                if (string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(version))
                {
                    return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
                }

                var currVersion = "";
                var currUrl = "";
                if (client == "android")
                {
                    currVersion = System.Configuration.ConfigurationManager.AppSettings["curr_android_version"];
                    currUrl = System.Configuration.ConfigurationManager.AppSettings["curr_android_url"];
                }
                else
                {
                    currVersion = System.Configuration.ConfigurationManager.AppSettings["curr_ios_version"];
                    currUrl = System.Configuration.ConfigurationManager.AppSettings["curr_ios_url"];
                }

                if (version == currVersion)
                {
                    return Json(new { result = false, message = "当前已是最新版本" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = true, version = currVersion, url = currUrl, remark = System.Configuration.ConfigurationManager.AppSettings["curr_version_remark"] }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_CheckUpdate:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查提现申请
        /// </summary>
        public ActionResult CheckApplyMoney()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                if (db.Exists<ApplyMoney>(x => x.CreateUserNumber == user.Number && x.Status == Enum_Status.Audit))
                {
                    return Json(new { result = false, message = "已申请，申请后5个工作日内发放完毕" }, JsonRequestBehavior.AllowGet);
                }

                //打赏金额
                var orderMoney = db.Find<Order>(x => x.ToUserNumber == user.Number && x.Status == Enum_Status.Approved).Sum(x => x.Price);

                //提现次数
                var applyCount = db.Find<ApplyMoney>(x => x.CreateUserNumber == user.Number && x.Status == Enum_Status.Approved).Count;

                //剩余赏金
                user.Money = orderMoney - applyCount * Apply_Money * 100;
                if (user.Money < 0)
                {
                    user.Money = 0;
                }
                else
                {
                    user.Money = user.Money / 100;
                }
                if (user.Money < Apply_Money)
                {
                    return Json(new { result = false, message = "账户余额不足" + Apply_Money + "元,暂时无法提现" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = true, message = "申请成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error(ex.Message);
            }
            return Json(new { result = false, message = "申请失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        public ActionResult ApplyMoney()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                if (db.Exists<ApplyMoney>(x => x.CreateUserNumber == user.Number && x.Status == Enum_Status.Audit))
                {
                    return Json(new { result = false, message = "已申请，申请后5个工作日内发放完毕" }, JsonRequestBehavior.AllowGet);
                }

                //打赏金额
                var orderMoney = db.Find<Order>(x => x.ToUserNumber == user.Number && x.Status == Enum_Status.Approved).Sum(x => x.Price);

                //提现次数
                var applyCount = db.Find<ApplyMoney>(x => x.CreateUserNumber == user.Number && x.Status == Enum_Status.Approved).Count;

                //剩余赏金
                user.Money = orderMoney - applyCount * Apply_Money * 100;
                if (user.Money < 0)
                {
                    user.Money = 0;
                }
                else
                {
                    user.Money = user.Money / 100;
                }

                if (user.Money < Apply_Money)
                {
                    return Json(new { result = false, message = "账户余额不足" + Apply_Money + "元,暂时无法提现" }, JsonRequestBehavior.AllowGet);
                }

                var name = ZNRequest.GetString("Name");
                var account = ZNRequest.GetString("Account");
                ApplyMoney model = new ApplyMoney();
                model.Account = account;
                model.AccountName = name;
                model.CreateUserNumber = user.Number;
                model.Status = Enum_Status.Audit;
                model.CreateDate = DateTime.Now;
                model.CreateIP = Tools.GetClientIP;
                var result = Tools.SafeInt(db.Add<ApplyMoney>(model)) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "申请成功，5个工作日内发放完毕" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_ApplyMoney:" + ex.Message);
            }
            return Json(new { result = false, message = "申请失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 申请提现处理
        /// </summary>
        public ActionResult ApplyMoneyDeal()
        {
            try
            {
                var id = ZNRequest.GetInt("ID");
                var model = db.Single<ApplyMoney>(x => x.ID == id);
                if (model != null)
                {
                    model.Status = Enum_Status.Approved;
                    var result = db.Update<ApplyMoney>(model);
                    if (result > 0)
                    {
                        return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { result = false, message = "当前记录不存在" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_ApplyMoneyDeal:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public ActionResult SendSMS()
        {
            try
            {
                string mobile = ZNRequest.GetString("Mobile");
                string sms = ZNRequest.GetString("SMS");
                string num = new Random().Next(100000, 999999).ToString();
                var templateId = System.Web.Configuration.WebConfigurationManager.AppSettings["defaultTemplate"].ToString();
                switch (sms)
                {
                    //找回密码验证码
                    case "findpwdsms":
                        templateId = System.Web.Configuration.WebConfigurationManager.AppSettings["findpwdsmsTemplate"].ToString();
                        break;
                    //用户注册验证码
                    case "regsms":
                        templateId = System.Web.Configuration.WebConfigurationManager.AppSettings["regsmsTemplate"].ToString();
                        break;
                    //绑定手机号码
                    case "bindphone":
                        templateId = System.Web.Configuration.WebConfigurationManager.AppSettings["bindphoneTemplate"].ToString();
                        break;
                    default:
                        break;
                }

                var code = "0";
                var msg = string.Empty;

                //是否启用短信
                var usesms = System.Web.Configuration.WebConfigurationManager.AppSettings["usesms"].ToString();
                if (usesms == "1")
                {
                    //发送模板短信
                    var result = SmsHelper.Send(templateId, mobile, "@1@=" + num, "");
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
                    }
                    var model = JObject.Parse(result);
                    code = model["code"].ToString();
                    msg = model["msg"].ToString();
                }

                //发送记录
                SendSMS log = new SendSMS();
                log.Mobile = mobile;
                log.Remark = sms + "|" + msg;
                log.Result = code;
                log.Code = num;
                log.CreateDate = DateTime.Now;
                log.CreateIP = Tools.GetClientIP;
                db.Add<SendSMS>(log);

                if (code == "0")
                {
                    CookieHelper.SetCookie("SMS", mobile + sms + num, DateTime.Now.AddMinutes(15));
                    return Json(new { result = true, usesms = usesms, message = usesms == "1" ? msg : num }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_SendSMS:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验短信验证码
        /// </summary>
        public ActionResult CheckSMSCode()
        {
            string mobile = ZNRequest.GetString("Mobile");
            string code = ZNRequest.GetString("Code");
            string sms = ZNRequest.GetString("SMS");
            if (string.IsNullOrWhiteSpace(mobile) || string.IsNullOrWhiteSpace(code))
            {
                return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var value = CookieHelper.GetCookieValue("SMS");
                if (value == mobile + sms + code)
                {
                    CookieHelper.ClearCookie("SMS");
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_CheckSMSCode:" + ex.Message);
            }
            return Json(new { result = false, message = "验证码错误" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Banner列表
        /// </summary>
        public ActionResult Banner()
        {
            try
            {
                var list = GetBanner();
                var result = new
                {
                    records = list.Count(),
                    list = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_Banner:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 图片验证码
        /// </summary>
        public ActionResult Validate()
        {
            Bitmap bmp = new Bitmap(80, 40);
            Graphics g = Graphics.FromImage(bmp);
            SolidBrush sb = new SolidBrush(ValidateCodeHelper.GetColor());
            var code = ValidateCodeHelper.BuildCode(4);
            CookieHelper.SetCookie("Validate", code, DateTime.Now.AddMinutes(5));
            g.DrawString(code, new Font("宋体", 20), sb, 0, 0);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return File(ms.GetBuffer(), "application/x-png");
        }

        /// <summary>
        /// 判断远程文件是否存在
        /// </summary>
        public ActionResult FileExist()
        {
            string url = "http://m2.music.126.net/SYf_XliJkNYVx6yFfiYyEA==/5725157045913981.mp3";

            return Json(CheckFileExist(url), JsonRequestBehavior.AllowGet);
        }

        protected bool CheckFileExist(string url)
        {
            try
            {
                System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.CreateDefault(new Uri(url));
                httpWebRequest.Method = "HEAD";
                httpWebRequest.Timeout = 2000;
                return (((System.Net.HttpWebResponse)httpWebRequest.GetResponse()).StatusCode == System.Net.HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断敏感词
        /// </summary>
        [HttpPost]
        public ActionResult CheckDirty()
        {
            try
            {
                var title = UrlDecode(ZNRequest.GetString("Title"));
                if (ProcessSqlStr(title))
                {
                    return Json(new { result = false, message = "禁止SQL注入" }, JsonRequestBehavior.AllowGet);
                }
                if (HasDirtyWord(title))
                {
                    return Json(new { result = false, message = "您输入的内容含有敏感内容，请检查后重试哦" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_System_CheckDirty:" + ex.Message);
            }
            return Json(new { result = true, message = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}
