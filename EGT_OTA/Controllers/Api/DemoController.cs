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
using System.IO;
using System.Drawing;

namespace EGT_OTA.Controllers.Api
{
    public class DemoController : BaseApiController
    {
        /// <summary>
        /// 敏感词列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Demo/Dirtyword")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(GetDirtyWord());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 下载表情图标
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Demo/Icons")]
        public void Icons()
        {

            List<string> list = new List<string>();
            string str = string.Empty;
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/icons.config");
            if (System.IO.File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath, Encoding.Default);
                str = sr.ReadToEnd();
                sr.Close();
            }
            list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(str);
            list.ForEach(x =>
            {
                var url = string.Format("https://cdnjs.cloudflare.com/ajax/libs/emojione/1.5.2/assets/png/{0}.png", x);

                WebClient wc = new WebClient();
                byte[] buffer = wc.DownloadData(new Uri(url));
                MemoryStream ms = new MemoryStream(buffer);
                Image image = System.Drawing.Image.FromStream(ms);

                string savePath = System.Web.HttpContext.Current.Server.MapPath("~/Images/Icons/" + x + ".png");
                image.Save(savePath);
                image.Dispose();
                ms.Close();
            });
        }
    }
}