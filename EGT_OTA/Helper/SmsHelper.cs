using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 短信
    /// </summary>
    public class SmsHelper
    {
        /// <summary>
        /// 接口服务器IP地址（从会员中心--短信中心--帐号签名中获取）
        /// </summary>
        static string SERVER_IP = System.Web.Configuration.WebConfigurationManager.AppSettings["smsip"].ToString();

        /// <summary>
        /// 接口服务器端口（从会员中心--短信中心--帐号签名中获取）
        /// </summary>
        static string SERVER_PORT = System.Web.Configuration.WebConfigurationManager.AppSettings["smsport"].ToString();

        /// <summary>
        /// 短信账号（从会员中心获取，请注意不是会员的登录帐号短信帐号以“ZH”开头）
        /// </summary>
        static string ACCOUNT_SID = System.Web.Configuration.WebConfigurationManager.AppSettings["smsuser"].ToString();

        /// <summary>
        /// APIKEY（从会员中心--短信中心--帐号签名中获取，APIKEY可以自主在会员中心更新）
        /// </summary>
        static string ACCOUNT_APIKEY = System.Web.Configuration.WebConfigurationManager.AppSettings["smspwd"].ToString();

        /// <summary>
        /// 发送模板短信
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="mobile">手机号码</param>
        /// <param name="content">短信内容</param>
        /// <param name="extno">扩展码（从会员中心帐号列表中获取）</param>
        /// <returns></returns>
        public static string Send(string templateId, string mobile, string content, string extno)
        {
            string resultJson = "";
            try
            {
                //请求头部设置认证信息
                string src = string.Format("{0}{1}{2}{3}{4}", ACCOUNT_SID, ACCOUNT_APIKEY, templateId, mobile, content);
                string sign = GetMD5(src);
                //短信发送的相关参数
                StringBuilder dic = new StringBuilder();
                dic.AppendFormat("&sid={0}", ACCOUNT_SID);
                dic.AppendFormat("&sign={0}", sign);

                dic.AppendFormat("&tplid={0}", templateId);
                dic.AppendFormat("&mobile={0}", mobile);
                dic.AppendFormat("&content={0}", content);
                dic.AppendFormat("&extno={0}", extno);

                //构建请求URL，所有url都必须包含sign、sid参数
                string url = string.Format("http://{0}:{1}/rcsapi/rest/sms/sendtplsms.json", SERVER_IP, SERVER_PORT);

                resultJson = SendHttpPost(url.ToString(), dic.ToString());
            }
            catch (Exception e)
            {
                resultJson = "";
            }
            return resultJson;
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string GetMD5(string encypStr)
        {
            string charset = "utf-8";
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url">路径</param>
        ///  <param name="paramData">参数</param>
        /// <returns>请求结果</returns>
        public static string SendHttpPost(string url, string paramData)
        {
            Stream respStream = null;
            HttpWebResponse httpResp = null;
            StreamReader respStreamReader = null;
            HttpWebRequest httpReq = null;
            string res = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
                httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                httpReq.Method = "POST";
                httpReq.ContentType = "application/x-www-form-urlencoded";
                httpReq.Headers.Add("Content-Encoding", "utf-8");
                httpReq.ContentLength = byteArray.Length;
                respStream = httpReq.GetRequestStream();
                respStream.Write(byteArray, 0, byteArray.Length);//写入参数
                respStream.Close();
                httpResp = (HttpWebResponse)httpReq.GetResponse();
                respStreamReader = new StreamReader(httpResp.GetResponseStream(), Encoding.UTF8);
                res = respStreamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (respStream != null)
                {
                    respStream.Close();
                }
                if (httpResp != null)
                {
                    httpResp.Close();
                }
                if (respStreamReader != null)
                {
                    respStreamReader.Close();
                }
            }
            return res;
        }
    }
}
