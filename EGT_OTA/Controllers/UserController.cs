using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// 第三方登录
        /// </summary>
        public ActionResult LoginThird()
        {
            try
            {
                var NickName = SqlFilter(ZNRequest.GetString("NickName").Trim());
                var avatar = ZNRequest.GetString("Avatar");
                var openID = ZNRequest.GetString("OpenID");
                var source = ZNRequest.GetInt("Source");

                User user = null;
                if (string.IsNullOrWhiteSpace(openID))
                {
                    openID = BuildNumber();
                }
                else
                {
                    switch (source)
                    {
                        case 1:
                            user = db.Single<User>(x => x.WeiXin == openID);
                            break;
                        case 2:
                            user = db.Single<User>(x => x.QQ == openID);
                            break;
                        case 3:
                            user = db.Single<User>(x => x.Weibo == openID);
                            break;
                        default:
                            break;
                    }

                }
                if (user == null)
                {
                    user = new User();
                    user.ClientID = ZNRequest.GetString("ClientID");
                    user.Province = ZNRequest.GetString("Province");
                    user.City = ZNRequest.GetString("City");
                    user.District = ZNRequest.GetString("District");
                    user.Street = ZNRequest.GetString("Street");
                    user.DetailName = ZNRequest.GetString("DetailName");
                    user.CityCode = ZNRequest.GetString("CityCode");
                    user.Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                    user.Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                    user.Password = string.Empty;
                    user.NickName = NickName;
                    user.Sex = ZNRequest.GetInt("Sex", Enum_Sex.Boy);
                    user.Cover = SqlFilter(ZNRequest.GetString("Cover"));
                    if (string.IsNullOrWhiteSpace(user.Cover))
                    {
                        user.Cover = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"].ToString() + "Images/User/cover01.png";
                    }
                    user.Email = string.Empty;
                    user.IsEmail = 0;
                    user.Signature = string.Empty;
                    user.Avatar = avatar;
                    if (string.IsNullOrWhiteSpace(user.Avatar))
                    {
                        user.Avatar = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"].ToString() + "Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg";
                    }
                    user.Phone = string.Empty;
                    user.WeiXin = string.Empty;
                    user.QQ = string.Empty;
                    user.Weibo = string.Empty;

                    switch (source)
                    {
                        case 1:
                            user.WeiXin = openID;
                            break;
                        case 2:
                            user.QQ = openID;
                            break;
                        case 3:
                            user.Weibo = openID;
                            break;
                        default:
                            break;
                    }

                    user.LoginTimes = 1;
                    user.CreateDate = DateTime.Now;
                    user.LastLoginDate = DateTime.Now;
                    user.LastLoginIP = Tools.GetClientIP;
                    user.Keeps = 0;
                    user.Follows = 0;
                    user.Fans = 0;
                    user.Articles = 0;
                    user.Comments = 0;
                    user.Zans = 0;
                    user.ShowArticle = 1;
                    user.ShowFollow = 1;
                    user.ShowFan = 1;
                    user.ShowPush = 1;
                    user.ShowPosition = 1;
                    user.Birthday = DateTime.Now;
                    user.Number = BuildNumber();
                    user.IsPay = 1;
                    user.ID = Tools.SafeInt(db.Add<User>(user), 0);
                    if (user.ID > 0)
                    {
                        user.Address = user.Province + " " + user.City;
                        user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");

                        return Json(new { result = true, message = user }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    user.ClientID = ZNRequest.GetString("ClientID");
                    user.Province = ZNRequest.GetString("Province");
                    user.City = ZNRequest.GetString("City");
                    user.District = ZNRequest.GetString("District");
                    user.Street = ZNRequest.GetString("Street");
                    user.DetailName = ZNRequest.GetString("DetailName");
                    user.CityCode = ZNRequest.GetString("CityCode");
                    user.Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                    user.Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                    user.LoginTimes += 1;
                    user.LastLoginDate = DateTime.Now;
                    user.LastLoginIP = Tools.GetClientIP;
                    var result = db.Update<User>(user) > 0;
                    if (result)
                    {
                        return Json(new { result = true, message = UserInfo(user) }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_LoginThird" + ex.Message, ex);
            }
            return Json(new { result = false, message = "登录失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登录
        /// </summary>
        public ActionResult Login()
        {
            try
            {
                var phone = ZNRequest.GetString("Phone").Trim();
                var password = ZNRequest.GetString("Password").Trim();
                if (String.IsNullOrWhiteSpace(phone) || String.IsNullOrWhiteSpace(password))
                {
                    return Json(new { result = false, message = "手机号码和密码不能为空" }, JsonRequestBehavior.AllowGet);
                }
                password = DesEncryptHelper.Encrypt(password);
                User user = db.Single<User>(x => x.Phone == phone && x.Password == password);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户名或密码错误" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string info = "\r\n" + user.Phone + "于" + DateTime.Now.ToString() + "登录APP\r\n" + "登录IP为:" + Tools.GetClientIP;
                    LogHelper.UserLoger.Info(info);

                    user.ClientID = ZNRequest.GetString("ClientID");
                    user.LoginTimes += 1;
                    user.LastLoginDate = DateTime.Now;
                    user.LastLoginIP = Tools.GetClientIP;
                    var result = db.Update<User>(user) > 0;
                    if (result)
                    {
                        return Json(new { result = true, message = UserInfo(user) }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Login" + ex.Message, ex);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 注册
        /// </summary>
        public ActionResult Register()
        {
            var result = string.Empty;
            try
            {
                var phone = ZNRequest.GetString("Phone");
                var password = ZNRequest.GetString("Password");
                var code = ZNRequest.GetString("Code");
                var sms = ZNRequest.GetString("SMS");
                if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { result = false, message = "手机号码和密码不能为空" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(code))
                {
                    return Json(new { result = false, message = "验证码不能为空" }, JsonRequestBehavior.AllowGet);
                }

                //var value = CookieHelper.GetCookieValue("SMS");
                //if (value != phone + sms + code)
                //{
                //    return Json(new { result = false, message = "验证码不正确" }, JsonRequestBehavior.AllowGet);
                //}

                if (db.Exists<User>(x => x.Phone == phone))
                {
                    return Json(new { result = false, message = "当前账号已注册" }, JsonRequestBehavior.AllowGet);
                }
                User user = new User();
                user.NickName = SqlFilter(ZNRequest.GetString("NickName"));
                if (string.IsNullOrWhiteSpace(user.NickName))
                {
                    return Json(new { result = false, message = "昵称不能为空" }, JsonRequestBehavior.AllowGet);
                }
                if (HasDirtyWord(user.NickName))
                {
                    return Json(new { result = false, message = "您输入的内容含有敏感内容，请检查后重试哦" }, JsonRequestBehavior.AllowGet);
                }
                user.Password = DesEncryptHelper.Encrypt(password);
                user.Sex = ZNRequest.GetInt("Sex", Enum_Sex.Boy);
                user.Cover = SqlFilter(ZNRequest.GetString("Cover"));
                if (string.IsNullOrWhiteSpace(user.Cover))
                {
                    user.Cover = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"].ToString() + "Images/User/cover01.png";
                }
                user.Province = ZNRequest.GetString("Province");
                user.City = ZNRequest.GetString("City");
                user.District = ZNRequest.GetString("District");
                user.Street = ZNRequest.GetString("Street");
                user.DetailName = ZNRequest.GetString("DetailName");
                user.CityCode = ZNRequest.GetString("CityCode");
                user.Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                user.Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                user.Email = string.Empty;
                user.IsEmail = 0;
                user.Signature = string.Empty;
                user.Avatar = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"].ToString() + "Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg";
                user.Phone = phone;
                user.WeiXin = string.Empty;
                user.QQ = string.Empty;
                user.Weibo = string.Empty;
                user.LoginTimes = 1;
                user.CreateDate = DateTime.Now;
                user.LastLoginDate = DateTime.Now;
                user.LastLoginIP = Tools.GetClientIP;
                user.Keeps = 0;
                user.Follows = 0;
                user.Fans = 0;
                user.ShowArticle = 1;
                user.ShowFollow = 1;
                user.ShowFan = 1;
                user.ShowPush = 1;
                user.ShowPosition = 1;
                user.Status = Enum_Status.Approved;
                user.Number = BuildNumber();
                user.IsPay = 1;
                user.ClientID = ZNRequest.GetString("ClientID");
                user.ID = Tools.SafeInt(db.Add<User>(user), 0);
                if (user.ID > 0)
                {
                    user.Address = user.Province + " " + user.City;
                    user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");

                    CookieHelper.ClearCookie("SMS");

                    return Json(new { result = true, message = user }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Register" + ex.Message, ex);
                result = ex.Message;
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        public ActionResult EditAvatar()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var avatar = ZNRequest.GetString("Avatar").Trim();
                if (string.IsNullOrEmpty(avatar))
                {
                    return Json(new { result = false, message = "请上传头像" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Avatar").EqualTo(avatar).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditAvatar" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改封面
        /// </summary>
        public ActionResult EditCover()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var cover = ZNRequest.GetString("Cover").Trim();
                if (string.IsNullOrEmpty(cover))
                {
                    return Json(new { result = false, message = "请上传背景图片" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Cover").EqualTo(cover).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditCover" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改地址
        /// </summary>
        public ActionResult EditAddress()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                user.Province = ZNRequest.GetString("Province");
                user.City = ZNRequest.GetString("City");
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditAddress" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改性别
        /// </summary>
        public ActionResult EditSex()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Sex").EqualTo(ZNRequest.GetInt("Sex")).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditSex" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改启用水印
        /// </summary>
        public ActionResult EditUseDraw()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("UseDraw").EqualTo(ZNRequest.GetInt("UseDraw")).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditUseDraw" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改打赏
        /// </summary>
        public ActionResult EditPay()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("IsPay").EqualTo(ZNRequest.GetInt("IsPay")).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditPay" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改生日
        /// </summary>
        public ActionResult EditBirthday()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Birthday").EqualTo(ZNRequest.GetDateTime("Birthday")).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditBirthday" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改昵称
        /// </summary>
        public ActionResult EditNickName()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var NickName = SqlFilter(ZNRequest.GetString("NickName").Trim());
                if (string.IsNullOrEmpty(NickName))
                {
                    return Json(new { result = false, message = "请填写昵称信息" }, JsonRequestBehavior.AllowGet);
                }
                if (HasDirtyWord(NickName))
                {
                    return Json(new { result = false, message = "您输入的昵称含有敏感内容，请检查后重试哦" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("NickName").EqualTo(NickName).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditNickName" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改签名
        /// </summary>
        public ActionResult EditSignature()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var Signature = SqlFilter(ZNRequest.GetString("Signature").Trim());
                if (!string.IsNullOrWhiteSpace(Signature))
                {
                    Signature = CutString(Signature, 200);
                    if (HasDirtyWord(Signature))
                    {
                        return Json(new { result = false, message = "您输入的签名含有敏感内容，请检查后重试哦" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Signature = "";
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Signature").EqualTo(Signature).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditSignature" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public ActionResult EditPassword()
        {
            try
            {
                var mobile = ZNRequest.GetString("Mobile");
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var user = db.Single<User>(x => x.Phone == mobile);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var newpassword = ZNRequest.GetString("NewPassword").Trim();
                if (string.IsNullOrWhiteSpace(newpassword))
                {
                    return Json(new { result = false, message = "请填写新密码" }, JsonRequestBehavior.AllowGet);
                }
                newpassword = DesEncryptHelper.Encrypt(newpassword);
                if (user.Password == newpassword)
                {
                    return Json(new { result = false, message = "新密码与原密码相同" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Password").EqualTo(newpassword).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = newpassword }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditPassword" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改是否分享昵称
        /// </summary>
        public ActionResult EditShareNick()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("ShareNick").EqualTo(ZNRequest.GetInt("ShareNick")).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditShareNick" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改是否自动播放音乐
        /// </summary>
        public ActionResult EditAutoMusic()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("AutoMusic").EqualTo(ZNRequest.GetInt("AutoMusic")).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditAutoMusic" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 邮箱认证
        /// </summary>
        public ActionResult EmailVerify()
        {
            User user = GetUserInfo();
            if (user == null)
            {
                return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
            }

            var result = false;
            var message = string.Empty;
            try
            {
                //判断是否已验证
                if (user.IsEmail == 1)
                {
                    return Json(new { result = result, message = "邮箱已认证" }, JsonRequestBehavior.AllowGet);
                }
                var email = ZNRequest.GetString("email");
                if (String.IsNullOrEmpty(email))
                {
                    return Json(new { result = result, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                //判断是否存在邮箱账号
                if (db.Exists<User>(x => x.Email == email && x.ID != user.ID))
                {
                    return Json(new { result = result, message = "该邮箱已被绑定" }, JsonRequestBehavior.AllowGet);
                }
                var code = Guid.NewGuid().ToString("N");
                CookieHelper.SetCookie("email" + user.ID, code);

                var url = "http://localhost/app/User/CheckeEmail?uid=" + user.ID + "&code=" + code;
                string body = @"<strong>这是发给您的邮箱认证的邮件，有效期24小时</strong><p>此为系统邮件，请勿直接回复此邮件。</p> <br />
                                请点击下面的链接完成邮箱验证，如果链接无法转向，请复制一下链接到浏览器的地址栏中直接访问。 <br />
                               <a href='" + url + "' target='_blank'>请点击此处链接</a> <br />如果链接无法转向，请复制此连接" + url + "到浏览器的地址栏中直接访问<br />";
                //FromUserModel fromUserModel = new FromUserModel
                //{
                //    UserID = "kangcy@axon.com.cn",
                //    UserPwd = "YXhvbjEyMzQ=",
                //    UserName = "少侠网",
                //    ToUserArray = new ToUserModel[] { new ToUserModel { UserID = email, UserName = user.NickName } }
                //};
                //MailHelper.SendMail("少侠网", body, fromUserModel);
                user.Email = email;
                result = db.Update<User>(user) > 0;
                return Json(new { result = result, message = "发送邮箱验证成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EmailVerify:" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验邮箱认证
        /// </summary>
        public ActionResult CheckEmail()
        {
            User user = GetUserInfo();
            if (user == null)
            {
                return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
            }

            var result = false;
            var message = string.Empty;
            try
            {
                var uid = ZNRequest.GetInt("uid");
                var code = ZNRequest.GetString("code");
                var cookie = CookieHelper.GetCookieValue("email" + uid);
                if (code == cookie)
                {
                    if (user.IsEmail == 0)
                    {
                        user.IsEmail = 1;
                        result = db.Update<User>(user) > 0;
                        message = "邮箱验证成功！";
                    }
                    else
                    {
                        message = "邮箱已经验证,请勿重复验证";
                    }
                }
                else
                {
                    message = "邮箱验证失败！<br />可能原因如下：<br />1、验证码过期<br />2、点击连接时网络连接失败<br />请重新发送验证请求";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_CheckEmail" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        public ActionResult Detail()
        {
            try
            {
                var Number = ZNRequest.GetString("Number");//当前查询用户
                var CurrNumber = ZNRequest.GetString("CurrNumber");//当前用户
                if (string.IsNullOrWhiteSpace(Number) || string.IsNullOrWhiteSpace(CurrNumber))
                {
                    return Json(new { result = false, message = "参数信息异常" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Number == Number);
                if (user == null)
                {
                    return Json(new { result = false, message = "用戶信息异常" }, JsonRequestBehavior.AllowGet);
                }

                //判断黑名单
                if (db.Exists<Black>(x => x.ToUserNumber == CurrNumber && x.CreateUserNumber == Number))
                {
                    return Json(new { result = false, message = "没有访问权限" }, JsonRequestBehavior.AllowGet);
                }

                var newuser = UserInfo(user);

                newuser.IsFan = db.Exists<Fan>(x => x.CreateUserNumber == CurrNumber && x.ToUserNumber == Number) ? 1 : 0;
                newuser.IsBlack = db.Exists<Black>(x => x.CreateUserNumber == CurrNumber && x.ToUserNumber == Number) ? 1 : 0;

                return Json(new { result = true, message = newuser }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Detail" + ex.Message, ex);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        public ActionResult Info()
        {
            try
            {
                var Number = ZNRequest.GetString("Number");//当前查询用户
                if (string.IsNullOrWhiteSpace(Number))
                {
                    return Json(new { result = false, message = "参数信息异常" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Number == Number);
                if (user == null)
                {
                    return Json(new { result = false, message = "用戶信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var newuser = UserInfo(user);
                return Json(new { result = true, message = newuser }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Info" + ex.Message, ex);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult All()
        {
            try
            {
                var pager = new Pager();
                var nickname = ZNRequest.GetString("NickName");
                var query = new SubSonic.Query.Select(provider).From<User>().Where<User>(x => x.Status == Enum_Status.Approved);
                if (!string.IsNullOrWhiteSpace(nickname))
                {
                    query.And("NickName").IsNotNull().And("NickName").Like("%" + nickname + "%");
                }

                //搜索默认显示推荐文章
                var Source = ZNRequest.GetString("Source");
                if (!string.IsNullOrWhiteSpace(Source))
                {
                    query = query.And("NickName").IsNotNull().And("Signature").IsNotNull().And("Avatar").IsNotNull();
                }

                //过滤黑名单
                var Number = ZNRequest.GetString("Number");
                if (!string.IsNullOrWhiteSpace(Number))
                {
                    var black = db.Find<Black>(x => x.CreateUserNumber == Number);
                    if (black.Count > 0)
                    {
                        var userids = black.Select(x => x.ToUserNumber).ToArray();
                        query = query.And("Number").NotIn(userids);
                    }
                }

                var list = new List<User>();
                if (string.IsNullOrWhiteSpace(Source))
                {
                    list = query.OrderDesc("ID").ExecuteTypedList<User>();
                }
                else
                {
                    list = query.Paged(1, 100).OrderDesc("ID").ExecuteTypedList<User>();
                }

                var follows = db.Find<Fan>(x => x.CreateUserNumber == Number).ToList();

                var recordCount = list.Count;
                var totalPage = 1;
                var newlist = (from l in list
                               select new
                               {
                                   ID = l.ID,
                                   NickName = l.NickName,
                                   Signature = l.Signature,
                                   Avatar = l.Avatar,
                                   Cover = l.Cover,
                                   Number = l.Number,
                                   IsFollow = follows.Exists(x => x.ToUserNumber == l.Number) ? 1 : 0
                               }).ToList();
                var result = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_All" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 绑定号码
        /// </summary>
        public ActionResult BindPhone()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var phone = ZNRequest.GetString("Phone");
                if (string.IsNullOrEmpty(phone))
                {
                    return Json(new { result = false, message = "手机号码不能为空" }, JsonRequestBehavior.AllowGet);
                }
                var code = ZNRequest.GetString("Code");
                if (string.IsNullOrWhiteSpace(code))
                {
                    return Json(new { result = false, message = "验证码不能为空" }, JsonRequestBehavior.AllowGet);
                }

                var value = CookieHelper.GetCookieValue("Validate");

                if (value.ToLower() != code.ToLower())
                {
                    return Json(new { result = false, message = "验证码不正确" }, JsonRequestBehavior.AllowGet);
                }

                if (db.Exists<User>(x => x.Phone == phone && x.ID != user.ID))
                {
                    return Json(new { result = false, message = "该手机号码已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Phone").EqualTo(phone).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    CookieHelper.ClearCookie("Validate");

                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindPhone" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 绑定微信
        /// </summary>
        public ActionResult BindWeixin()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var key = ZNRequest.GetString("Key");
                if (db.Exists<User>(x => x.WeiXin == key && x.ID != user.ID))
                {
                    return Json(new { result = false, message = "该微信账号已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("WeiXin").EqualTo(key).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindWeixin" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 绑定微博
        /// </summary>
        public ActionResult BindWeibo()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var key = ZNRequest.GetString("Key");
                if (db.Exists<User>(x => x.Weibo == key && x.ID != user.ID))
                {
                    return Json(new { result = false, message = "该微博账号已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Weibo").EqualTo(key).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindWeibo" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 绑定QQ
        /// </summary>
        public ActionResult BindQQ()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var key = ZNRequest.GetString("Key");
                if (db.Exists<User>(x => x.QQ == key && x.ID != user.ID))
                {
                    return Json(new { result = false, message = "账号已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("QQ").EqualTo(key).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindQQ" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        public ActionResult UnBind()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var source = ZNRequest.GetInt("Source");
                if (string.IsNullOrWhiteSpace(user.Phone))
                {
                    var name = string.Empty;
                    switch (source)
                    {
                        case 1:
                            name = "微信";
                            break;
                        case 2:
                            name = "QQ";
                            break;
                        case 3:
                            name = "微博";
                            break;
                        default:
                            break;
                    }
                    return Json(new { result = false, message = "解绑" + name + "前需要绑定手机" }, JsonRequestBehavior.AllowGet);
                }
                var result = false;
                switch (source)
                {
                    case 1:
                        result = new SubSonic.Query.Update<User>(provider).Set("WeiXin").EqualTo(string.Empty).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    case 2:
                        result = new SubSonic.Query.Update<User>(provider).Set("QQ").EqualTo(string.Empty).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    case 3:
                        result = new SubSonic.Query.Update<User>(provider).Set("Weibo").EqualTo(string.Empty).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    default:
                        break;
                }
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_UnBind" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        public class PersonCompar : System.Collections.Generic.IEqualityComparer<ArticlePart>
        {
            public bool Equals(ArticlePart x, ArticlePart y)
            {
                if (x == null)
                    return y == null;
                return x.Introduction == y.Introduction;
            }
            public int GetHashCode(ArticlePart obj)
            {
                if (obj == null)
                    return 0;
                return obj.ID.GetHashCode();
            }
        }

        /// <summary>
        /// 相册
        /// </summary>
        public ActionResult Pic()
        {
            try
            {
                var Number = ZNRequest.GetString("Number");
                var UserNumber = ZNRequest.GetString("UserNumber");
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic && x.Status != Enum_Status.DELETE);
                if (Number != UserNumber)
                {
                    query = query.And("Status").IsEqualTo(Enum_Status.Approved);
                }
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    return Json(new
                    {
                        currpage = pager.Index,
                        records = 0,
                        totalpage = 1,
                        list = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                }

                query = query.And("CreateUserNumber").IsEqualTo(UserNumber);

                var recordCount = query.GetRecordCount();
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<ArticlePart>();


                //var list = query.ExecuteTypedList<ArticlePart>();
                //var newlist = new List<ArticlePart>();
                //list.GroupBy(x => x.Introduction).ToList().ForEach(x =>
                //{
                //    newlist.Add(x.FirstOrDefault());
                //});
                //newlist = newlist.OrderByDescending(x => x.ID).ToList();

                //var recordCount = newlist.Count;
                //var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                //newlist = newlist.Skip((pager.Index - 1) * pager.Size).Take(pager.Size).ToList();


                var result = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Pic" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 相册图片删除
        /// </summary>
        public ActionResult PicDelete()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var ids = ZNRequest.GetString("PartID");
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var id = ids.Split(',').ToList();
                id.ForEach(x =>
                {
                    var partid = Tools.SafeInt(x);
                    var result = new SubSonic.Query.Update<ArticlePart>(provider).Set("Status").EqualTo(Enum_Status.DELETE).Where<ArticlePart>(y => y.ID == partid && y.CreateUserNumber == user.Number).Execute() > 0;
                });
                return Json(new { result = true, message = "刪除成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_PicDelete:" + ex.Message);
            }
            return Json(new { result = false, message = "刪除失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 隐私管理
        /// </summary>
        public ActionResult EditSecret()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var target = ZNRequest.GetString("Name");
                var show = ZNRequest.GetInt("Show");
                var result = false;
                switch (target)
                {
                    //显示我喜欢的文章
                    case "ShowArticle":
                        result = new SubSonic.Query.Update<User>(provider).Set("ShowArticle").EqualTo(show).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    //显示我的关注
                    case "ShowFollow":
                        result = new SubSonic.Query.Update<User>(provider).Set("ShowFollow").EqualTo(show).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    //显示我的粉丝
                    case "ShowFan":
                        result = new SubSonic.Query.Update<User>(provider).Set("ShowFan").EqualTo(show).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    //显示消息推送
                    case "ShowPush":
                        result = new SubSonic.Query.Update<User>(provider).Set("ShowPush").EqualTo(show).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    //显示我的定位
                    case "ShowPosition":
                        result = new SubSonic.Query.Update<User>(provider).Set("ShowPosition").EqualTo(show).Where<User>(x => x.ID == user.ID).Execute() > 0;
                        break;
                    default:
                        break;
                }
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditSecret" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 用户文章
        /// </summary>
        public ActionResult Article()
        {
            try
            {
                var UserNumber = ZNRequest.GetString("UserNumber");
                var CurrUserNumber = ZNRequest.GetString("CurrUserNumber");
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider, "ID", "Number", "CreateUserNumber", "Cover", "ArticlePower", "Status", "CreateDate").From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved && x.CreateUserNumber == UserNumber);


                if (UserNumber != CurrUserNumber)
                {
                    query = query.And("ArticlePower").IsEqualTo(Enum_ArticlePower.Public);
                }

                var list = query.OrderDesc(new string[] { "ID" }).ExecuteTypedList<Article>();

                var recordCount = list.Count();
                if (recordCount == 0)
                {
                    return Json(new
                    {
                        records = 0,
                        list = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                }

                List<UserArticleJson> newlist = new List<UserArticleJson>();
                list.GroupBy(x => x.CreateDate.ToString("yyyyMM")).ToList().ForEach(x =>
                {
                    UserArticleJson model = new UserArticleJson();
                    model.List = new List<UserArticleSubJson>();
                    var items = list.FindAll(y => y.CreateDate.ToString("yyyyMM") == x.Key);
                    items.ForEach(y =>
                    {
                        UserArticleSubJson item = new UserArticleSubJson();
                        item.ID = y.ID;
                        item.Number = y.Number;
                        item.CreateUserNumber = y.CreateUserNumber;
                        item.Cover = y.Cover;
                        item.ArticlePower = y.ArticlePower;
                        model.List.Add(item);
                    });
                    model.CreateDate = items[0].CreateDate.Year + "年" + items[0].CreateDate.Month + "月";
                    model.Count = items.Count;
                    newlist.Add(model);
                });
                return Json(new
                {
                    records = newlist.Sum(x => x.Count),
                    list = newlist
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Article:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 同城用户
        /// </summary>
        public ActionResult CityAll()
        {
            try
            {
                var number = ZNRequest.GetString("Number");
                var CityCode = ZNRequest.GetString("CityCode");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(CityCode))
                {
                    return Json(new { result = false, message = "定位失败" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Number == number);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<User>().Where<User>(x => x.Status == Enum_Status.Approved && x.Latitude > 0 && x.Longitude > 0 && x.CityCode == CityCode && x.ID != user.ID);

                //过滤黑名单
                var black = db.Find<Black>(x => x.CreateUserNumber == user.Number);
                if (black.Count > 0)
                {
                    var userids = black.Select(x => x.ToUserNumber).ToArray();
                    query = query.And("Number").NotIn(userids);
                }

                var list = query.ExecuteTypedList<User>();
                var recordCount = list.Count;
                list.ForEach(x =>
                {
                    x.Distance = DistanceHelper.GetDistance(user.Latitude, user.Longitude, x.Latitude, x.Longitude);
                });

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                list = list.OrderBy(x => x.Distance).Skip((pager.Index - 1) * pager.Size).Take(pager.Size).ToList();

                var follows = db.Find<Fan>(x => x.CreateUserNumber == number).ToList();

                var newlist = (from l in list
                               select new
                               {
                                   ID = l.ID,
                                   NickName = l.NickName,
                                   Signature = l.Signature,
                                   Avatar = l.Avatar,
                                   Number = l.Number,
                                   Distance = l.Distance,
                                   IsFollow = follows.Exists(x => x.ToUserNumber == l.Number) ? 1 : 0
                               }).ToList();
                var result = new
                {
                    result = true,
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_CityAll" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 推荐用户
        /// </summary>
        public ActionResult RecommendAll()
        {
            try
            {
                var number = ZNRequest.GetString("Number");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Number == number);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<User>().Where<User>(x => x.Status == Enum_Status.Approved && x.IsRecommend == 1 && x.ID != user.ID);

                //过滤黑名单
                var black = db.Find<Black>(x => x.CreateUserNumber == user.Number);
                if (black.Count > 0)
                {
                    var userids = black.Select(x => x.ToUserNumber).ToArray();
                    query = query.And("Number").NotIn(userids);
                }

                var list = query.ExecuteTypedList<User>();
                var recordCount = list.Count;
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                list = list.OrderBy(x => x.Distance).Skip((pager.Index - 1) * pager.Size).Take(pager.Size).ToList();

                var follows = db.Find<Fan>(x => x.CreateUserNumber == number).ToList();

                var newlist = (from l in list
                               select new
                               {
                                   ID = l.ID,
                                   NickName = l.NickName,
                                   Signature = l.Signature,
                                   Cover = l.Cover,
                                   Avatar = l.Avatar,
                                   Number = l.Number,
                                   Distance = l.Distance,
                                   IsFollow = follows.Exists(x => x.ToUserNumber == l.Number) ? 1 : 0
                               }).ToList();
                var result = new
                {
                    result = true,
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_RecommendAll" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        protected User UserInfo(User user)
        {
            user.Address = user.Province + " " + user.City;
            user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");

            var order = db.Find<Order>(x => x.ToUserNumber == user.Number && x.Status == Enum_Status.Approved);
            user.Pays = order.Count;

            //打赏金额
            var orderMoney = order.Sum(x => x.Price);

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
                user.Money = user.Money;
            }

            //关注
            user.Follows = new SubSonic.Query.Select(provider, "ID").From<Fan>().Where<Fan>(x => x.CreateUserNumber == user.Number).GetRecordCount();

            //粉丝
            user.Fans = new SubSonic.Query.Select(provider, "ID").From<Fan>().Where<Fan>(x => x.ToUserNumber == user.Number).GetRecordCount();

            //我的
            user.Articles = new SubSonic.Query.Select(provider, "ID").From<Article>().Where<Article>(x => x.CreateUserNumber == user.Number && x.Status != Enum_Status.DELETE).GetRecordCount();

            //评论
            user.Comments = new SubSonic.Query.Select(provider, "ID").From<Comment>().Where<Comment>(x => x.CreateUserNumber == user.Number).GetRecordCount();

            //点赞
            user.Zans = new SubSonic.Query.Select(provider, "ID").From<ArticleZan>().Where<ArticleZan>(x => x.ArticleUserNumber == user.Number).GetRecordCount();

            //我收藏的文章
            user.Keeps = new SubSonic.Query.Select(provider, "ID").From<Keep>().Where<Keep>(x => x.CreateUserNumber == user.Number).GetRecordCount();

            user.ShareUrl = System.Configuration.ConfigurationManager.AppSettings["share_url"] + "u/" + user.Number;
            return user;
        }
    }
}
