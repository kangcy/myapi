using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using CommonTools;
using EGT_OTA.Controllers.Filter;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace EGT_OTA.Controllers.Api
{
    public class WeixinController : BaseApiController
    {
        public const string Token = "kangcy4373686";

        public const string EncodingAESKey = "11DrT4US9XTR4g5NdyQ9dtwYCFSgUbkJUOoBpRPNUVK";

        /// <summary>
        /// 微信认证
        /// 加密/校验流程如下
        /// 1. 将token、timestamp、nonce三个参数进行字典序排序
        /// 2. 将三个参数字符串拼接成一个字符串进行sha1加密
        /// 3. 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信
        /// </summary>
        [HttpGet]
        [Route("Api/Weixin/Check")]
        public string CheckSignature()
        {
            string signature = ZNRequest.GetString("signature");
            string timestamp = ZNRequest.GetString("timestamp");
            string nonce = ZNRequest.GetString("nonce");
            string echostr = ZNRequest.GetString("echostr");

            LogHelper.ErrorLoger.Error("Api/Weixin/Check:" + signature + "," + timestamp + "," + nonce + "," + echostr);


            //按字典排序 
            String[] arr = new String[] { Token, timestamp, nonce };
            Array.Sort(arr);
            StringBuilder content = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                content.Append(arr[i]);
            }

            LogHelper.ErrorLoger.Error("Api/Weixin/Check:" + content.ToString() + "," + SHA1(content.ToString()));

            //加密并返回验证结果  
            var result = string.IsNullOrWhiteSpace(signature) ? false : signature.ToUpper().Equals(SHA1(content.ToString()));

            LogHelper.ErrorLoger.Error("Api/Weixin/Check:" + result);

            if (result)
            {
                return echostr;
            }
            else
            {
                return "";
            }
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