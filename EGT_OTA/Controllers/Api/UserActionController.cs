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
    /// <summary>
    /// 用户操作
    /// </summary>
    public class UserActionController : BaseApiController
    {
        /// <summary>
        /// 用户操作
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/UserAction/List")]
        public string List()
        {
            ApiResult result = new ApiResult();
            try
            {
                var ActionID = ZNRequest.GetInt("ActionID");
                var UserNumber = ZNRequest.GetString("UserNumber");
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }

                //我的关注
                var follows = db.Find<Fan>(x => x.CreateUserNumber == UserNumber).ToList();
                if (follows.Count == 0)
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<UserAction>().Where("CreateUserNumber").In(follows.Select(x => x.ToUserNumber).ToArray());
                //if (ActionID > 0)
                //{
                //    query = query.And("ID").IsLessThan(ActionID);
                //}
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<UserAction>();

                var like = Enum_ActionType.Like;
                var follow = Enum_ActionType.Follow;
                var keep = Enum_ActionType.Keep;
                var userList = new List<string>();
                list.ForEach(x =>
                {
                    userList.Add(x.CreateUserNumber);
                });
                var newlist = new List<UserActionJson>();
                if (list.Count > 0)
                {
                    var allUser = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Cover", "Number").From<User>().Where<User>(y => y.Status == Enum_Status.Approved).And("Number").In(userList.Distinct().ToArray()).ExecuteTypedList<User>();
                    list.ForEach(x =>
                    {
                        var userModel = allUser.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                        if (userModel != null)
                        {
                            UserActionJson action = new UserActionJson();
                            action.ID = x.ID;
                            action.Number = x.CreateUserNumber;
                            action.NickName = userModel.NickName;
                            action.Avatar = userModel.Avatar;
                            action.Cover = userModel.Cover;
                            action.CreateTime = FormatTime(x.CreateTime);
                            action.ActionType = x.ActionType;
                            action.ArticleInfoJson = new List<ArticleInfoJson>();
                            action.UserInfoJson = new List<UserInfoJson>();
                            //喜欢、收藏
                            if (x.ActionType == like || x.ActionType == keep)
                            {
                                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Cover", "ArticlePower", "CreateUserNumber", "Status", "Title").From<Article>().Where("Number").In(x.ActionInfo.Split(',')).And("Status").IsEqualTo(Enum_Status.Approved).And("ArticlePower").IsEqualTo(Enum_ArticlePower.Public).ExecuteTypedList<Article>();
                                if (articles.Count > 0)
                                {
                                    articles.ForEach(y =>
                                    {
                                        ArticleInfoJson article = new ArticleInfoJson();
                                        article.ID = y.ID;
                                        article.Title = y.Title;
                                        article.Number = y.Number;
                                        article.Cover = y.Cover;
                                        article.ArticlePower = y.ArticlePower;
                                        action.ArticleInfoJson.Add(article);
                                    });
                                    newlist.Add(action);
                                }
                            }
                            //关注
                            if (x.ActionType == follow)
                            {
                                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Cover", "Number").From<User>().Where<User>(y => y.Status == Enum_Status.Approved).And("Number").In(x.ActionInfo.Split(',')).ExecuteTypedList<User>();
                                if (users.Count > 0)
                                {
                                    users.ForEach(y =>
                                    {
                                        UserInfoJson user = new UserInfoJson();
                                        user.ID = y.ID;
                                        user.NickName = y.NickName;
                                        user.Number = y.Number;
                                        user.Avatar = y.Avatar;
                                        user.Cover = y.Cover;
                                        action.UserInfoJson.Add(user);
                                    });
                                    newlist.Add(action);
                                }
                            }
                        }
                    });
                }
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
                LogHelper.ErrorLoger.Error("Api/UserAction/List:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}