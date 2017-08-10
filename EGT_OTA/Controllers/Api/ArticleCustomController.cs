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
    public class ArticleCustomController : BaseApiController
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [HttpGet]
        [Route("Api/ArticleCustom/Edit")]
        public string Edit()
        {
            ApiResult result = new ApiResult();
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    result.message = EnumBase.GetDescription(typeof(Enum_ErrorCode), Enum_ErrorCode.UnLogin);
                    result.code = Enum_ErrorCode.UnLogin;
                    return JsonConvert.SerializeObject(result);
                }
                var number = ZNRequest.GetString("ArticleNumber");
                var url = ZNRequest.GetString("ShowyUrl");
                if (string.IsNullOrWhiteSpace(number))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                ArticleCustom model = db.Single<ArticleCustom>(x => x.ArticleNumber == number);
                if (model == null)
                {
                    model = new ArticleCustom();
                    model.ArticleNumber = number;
                }
                model.ShowyUrl = url;
                var success = false;
                if (model.ID == 0)
                {
                    success = Tools.SafeInt(db.Add<ArticleCustom>(model)) > 0;
                }
                else
                {
                    success = db.Update<ArticleCustom>(model) > 0;
                }
                if (success)
                {
                    result.result = true;
                    result.message = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_ArticleCustom_Edit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 列表
        /// </summary>
        [HttpGet]
        [Route("Api/ArticleCustom/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var list = new List<ArticleCustom>();
                var number = ZNRequest.GetString("ArticleNumber");
                if (!string.IsNullOrWhiteSpace(number))
                {
                    list = db.Find<ArticleCustom>(x => x.ArticleNumber == number).OrderByDescending(x => x.ID).ToList();
                }
                result.result = true;
                result.message = list;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_ArticleCustom_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}