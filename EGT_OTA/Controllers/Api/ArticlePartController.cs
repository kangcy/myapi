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
    public class ArticlePartController : BaseApiController
    {
        /// <summary>
        /// 批量导入
        /// </summary>
        [HttpGet]
        [Route("Api/ArticlePart/Import")]
        public string Import()
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
                var urls = ZNRequest.GetString("Url");
                var number = ZNRequest.GetString("ArticleNumber");
                if (string.IsNullOrWhiteSpace(urls))
                {
                    result.message = "信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                var list = new List<ArticlePartJson>();
                var url = urls.Split(',').ToList();
                url.ForEach(x =>
                {
                    var model = new ArticlePart();
                    model.Introduction = x;
                    model.IntroExpand = string.Empty;
                    model.ArticleNumber = number;
                    model.Types = Enum_ArticlePart.Pic;
                    model.CreateUserNumber = user.Number;
                    model.CreateIP = Tools.GetClientIP;
                    model.CreateDate = DateTime.Now;
                    model.Status = Enum_Status.Audit;
                    model.SortID = 0;
                    var id = Tools.SafeInt(db.Add<ArticlePart>(model));
                    list.Add(new ArticlePartJson(id, x));
                });
                result.result = true;
                result.message = list;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_ArticlePart_Import" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}