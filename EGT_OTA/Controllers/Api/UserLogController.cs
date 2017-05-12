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
    public class UserLogController : BaseApiController
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [HttpGet]
        [Route("Api/UserLog/Edit")]
        public string Edit()
        {
            ApiResult result = new ApiResult();
            try
            {
                var info = ZNRequest.GetString("info");
                if (!string.IsNullOrWhiteSpace(info))
                {
                    UserLog log = new UserLog();
                    log.Info = info;
                    db.Add<UserLog>(log);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_UserLog_Edit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}