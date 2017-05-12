using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 邮件发送
    /// </summary>
    public static class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sub">邮件主题</param>
        /// <param name="message">邮件内容</param>
        /// <param name="mailInfo">发送人实体类</param>
        public static void SendMail(string sub, string message, MailInfo mailInfo)
        {
            SmtpClient smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false,
                Host = mailInfo.MailHost,
                Port = 0x19,
                Credentials = new NetworkCredential(mailInfo.UserID, CoderMaker.Decode(mailInfo.UserPwd))
            };
            MailMessage mm = new MailMessage
            {
                Priority = MailPriority.High,
                From = new MailAddress(mailInfo.UserID, mailInfo.UserName, Encoding.GetEncoding(0x3a8))
            };
            for (int i = 0; i < mailInfo.ToUser.Length; i++)
            {
                try
                {
                    mm.To.Add(new MailAddress(mailInfo.ToUser[i].UserID, mailInfo.ToUser[i].UserName, Encoding.GetEncoding(936)));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            mm.Subject = sub;
            mm.SubjectEncoding = Encoding.GetEncoding(0x3a8);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = Encoding.GetEncoding(0x3a8);
            mm.Body = message;
            try
            {
                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region  方法调用

        //Json配置文件
        //"MailInfo": {
        //    "MailHost": "smtp.qiye.163.com",
        //    "IsSendEmail": true,
        //    "ToUser": [ { "UserID": "xusx@axon.com.cn", "UserName": "徐申兴" }, { "UserID": "kangcy@axon.com.cn", "UserName": "康春阳" } ],
        //    "UserID": "xusx@axon.com.cn",
        //    "UserPwd": "eHVzaGVuKzI2MA==",
        //    "UserName": "徐申兴"
        //}

        /// <summary>
        /// 发送邮件信息
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="msg"></param>
        public static void SendMailInfo(string sub, string msg)
        {
            try
            {
                MailInfo mailInfo = new MailInfo();
                //mailInfo = CommonConfig.Instance.Model().MailInfo;
                new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        MailHelper.SendMail(sub, msg, mailInfo);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.ErrorLoger.Error("SendMail1:" + ex.Message);
                    }
                })
                ).Start();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SendMail2:" + ex.Message);
            }
        }

        #endregion
    }

    /// <summary>
    /// 邮件发送类主体
    /// </summary>
    public class MailInfo
    {
        /// <summary>
        /// 发件人邮箱
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string UserPwd { get; set; }

        /// <summary>
        /// 发件人
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 是否邮件提醒
        /// </summary>
        public bool IsSendEmail { get; set; }

        /// <summary>
        /// 邮件人服务器
        /// </summary>
        public string MailHost { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public ToUser[] ToUser { get; set; }
    }

    /// <summary>
    /// 收件人主体
    /// </summary>
    public class ToUser
    {
        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 收件人名
        /// </summary>
        public string UserName { get; set; }
    }

    public static class CoderMaker
    {
        public static string Encode(string data)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public static string Decode(string data)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
    }
}
