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
    public class HelpController : BaseApiController
    {
        /// <summary>
        /// 帮助类型
        /// </summary>
        [HttpGet]
        [Route("Api/Help/HelpType")]
        public string HelpType()
        {
            ApiResult result = new ApiResult();
            try
            {
                result.result = true;
                result.message = AppHelper.InitHelpType();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Help_HelpType:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 帮助列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Help/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var type = ZNRequest.GetInt("type");
                result.result = true;
                result.message = AppHelper.GetHelp().FindAll(x => x.HelpType == type);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Help_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}