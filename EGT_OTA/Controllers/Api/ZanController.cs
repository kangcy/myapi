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
    public class ZanController : BaseApiController
    {
        /// <summary>
        /// 文章点赞编辑
        /// </summary>
        [HttpGet]
        [Route("Api/Zan/ArticleZanEdit")]
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
                Article article = new SubSonic.Query.Select(provider, "ID", "CreateUserNumber", "Goods", "Number").From<Article>().Where<Article>(x => x.ID == articleID).ExecuteSingle<Article>();
                if (article == null)
                {
                    result.message = "文章信息异常";
                    return JsonConvert.SerializeObject(result);
                }

                //判断是否拉黑
                var black = db.Exists<Black>(x => x.CreateUserNumber == article.CreateUserNumber && x.ToUserNumber == user.Number);
                if (black)
                {
                    result.code = Enum_ErrorCode.NoPower;
                    result.message = "没有权限";
                    return JsonConvert.SerializeObject(result);
                }

                var success = 0;
                var model = db.Single<ArticleZan>(x => x.CreateUserNumber == user.Number && x.ArticleNumber == article.Number);
                var goods = model == null ? article.Goods + 1 : article.Goods - 1;

                //是否新增
                var isadd = model == null ? 0 : 1;
                if (model == null)
                {
                    model = new ArticleZan();
                    model.CreateDate = DateTime.Now;
                    model.CreateUserNumber = user.Number;
                    model.CreateIP = Tools.GetClientIP;
                    model.ArticleNumber = article.Number;
                    model.ArticleUserNumber = article.CreateUserNumber;
                    success = Tools.SafeInt(db.Add<ArticleZan>(model));

                    //操作记录
                    var now = DateTime.Now.ToString("yyyy-MM-dd");
                    var action = db.Single<UserAction>(x => x.CreateUserNumber == user.Number && x.CreateTimeText == now && x.ActionType == Enum_ActionType.Like);

                    LogHelper.ErrorLoger.Error(JsonConvert.SerializeObject(action));

                    if (action == null)
                    {
                        action = new UserAction();
                        action.CreateUserNumber = user.Number;
                        action.ActionType = Enum_ActionType.Like;
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
                else
                {
                    success = db.Delete<ArticleZan>(model.ID);
                }
                if (success > 0)
                {
                    if (goods < 0)
                    {
                        goods = 0;
                    }
                    new SubSonic.Query.Update<Article>(provider).Set("Goods").EqualTo(goods).Where<Article>(x => x.ID == articleID).Execute();
                    result.result = true;
                    result.message = isadd + "|" + goods;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Zan_ArticleZanEdit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 评论点赞编辑
        /// </summary>
        [HttpGet]
        [Route("Api/Zan/CommentZanEdit")]
        public string CommentZanEdit()
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
                var number = ZNRequest.GetString("CommentNumber");
                if (string.IsNullOrWhiteSpace(number))
                {
                    result.message = "评论信息异常";
                    return JsonConvert.SerializeObject(result);
                }

                Comment comment = new SubSonic.Query.Select(provider, "ID", "CreateUserNumber", "Goods", "Number").From<Comment>().Where<Comment>(x => x.Number == number).ExecuteSingle<Comment>();
                if (comment == null)
                {
                    result.message = "评论信息异常";
                    return JsonConvert.SerializeObject(result);
                }

                //判断是否拉黑
                var black = db.Exists<Black>(x => x.CreateUserNumber == comment.CreateUserNumber && x.ToUserNumber == user.Number);
                if (black)
                {
                    result.code = Enum_ErrorCode.NoPower;
                    result.message = "没有权限";
                    return JsonConvert.SerializeObject(result);
                }
                var success = 0;
                var model = db.Single<CommentZan>(x => x.CreateUserNumber == user.Number && x.CommentNumber == number);
                var goods = model == null ? comment.Goods + 1 : comment.Goods - 1;

                //是否新增
                var isadd = model == null ? 0 : 1;
                if (model == null)
                {
                    model = new CommentZan();
                    model.CreateDate = DateTime.Now;
                    model.CreateUserNumber = user.Number;
                    model.CreateIP = Tools.GetClientIP;
                    model.CommentNumber = number;
                    success = Tools.SafeInt(db.Add<CommentZan>(model));
                }
                else
                {
                    success = db.Delete<CommentZan>(model.ID);
                }
                if (success > 0)
                {
                    if (goods < 0)
                    {
                        goods = 0;
                    }
                    new SubSonic.Query.Update<Comment>(provider).Set("Goods").EqualTo(goods).Where<Comment>(x => x.ID == comment.ID).Execute();
                    result.result = true;
                    result.message = isadd + "|" + goods;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Zan_CommentZanEdit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 点赞我
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Zan/ToMe")]
        public string ToMe()
        {
            ApiResult result = new ApiResult();
            try
            {
                var UserNumber = ZNRequest.GetString("UserNumber");
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<ArticleZan>().Where<ArticleZan>(x => x.ArticleUserNumber == UserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<ArticleZan>();

                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Cover", "ArticlePower", "CreateUserNumber", "Status", "Title").From<Article>().Where("Number").In(list.Select(x => x.ArticleNumber).ToArray()).ExecuteTypedList<Article>();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number", "Cover").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).ToArray()).ExecuteTypedList<User>();

                List<ZanJson> newlist = new List<ZanJson>();
                list.ForEach(x =>
                {
                    var article = articles.FirstOrDefault(y => y.Number == x.ArticleNumber);
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (article != null && user != null)
                    {
                        ZanJson model = new ZanJson();
                        model.ID = x.ID;
                        model.CreateDate = x.CreateDate.ToString("yyyy-MM-dd");
                        model.ArticleID = article.ID;
                        model.Title = article.Title;
                        model.Number = article.Number;
                        model.Cover = article.Cover;
                        model.ArticlePower = article.ArticlePower;
                        model.CreateUserNumber = article.CreateUserNumber;
                        model.NickName = user.NickName;
                        model.Avatar = user.Avatar;
                        model.UserNumber = user.Number;
                        model.UserCover = user.Cover;
                        newlist.Add(model);
                    }
                });
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
                LogHelper.ErrorLoger.Error("Api_Zan_ToMe:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 我点赞
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Zan/ToUser")]
        public string ToUser()
        {
            ApiResult result = new ApiResult();
            try
            {
                var UserNumber = ZNRequest.GetString("UserNumber");
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<ArticleZan>().Where<ArticleZan>(x => x.CreateUserNumber == UserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<ArticleZan>();

                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Cover", "ArticlePower", "CreateUserNumber", "Status", "Title").From<Article>().Where("Number").In(list.Select(x => x.ArticleNumber).ToArray()).ExecuteTypedList<Article>();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number", "Cover").From<User>().Where("Number").In(list.Select(x => x.ArticleUserNumber).ToArray()).ExecuteTypedList<User>();

                List<ZanJson> newlist = new List<ZanJson>();
                list.ForEach(x =>
                {
                    var article = articles.FirstOrDefault(y => y.Number == x.ArticleNumber);
                    var user = users.FirstOrDefault(y => y.Number == x.ArticleUserNumber);
                    if (article != null && user != null)
                    {
                        ZanJson model = new ZanJson();
                        model.ID = x.ID;
                        model.CreateDate = x.CreateDate.ToString("yyyy-MM-dd");
                        model.ArticleID = article.ID;
                        model.Title = article.Title;
                        model.Number = article.Number;
                        model.Cover = article.Cover;
                        model.ArticlePower = article.ArticlePower;
                        model.CreateUserNumber = article.CreateUserNumber;
                        model.NickName = user.NickName;
                        model.Avatar = user.Avatar;
                        model.UserNumber = user.Number;
                        model.UserCover = user.Cover;
                        newlist.Add(model);
                    }
                });
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
                LogHelper.ErrorLoger.Error("Api_Zan_ToUser:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}