using CommonTools;
using EGT_OTA.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 微信公众平台
    /// </summary>
    public class WeixinController : Controller
    {
        /// <summary>
        /// 微信认证
        /// 加密/校验流程如下
        /// 1. 将token、timestamp、nonce三个参数进行字典序排序
        /// 2. 将三个参数字符串拼接成一个字符串进行sha1加密
        /// 3. 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信
        /// </summary>
        [HttpGet]
        public ActionResult Check()
        {
            string signature = ZNRequest.GetString("signature");
            string timestamp = ZNRequest.GetString("timestamp");
            string nonce = ZNRequest.GetString("nonce");
            string echostr = ZNRequest.GetString("echostr");
            String[] arr = new String[] { WeixinHelper.Token, timestamp, nonce };
            Array.Sort(arr);
            StringBuilder content = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                content.Append(arr[i]);
            }
            //加密并返回验证结果  
            var result = string.IsNullOrWhiteSpace(signature) ? false : signature.ToUpper().Equals(WeixinHelper.SHA1(content.ToString()));
            return Content(result ? echostr : "");
        }

        public ActionResult Home()
        {
            var openid = ZNRequest.GetString("openid");
            var nickname = ZNRequest.GetString("nickname");//用户昵称
            var avatar = ZNRequest.GetString("headimgurl");//用户头像

            //判断微信浏览器跳转授权
            //string userAgent = Request.UserAgent;
            //if (userAgent.ToLower().Contains("micromessenger") && string.IsNullOrWhiteSpace(openid) &&
            //    string.IsNullOrWhiteSpace(nickname) && string.IsNullOrWhiteSpace(avatar))
            //{
            //    var url = System.Web.HttpContext.Current.Request.Url.ToString();
            //    url = url.Replace("&", "godlike");

            //    var redirectUrl1 = "wx974a5cbcb4a85bc7";
            //    var redirectUrl2 = "http://wx.51pinzhi.cn/wx_new/api/WeixinApi/GetPrivilege";
            //    var redirectUrl3 = "iwx_wxd944a4e7e4fbb864";
            //    var redirectUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + redirectUrl1 + "&redirect_uri=" + redirectUrl2 + "?backurl=" + url + "&response_type=code&scope=snsapi_userinfo&state=" + redirectUrl3 + "#wechat_redirect";
            //    ViewBag.Url = redirectUrl;
            //    Response.Redirect(redirectUrl);
            //}

            var url2 = System.Web.HttpContext.Current.Server.UrlEncode("http://www.baidu.com");
            //https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx3b2ef7c18241c20f&redirect_uri=http%3a%2f%2fwww.baidu.com&response_type=code&scope=snsapi_userinfo&state=123456&connect_redirect=1#wechat_redirect
            return Json(true);
        }
    }
}
