using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using EGT_OTA.Controllers.Filter;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json;

namespace EGT_OTA.Controllers.Api
{
    public class ArticleTypeController : BaseApiController
    {
        [HttpGet]
        [Route("Api/ArticleType/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var list = AppHelper.GetArticleType().Where(x => x.ID > 0).OrderBy(x => x.SortID).ToList();
                list.ForEach(x =>
                {
                    x.Cover = GetFullUrl(x.Cover);
                });
                result.result = true;
                result.message = list;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_ArticleType_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        [Route("Api/ArticleType/All2")]
        public string All2()
        {
            ApiResult result = new ApiResult();
            try
            {
                var list = AppHelper.GetArticleType();
                var first = list.FindAll(x => x.ParentID == 0 && x.ID > 0).OrderBy(x => x.ID).ToList();

                first.ForEach(x =>
                {
                    x.List = new List<ArticleType>();
                    x.List.AddRange(list.FindAll(y => y.ParentID == x.ID).OrderBy(y => y.ID).ToList());
                });
                result.result = true;
                result.message = first;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_ArticleType_All2:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}