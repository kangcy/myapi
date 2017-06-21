using CommonTools;
using EGT_OTA.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EGT_OTA.Helper
{
    public class WeixinHelper
    {
        public const string AppID = "wx3b2ef7c18241c20f";
        public const string AppSecret = "bf1247d67a635d2bfe5822a8c525fb1d";
        public const string Token = "kangcy4373686";
        public const string EncodingAESKey = "11DrT4US9XTR4g5NdyQ9dtwYCFSgUbkJUOoBpRPNUVK";
        public const string NonceStr = "xiaoweipian";

        /// <summary>
        /// 获取access_token
        /// {"access_token":"MRL4eEI1dj3PicadOCwpWFrxClmgnb_LbUZpUSKOjdlAbJg997W_J_UCGcJVKWOm7t0H04zwayNxMAnmLPoySPeb3UGDxXzrbSgNlQZBgBQMx9vwZKJfFuwASMr7MuAqMZJfAJAQDN","expires_in":7200}
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            var access_token = "";
            try
            {
                access_token = Tools.SafeString(CacheHelper.GetCache("Token"));
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", AppID, AppSecret);
                    var result = HttpUtil.Get(url);
                    var token = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(result);
                    access_token = token.access_token;
                    CacheHelper.Insert("Token", token.access_token, TimeSpan.FromSeconds(token.expires_in));
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("GetToken:" + ex.Message);
            }
            return access_token;
        }

        /// <summary>
        /// 获取jsapi_ticket
        /// {"errcode":0,"errmsg":"ok","ticket":"bxLdikRXVbTPdHSM05e5u_ZbptsYyoMw1ktvUeFohdruNx6xoHhqzuruhJBeMuXn2w_5-0aisJg6pN__L7_RGA","expires_in":7200}
        /// </summary>
        /// <returns></returns>
        public static string GetTicket()
        {
            var ticket = "";
            try
            {
                string access_token = GetToken();
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi";
                var result = HttpUtil.Get(url);
                JObject json = JObject.Parse(result);
                ticket = Tools.SafeString(json["ticket"]);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("GetTicket:" + ex.Message);
            }
            return ticket;
        }

        /// <summary>
        /// 获取微信分享签名
        /// </summary>
        public static string GetSignature(string url, string timestamp)
        {
            string signature = "";
            try
            {
                var jsapi_ticket = GetTicket();
                var str = "jsapi_ticket=" + jsapi_ticket + "&noncestr=" + NonceStr + "&timestamp=" + timestamp + "&url=" + url;
                signature = SHA1(str);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("GetSignature:" + ex.Message);
            }
            return signature;
        }

        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <returns>返回40位UTF8 大写</returns>  
        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }
        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <param name="encode">指定加密编码</param>  
        /// <returns>返回40位大写字符串</returns>  
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }
    }
}