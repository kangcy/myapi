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
    }
}