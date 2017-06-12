using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using CommonTools;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using IPay.Alipay;

namespace EGT_OTA.Controllers
{
    public class NotifyController : BaseController
    {
        protected SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// 支付宝支付
        /// </summary>
        public ActionResult AliPay()
        {
            var result = new ResultJson();
            bool verifyResult = false;
            string strResult = "";
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();

            Notify aliNotify = new Notify();
            verifyResult = aliNotify.Verify(sArray, ZNRequest.GetString("notify_id"), ZNRequest.GetString("sign"));
            foreach (KeyValuePair<string, string> kvp in sArray)
            {
                strResult += kvp.Key;
                strResult += ":";
                strResult += kvp.Value;
                strResult += "\r\n";
            }

            //记录文本日志
            //LogHelper.InfoLoger.Info("NotifyController_AliPay:" + "支付通知参数：" + strResult);

            if (verifyResult && !string.IsNullOrWhiteSpace(strResult))
            {
                //商户订单号
                string oid = sArray["out_trade_no"];

                //支付宝交易号
                string trade_no = sArray["trade_no"];

                //商品名称
                var subject = sArray["subject"];

                //支付价格
                var price = sArray["price"];

                //交易状态
                string trade_status = sArray["trade_status"];

                var order = db.Single<Order>(x => x.OrderNumber == oid);

                if (trade_status == "TRADE_FINISHED" && order != null)
                {
                    //更新订单状态
                    order.Status = Enum_Status.Approved;
                    db.Update<Order>(order);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region  微信支付

        /// <summary>
        /// 微信支付
        /// </summary>
        public ActionResult WxPay()
        {
            var result = new ResultJson();
            Stream stream = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[stream.Length];
            stream.Read(b, 0, (int)stream.Length);
            string xml = Encoding.UTF8.GetString(b);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            //LogHelper.InfoLoger.Info("NotifyController_WxPay:" + "支付通知参数：" + xml);

            if (xml.IndexOf("SUCCESS") > 0)
            {
                FromXml(xml);

                if (CheckSign())
                {
                    string out_trade_no = GetValue("out_trade_no").ToString();
                    string productname = GetValue("attach").ToString();
                    string openid = GetValue("openid").ToString();
                    string pay_fee = GetValue("cash_fee").ToString();
                    double price = double.Parse(pay_fee) / 100;
                    var order = db.Single<Order>(x => x.OrderNumber == out_trade_no);

                    if (order != null)
                    {
                        if (order.Status != Enum_Status.Approved)
                        {
                            //更新充值状态
                            order.Status = Enum_Status.Approved;

                            db.Update<Order>(order);

                            //推送
                            new AppHelper().Push(order.ToUserNumber, 0, "", "", Enum_PushType.Money);
                        }
                    }
                }
                else
                {
                    LogHelper.InfoLoger.Info("NotifyController_WxPay:" + "微信支付签名验证失败");
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  微信下单

        [HttpGet]
        public ActionResult AddWxOrder()
        {
            try
            {
                //创建订单
                Order order = new Order();
                order.OrderNumber = Guid.NewGuid().ToString("N");
                order.CreateDate = DateTime.Now;
                order.PayType = 2;
                order.Status = Enum_Status.Audit;
                order.Summary = "我的微篇-打赏";
                order.Price = ZNRequest.GetInt("Money", 0);//单位：分
                if (order.Price == 0)
                {
                    order.Price = 1;
                }
                order.Anony = ZNRequest.GetInt("Anony", 0);
                order.CreateUserNumber = ZNRequest.GetString("UserNumber");
                order.ToArticleNumber = ZNRequest.GetString("ArticleNumber");
                if (string.IsNullOrWhiteSpace(order.ToArticleNumber))
                {
                    order.ToUserNumber = ZNRequest.GetString("ArticleUserNumber");
                }
                else
                {
                    var toUserNumber = db.Single<Article>(x => x.Number == order.ToArticleNumber);
                    order.ToUserNumber = toUserNumber == null ? "" : toUserNumber.CreateUserNumber;
                }
                order.CreateIP = Tools.GetClientIP;
                db.Add<Order>(order);

                string wx_prepay = "https://api.mch.weixin.qq.com/pay/unifiedorder";

                string body = order.Summary;
                string nonce_str = GetMD5(DateTime.Now.ToString("yyyyMMddhhmmssffff"));
                string notify_url = Base_Url + "Notify/WxPay/";//支付成功回调
                string out_trade_no = order.OrderNumber;
                string appid = System.Configuration.ConfigurationManager.AppSettings["wxappid"];
                string partner = System.Configuration.ConfigurationManager.AppSettings["wxapppartner"];
                string partnerKey = System.Configuration.ConfigurationManager.AppSettings["wxapppartnerkey"];

                string spbill_create_ip = Tools.GetClientIP;
                string total_fee = order.Price.ToString();//总金额。

                string signString = "appid=" + appid + "&attach=" + body + "&body=" + body + "&mch_id=" + partner + "&nonce_str=" + nonce_str + "&notify_url=" + notify_url + "&out_trade_no=" + out_trade_no + "&spbill_create_ip=" + spbill_create_ip + "&total_fee=" + total_fee + "&trade_type=APP" + "&key=" + partnerKey;

                string md5SignValue = GetMD5(signString);

                StringBuilder strXML = new StringBuilder();
                strXML.Append("<xml>");
                strXML.Append("<appid>" + appid + "</appid>");
                strXML.Append("<attach>" + body + "</attach>");
                strXML.Append("<body>" + body + "</body>");
                strXML.Append("<mch_id>" + partner + "</mch_id>");
                strXML.Append("<nonce_str>" + nonce_str + "</nonce_str>");
                strXML.Append("<notify_url>" + notify_url + "</notify_url>");
                strXML.Append("<out_trade_no>" + out_trade_no + "</out_trade_no>");
                strXML.Append("<spbill_create_ip>" + spbill_create_ip + "</spbill_create_ip>");
                strXML.Append("<total_fee>" + total_fee + "</total_fee>");
                strXML.Append("<trade_type>APP</trade_type>");
                strXML.Append("<sign>" + md5SignValue + "</sign>");
                strXML.Append("</xml>");

                //LogHelper.ErrorLoger.Error("NotifyController_AddWxOrder1:" + strXML.ToString());

                string strSource = GetHttp(wx_prepay, strXML.ToString());

                //LogHelper.ErrorLoger.Error("NotifyController_AddWxOrder2:" + strSource);

                if (strSource.IndexOf("prepay_id") > 0)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(strSource);
                    XmlNode node_prepay_id = doc.LastChild.ChildNodes.Item(7);
                    string prepayid = node_prepay_id.InnerText;
                    long timeStamp = MakeTimestamp();

                    Hashtable paySignReqHandler = new Hashtable();
                    paySignReqHandler.Add("appid", appid);
                    paySignReqHandler.Add("partnerid", partner);
                    paySignReqHandler.Add("prepayid", prepayid);
                    paySignReqHandler.Add("noncestr", nonce_str);
                    paySignReqHandler.Add("package", "Sign=WXPay");
                    paySignReqHandler.Add("timestamp", timeStamp.ToString());
                    var paySign = CreateMd5Sign(paySignReqHandler, partnerKey);

                    //LogHelper.ErrorLoger.Error("NotifyController_AddWxOrder3:" + paySign);

                    var obj = new { appid = appid, noncestr = nonce_str, package = "Sign=WXPay", partnerid = partner, prepayid = prepayid, timestamp = timeStamp, sign = paySign };

                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                }
                else
                {
                    return Content(string.Empty);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("NotifyController_AddWxOrder_Error:" + ex.Message);
                return Content(string.Empty);
            }
        }

        #endregion

        #region  帮助方法

        private Int64 MakeTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        private string CreateMd5Sign(Hashtable parameters, string ApiKey)
        {
            var sb = new StringBuilder();
            var akeys = new ArrayList(parameters.Keys);
            akeys.Sort();//排序，这是微信要求的
            foreach (string k in akeys)
            {
                var v = (string)parameters[k];
                sb.Append(k + "=" + v + "&");
            }
            sb.Append("key=" + ApiKey);
            string sign = GetMD5(sb.ToString());
            return sign;
        }

        private string GetMD5(string src)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(src);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            var retStr = BitConverter.ToString(md5data);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        /// <summary>   
        /// 用HttpWebRequest取得网页源码   
        /// 对于带BOM的网页很有效，不管是什么编码都能正确识别   
        /// </summary>   
        /// <param name="url">网页地址" </param>    
        /// <returns>返回网页源文件</returns>   
        private string GetHttp(string url, string param)
        {
            Encoding encoding = Encoding.UTF8;
            string responseData = String.Empty;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            byte[] bs = Encoding.UTF8.GetBytes(param);
            request.Method = "POST";
            request.AllowAutoRedirect = true;//是否允许302

            request.ContentLength = bs.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseData = reader.ReadToEnd().ToString();
                }
            }
            return responseData;
        }

        /// <summary>
        /// xml转排序字典
        /// </summary>
        public SortedDictionary<string, object> FromXml(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }
            return m_values;
        }

        /// <summary>
        /// 检查签名
        /// </summary>
        public bool CheckSign()
        {
            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();
            //在本地计算新的签名
            string cal_sign = MakeSign();
            if (cal_sign == return_sign)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 本地签名
        /// </summary>
        public string MakeSign()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            string partnerKey = System.Configuration.ConfigurationManager.AppSettings["wxapppartnerkey"];
            str += "&key=" + partnerKey;
            //MD5加密
            str = GetMD5(str);
            //所有字符转为大写
            return str.ToUpper();
        }

        /// <summary>
        /// 获取字典值
        /// </summary>
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /// <summary>
        /// 字典转URL
        /// </summary>
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    //Axon.Log.SaveLog("通知参数中含有null字段", context.Server.MapPath("~/log/" + DateTime.Now.ToString("yyyyMMdd") + ".log"));
                }
                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }

        #endregion
    }
}
