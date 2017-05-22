using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO.Compression;
using System.Collections.Specialized;
using System.Data;
using System.Web;
using System.Xml;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// Http请求
    /// </summary>
    public class HttpUtil
    {         
        #region POST方法

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     ContentType为默认："application/x-www-form-urlencoded"；
        ///     UserAgent为默认："Mozilla-Firefox-Spider(Axon)"。
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">string类型。提交数据。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Post(string url, string postdata)
        {
            return Post(url, postdata, "application/x-www-form-urlencoded", "");
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     ContentType为默认："application/x-www-form-urlencoded"；
        ///     UserAgent为默认："Mozilla-Firefox-Spider(Axon)"。
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">IDictionary。提交数据。</param>
        public static string Post(string url, IDictionary<string, object> parameters)
        {
            string data = string.Empty;
            if (!(parameters == null || parameters.Count == 0))
            {
                var list = new List<string>();
                foreach (var p in parameters)
                {
                    list.Add(p.Key + "=" + p.Value);
                }
                data = string.Join("&", list.ToArray());
            }
            return Post(url, data, "application/x-www-form-urlencoded", "");
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     ContentType为默认："application/x-www-form-urlencoded"
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">string类型。提交数据。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Post(string url, string postdata, string useragent)
        {
            return Post(url, postdata, "application/x-www-form-urlencoded", useragent);
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     ContentType为默认："text/xml"
        ///     UserAgent为默认："Mozilla-Firefox-Spider(Axon)"。
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">string类型。提交数据。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string PostXML(string url, string postdata)
        {
            return Post(url, postdata, "text/xml", "");
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     ContentType为默认："text/xml"
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">string类型。提交数据。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string PostXML(string url, string postdata, string useragent)
        {
            return Post(url, postdata, "text/xml", useragent);
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">string类型。提交数据。</param>
        /// <param name="contenttype">string类型。ContentType，如"application/x-www-form-urlencoded"。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Post(string url, string postdata, string contenttype, string useragent)
        {
            return Post(url, postdata, contenttype, useragent, null);
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     详细编码的代码页名称参考：http://msdn.microsoft.com/zh-cn/library/system.text.encoding.aspx
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">string类型。提交数据。</param>
        /// <param name="contenttype">string类型。ContentType，如"application/x-www-form-urlencoded"。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <param name="encodingstring">string类型。编码的代码页名称，如"utf-8"。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Post(string url, string postdata, string contenttype, string useragent,
            string encodingstring)
        {
            return Post(url, postdata, contenttype, useragent, encodingstring, null, null);
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     详细编码的代码页名称参考：http://msdn.microsoft.com/zh-cn/library/system.text.encoding.aspx
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="postdata">string类型。提交数据。</param>
        /// <param name="contenttype">string类型。ContentType，如"application/x-www-form-urlencoded"。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <param name="encodingstring">string类型。编码的代码页名称，如"utf-8"。</param>
        /// <param name="proxy">WebProxy类型。Webproxy。</param>
        /// <param name="cookie">CookieContainer类型。Cookies。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Post(string url, string postdata, string contenttype, string useragent,
            string encodingstring, WebProxy proxy, CookieContainer cookie)
        {
            var ret = "";
            try
            {
                var httpReq = (HttpWebRequest)WebRequest.Create(url);
                httpReq.Method = "POST";
                httpReq.ContentType = contenttype;
                httpReq.Accept = "*/*";
                httpReq.UserAgent = useragent;
                if (proxy != null)
                {
                    httpReq.Proxy = proxy;
                }
                if (cookie != null)
                {
                    httpReq.CookieContainer = cookie;
                }
                var reqStream = httpReq.GetRequestStream();
                var sw = new StreamWriter(reqStream);
                sw.Write(postdata);
                sw.Close();
                var rspStream = httpReq.GetResponse().GetResponseStream();
                var encoding = Encoding.Default;
                if (string.IsNullOrEmpty(encodingstring))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(encodingstring);
                }
                var sr = new StreamReader(rspStream, encoding);
                ret = sr.ReadToEnd();
                sr.Close();
                rspStream.Close();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <param name="PostUrl">string类型。提交地址。</param>
        /// <param name="PostValue">NameValueCollection类型。需要提交的数据。</param>
        /// <param name="HeaderValue">NameValueCollection类型。设置提交的请求头。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Post(string PostUrl, NameValueCollection PostValue, NameValueCollection HeaderValue)
        {
            return Post(PostUrl, PostValue, HeaderValue, null);
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <param name="PostUrl">string类型。提交地址。</param>
        /// <param name="PostValue">需要提交的数据。</param>
        /// <param name="HeaderValue">NameValueCollection类型。设置提交的请求头。</param>
        /// <returns>string类型。接收返回。</returns>
        public static DataSet PostDs(string PostUrl, NameValueCollection PostValue, NameValueCollection HeaderValue)
        {
            var ds = new DataSet();
            ds.ReadXml(new XmlTextReader(new StringReader(Post(PostUrl, PostValue, HeaderValue, null))));
            return ds;
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <param name="PostUrl">string类型。提交地址。</param>
        /// <param name="PostValue">需要提交的数据。</param>
        /// <param name="HeaderValue">NameValueCollection类型。设置提交的请求头。</param>
        /// <returns>string类型。接收返回。</returns>
        public static DataSet Post<T>(string PostUrl, T PostValue, NameValueCollection HeaderValue)
        {
            if (PostValue == null || string.IsNullOrWhiteSpace(PostUrl))
            {
                return new DataSet();
            }
            System.Reflection.PropertyInfo[] properties = PostValue.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return new DataSet();
            }
            var list = new NameValueCollection();
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name; //名称  
                object value = item.GetValue(PostValue, null);  //值  
                list.Add(name, value.ToString());
            }
            var ds = new DataSet();
            ds.ReadXml(new XmlTextReader(new StringReader(Post(PostUrl, list, HeaderValue, null))));
            return ds;
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <param name="PostUrl">string类型。提交地址。</param>
        /// <param name="PostValue">需要提交的数据。</param>
        /// <param name="HeaderValue">NameValueCollection类型。设置提交的请求头。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string PostService<T>(string PostUrl, T PostValue, NameValueCollection HeaderValue)
        {
            if (PostValue == null || string.IsNullOrWhiteSpace(PostUrl))
            {
                return string.Empty;
            }
            System.Reflection.PropertyInfo[] properties = PostValue.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return string.Empty;
            }
            var list = new NameValueCollection();
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name; //名称  
                object value = item.GetValue(PostValue, null);  //值  
                list.Add(name, value.ToString());
            }
            return Post(PostUrl, list, HeaderValue, null);
        }

        /// <summary>
        ///     POST提交方法重载
        /// </summary>
        /// <remarks>
        ///     详细编码的代码页名称参考：http://msdn.microsoft.com/zh-cn/library/system.text.encoding.aspx
        /// </remarks>
        /// <param name="PostUrl">string类型。提交地址。</param>
        /// <param name="PostValue">NameValueCollection类型。需要提交的数据。</param>
        /// <param name="HeaderValue">NameValueCollection类型。设置提交的请求头。</param>
        /// <param name="encodingstring">string类型。编码的代码页名称，如"utf-8"。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Post(string PostUrl, NameValueCollection PostValue, NameValueCollection HeaderValue,
            string encodingstring)
        {
            try
            {
                //定义webClient对象
                var webClient = new WebClient();
                var encoding = Encoding.Default;
                if (string.IsNullOrEmpty(encodingstring))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(encodingstring);
                }
                webClient.Encoding = encoding;
                if (HeaderValue != null)
                {
                    try
                    {
                        //请求头
                        var myEnumerator = HeaderValue.GetEnumerator();
                        foreach (var headername in HeaderValue.AllKeys)
                        {
                            webClient.Headers.Add(headername, HeaderValue[headername]);
                        }
                    }
                    catch
                    {
                    }
                }
                //向服务器发送POST数据
                var responseArray = webClient.UploadValues(PostUrl, PostValue);
                var contentencodeing = webClient.ResponseHeaders[HttpResponseHeader.ContentEncoding];
                var data = GetStringFromArray(responseArray, contentencodeing, encoding);
                return data;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion

        #region GET方法

        /// <summary>
        ///     GET方法重载
        /// </summary>
        /// <remarks>
        ///     ContentType为默认："text/html"；
        ///     UserAgent为默认："Mozilla-Firefox-Spider(Axon)"。
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Get(string url)
        {
            return Get(url, "text/html", "Mozilla-Firefox-Spider(Axon)");
        }

        /// <summary>
        ///     GET方法重载
        /// </summary>
        /// <remarks>
        ///     ContentType为默认："text/html"
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Get(string url, string useragent)
        {
            return Get(url, "text/html", useragent);
        }

        /// <summary>
        ///     GET方法重载
        /// </summary>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="contenttype">string类型。ContentType。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Get(string url, string contenttype, string useragent)
        {
            return Get(url, contenttype, useragent, null);
        }

        /// <summary>
        ///     GET方法重载
        /// </summary>
        /// <remarks>
        ///     详细编码的代码页名称参考：http://msdn.microsoft.com/zh-cn/library/system.text.encoding.aspx
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="contenttype">string类型。ContentType。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <param name="encodingstring">string类型。编码的代码页名称，如"utf-8"。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Get(string url, string contenttype, string useragent, string encodingstring)
        {
            return Get(url, contenttype, useragent, encodingstring, null, null);
        }

        /// <summary>
        ///     GET方法重载
        /// </summary>
        /// <remarks>
        ///     详细编码的代码页名称参考：http://msdn.microsoft.com/zh-cn/library/system.text.encoding.aspx
        /// </remarks>
        /// <param name="url">string类型。提交地址。</param>
        /// <param name="contenttype">string类型。ContentType。</param>
        /// <param name="useragent">string类型。User-Agent。</param>
        /// <param name="encodingstring">string类型。编码的代码页名称，如"utf-8"。</param>
        /// <param name="proxy">WebProxy类型。Webproxy。</param>
        /// <param name="cookie">CookieContainer类型。Cookies。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Get(string url, string contenttype, string useragent, string encodingstring, WebProxy proxy,
            CookieContainer cookie)
        {
            var ret = "";
            try
            {
                var httpReq = (HttpWebRequest)WebRequest.Create(url);
                httpReq.Method = "GET";
                httpReq.ContentType = contenttype;
                httpReq.Accept = "*/*";
                httpReq.UserAgent = useragent;
                if (proxy != null)
                {
                    httpReq.Proxy = proxy;
                }
                if (cookie != null)
                {
                    httpReq.CookieContainer = cookie;
                }

                var rspStream = httpReq.GetResponse().GetResponseStream();
                var encoding = Encoding.Default;
                if (string.IsNullOrEmpty(encodingstring))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(encodingstring);
                }
                var sr = new StreamReader(rspStream, encoding);
                ret = sr.ReadToEnd();
                sr.Close();
                rspStream.Close();
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        /// <summary>
        ///     GET方法重载
        /// </summary>
        /// <param name="GetUrl">string类型。请求地址。</param>
        /// <param name="HeaderValue">NameValueCollection类型。设置提交的请求头。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Get(string GetUrl, NameValueCollection HeaderValue)
        {
            return Get(GetUrl, HeaderValue, null);
        }

        /// <summary>
        ///     GET方法重载
        /// </summary>
        /// <remarks>
        ///     详细编码的代码页名称参考：http://msdn.microsoft.com/zh-cn/library/system.text.encoding.aspx
        /// </remarks>
        /// <param name="GetUrl">string类型。请求地址。</param>
        /// <param name="HeaderValue">NameValueCollection类型。设置提交的请求头。</param>
        /// <param name="encodingstring">string类型。编码的代码页名称，如"utf-8"。</param>
        /// <returns>string类型。接收返回。</returns>
        public static string Get(string GetUrl, NameValueCollection HeaderValue, string encodingstring)
        {
            try
            {
                //定义webClient对象
                var webClient = new WebClient();
                var encoding = Encoding.Default;
                if (string.IsNullOrEmpty(encodingstring))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(encodingstring);
                }
                webClient.Encoding = encoding;
                if (HeaderValue != null)
                {
                    try
                    {
                        //请求头
                        var myEnumerator = HeaderValue.GetEnumerator();
                        foreach (var headername in HeaderValue.AllKeys)
                        {
                            webClient.Headers.Add(headername, HeaderValue[headername]);
                        }
                    }
                    catch
                    {
                    }
                }
                //向服务器发送并接收数据
                var responseArray = webClient.DownloadData(GetUrl);
                var contentencodeing = webClient.ResponseHeaders[HttpResponseHeader.ContentEncoding];
                var data = GetStringFromArray(responseArray, contentencodeing, encoding);
                return data;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        ///     从数组和Gzip解析出字符串
        /// </summary>
        private static string GetStringFromArray(byte[] responseData, string Gzip, Encoding encoding)
        {
            var str = "";
            if ((Gzip != null) && (Gzip == "gzip"))
            {
                str = gzipDecompress(responseData, encoding);
            }
            else
            {
                str = encoding.GetString(responseData);
            }
            return str;
        }

        /// <summary>
        ///     GZIP解压缩
        /// </summary>
        private static string gzipDecompress(byte[] responseData, Encoding encoding)
        {
            var builder = new StringBuilder(0x19000);
            var stream = new GZipStream(new MemoryStream(responseData), CompressionMode.Decompress);
            var buffer = new byte[0x5000];
            for (var i = stream.Read(buffer, 0, 0x5000); i > 0; i = stream.Read(buffer, 0, 0x5000))
            {
                builder.Append(encoding.GetString(buffer, 0, i));
            }
            return builder.ToString();
        }

        #endregion
    }
}
