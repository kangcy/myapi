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
    public class SystemController : BaseApiController
    {
        /// <summary>
        /// 判断敏感词
        /// </summary>
        [HttpGet]
        [Route("Api/System/CheckDirtyword")]
        public string CheckDirtyword()
        {
            ApiResult result = new ApiResult();
            try
            {
                var title = UrlDecode(ZNRequest.GetString("Title"));
                if (ProcessSqlStr(title))
                {
                    result.message = "存在违禁词";
                    return JsonConvert.SerializeObject(result);
                }
                var dirtyword = HasDirtyWord(title);
                if (!string.IsNullOrWhiteSpace(dirtyword))
                {
                    result.message = "您输入的内容含有敏感内容[" + dirtyword + "]，请检查后重试哦";
                    return JsonConvert.SerializeObject(result);
                }
                result.result = true;
                result.message = "";
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_System_CheckDirtyword:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 判断敏感词
        /// </summary>
        [HttpPost]
        [Route("Api/System/CheckDirty")]
        public IHttpActionResult CheckDirty()
        {
            ApiResult result = new ApiResult();
            try
            {
                var title = UrlDecode(ZNRequest.GetString("Title"));
                if (ProcessSqlStr(title))
                {
                    result.message = "存在违禁词";
                    return Json<ApiResult>(result);
                }
                var dirtyword = HasDirtyWord(title);
                if (!string.IsNullOrWhiteSpace(dirtyword))
                {
                    result.message = "您输入的内容含有敏感内容[" + dirtyword + "]，请检查后重试哦";
                    return Json<ApiResult>(result);
                }
                result.result = true;
                result.message = "";
                return Json<ApiResult>(result);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_System_CheckDirty:" + ex.Message);
                result.message = ex.Message;
            }
            return Json<ApiResult>(result);
        }
    }
}