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
        /// 编辑
        /// </summary>
        [HttpGet]
        [Route("Api/Keep/Edit")]
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
                var articleID = ZNRequest.GetInt("ArticleID");
                if (articleID <= 0)
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                Article article = new SubSonic.Query.Select(provider, "ID", "CreateUserNumber", "Number").From<Article>().Where<Article>(x => x.ID == articleID).ExecuteSingle<Article>();
                if (article == null)
                {
                    result.message = "文章信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                Keep model = db.Single<Keep>(x => x.CreateUserNumber == user.Number && x.ArticleNumber == article.Number);
                if (model == null)
                {
                    model = new Keep();
                    model.CreateDate = DateTime.Now;
                    model.CreateUserNumber = user.Number;
                    model.CreateIP = Tools.GetClientIP;
                }
                else
                {
                    result.result = true;
                    result.message = "";
                    return JsonConvert.SerializeObject(result);
                }
                model.ArticleNumber = article.Number;
                model.ArticleUserNumber = article.CreateUserNumber;
                var success = Tools.SafeInt(db.Add<Keep>(model)) > 0;
                if (success)
                {
                    result.result = true;
                    result.message = user.Follows;


                    //操作记录
                    var now = DateTime.Now.ToString("yyyy-MM-dd");
                    var action = db.Single<UserAction>(x => x.CreateUserNumber == user.Number && x.CreateTimeText == now && x.ActionType == Enum_ActionType.Keep);
                    if (action == null)
                    {
                        action = new UserAction();
                        action.CreateUserNumber = user.Number;
                        action.ActionType = Enum_ActionType.Keep;
                        action.CreateTime = DateTime.Now;
                        action.CreateTimeText = now;
                        action.ActionInfo = article.Number;
                        db.Add<UserAction>(action);
                    }
                    else
                    {
                        if (!action.ActionInfo.Contains(article.Number))
                        {
                            action.ActionInfo += "," + article.Number;
                            db.Update<UserAction>(action);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Keep_Edit:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpGet]
        [Route("Api/Keep/Delete")]
        public string Delete()
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
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                if (string.IsNullOrWhiteSpace(ArticleNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var model = db.Single<Keep>(x => x.ArticleNumber == ArticleNumber && x.CreateUserNumber == user.Number);
                if (model == null)
                {
                    result.message = "信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                var success = db.Delete<Keep>(model.ID) > 0;
                if (success)
                {
                    result.result = true;
                    result.message = "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Keep_Delete:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

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