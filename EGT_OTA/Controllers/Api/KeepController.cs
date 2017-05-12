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
    public class KeepController : BaseApiController
    {
        /// <summary>
        /// 收藏列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Keep/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var CreateUserNumber = ZNRequest.GetString("CreateUserNumber");
                if (string.IsNullOrWhiteSpace(CreateUserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<Keep>().Where<Keep>(x => x.CreateUserNumber == CreateUserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Keep>();
                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Title", "TypeID", "Cover", "Views", "Goods", "CreateUserNumber", "CreateDate", "ArticlePower", "ArticlePowerPwd", "Recommend", "City", "Province").From<Article>().Where("Number").In(list.Select(x => x.ArticleNumber).ToArray()).And("Status").IsNotEqualTo(Enum_Status.Audit).OrderDesc(new string[] { "Recommend", "ID" }).ExecuteTypedList<Article>();

                List<ArticleJson> newlist = ArticleListInfo(articles, CreateUserNumber);
                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Keep_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}