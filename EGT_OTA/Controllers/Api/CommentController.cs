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
    public class CommentController : BaseApiController
    {
        /// <summary>
        /// 详情
        /// </summary>
        [HttpGet]
        [Route("Api/Comment/Detail")]
        public string Detail()
        {
            ApiResult result = new ApiResult();
            try
            {
                var id = ZNRequest.GetInt("ID");
                if (id <= 0)
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var model = db.Single<Comment>(x => x.ID == id);
                if (model == null)
                {
                    result.message = "数据异常";
                    return JsonConvert.SerializeObject(result);
                }
                var user = db.Single<User>(x => x.Number == model.CreateUserNumber);
                model.UserID = user == null ? 0 : user.ID;
                model.UserNumber = user == null ? "" : user.Number;
                model.NickName = user == null ? "" : user.NickName;
                model.Avatar = user == null ? "" : user.Avatar;
                model.ArticleID = model.ArticleID;
                model.CreateDateText = FormatTime(model.CreateDate);
                result.result = true;
                result.message = model;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Comment_Detail:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 评论编辑
        /// </summary>
        [HttpGet]
        [Route("Api/Comment/Edit")]
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
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                if (string.IsNullOrWhiteSpace(ArticleNumber))
                {
                    result.message = "文章信息异常";
                    return JsonConvert.SerializeObject(result);
                }

                var summary = SqlFilter(ZNRequest.GetString("Summary"), false, false);
                if (string.IsNullOrWhiteSpace(summary))
                {
                    result.message = "请填写评论内容";
                    return JsonConvert.SerializeObject(result);
                }
                summary = CutString(summary, 2000);
                var dirtyword = HasDirtyWord(summary);
                if (!string.IsNullOrWhiteSpace(dirtyword))
                {
                    result.message = "您的输入内容含有敏感内容[" + dirtyword + "]，请检查后重试哦";
                    return JsonConvert.SerializeObject(result);
                }

                Article article = new SubSonic.Query.Select(provider, "ID", "Number", "CreateUserNumber").From<Article>().Where<Article>(x => x.Number == ArticleNumber).ExecuteSingle<Article>();
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

                Comment model = new Comment();
                model.ArticleNumber = article.Number;
                model.ArticleUserNumber = article.CreateUserNumber;
                model.Summary = summary;
                model.Number = BuildNumber();
                model.CreateDate = DateTime.Now;
                model.CreateUserNumber = user.Number;
                model.CreateIP = Tools.GetClientIP;
                model.ParentCommentNumber = ZNRequest.GetString("ParentCommentNumber");
                model.ParentUserNumber = ZNRequest.GetString("ParentUserNumber");

                model.Province = ZNRequest.GetString("Province");
                model.City = ZNRequest.GetString("City");
                model.District = ZNRequest.GetString("District");
                model.Street = ZNRequest.GetString("Street");
                model.DetailName = ZNRequest.GetString("DetailName");
                model.CityCode = ZNRequest.GetString("CityCode");
                model.Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                model.Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                model.ShowPosition = ZNRequest.GetInt("ShowPosition");
                model.ID = Tools.SafeInt(db.Add<Comment>(model));
                if (model.ID > 0)
                {
                    result.result = true;
                    result.message = model.ID;

                    //推送
                    new AppHelper().Push(article.CreateUserNumber, article.ID, article.Number, user.NickName, Enum_PushType.Comment, false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Comment_Edit:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 评论列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Comment/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var pager = new Pager();
                var id = ZNRequest.GetInt("NewId");
                var ArticleUserNumber = ZNRequest.GetString("ArticleUserNumber");
                if (string.IsNullOrWhiteSpace(ArticleUserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var query = new SubSonic.Query.Select(provider).From<Comment>().Where<Comment>(x => x.ArticleUserNumber == ArticleUserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                if (recordCount == 1 && id > 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                query = query.And("ID").IsNotEqualTo(id);

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Comment>();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).ToArray()).ExecuteTypedList<User>();
                var parentComments = new SubSonic.Query.Select(provider, "ID", "ParentCommentNumber", "Number", "CreateUserNumber").From<Comment>().Where("ParentCommentNumber").In(list.Select(x => x.Number).ToArray()).ExecuteTypedList<Comment>();
                var zans = db.Find<CommentZan>(x => x.CreateUserNumber == ArticleUserNumber).ToList();
                List<CommentJson> newlist = new List<CommentJson>();
                list.ForEach(x =>
                {
                    CommentJson model = new CommentJson();
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (user == null)
                    {
                        return;
                    }
                    model.ID = x.ID;
                    model.Summary = x.Summary;
                    model.Goods = x.Goods;
                    model.Number = x.Number;
                    model.CreateDateText = FormatTime(x.CreateDate);
                    model.ShowPosition = x.ShowPosition;
                    model.City = x.City;
                    if (string.IsNullOrWhiteSpace(model.City))
                    {
                        model.ShowPosition = 0;
                    }
                    model.UserID = user.ID;
                    model.UserNumber = user.Number;
                    model.NickName = user.NickName;
                    model.Avatar = user.Avatar;
                    model.SubCommentCount = parentComments.Count(y => y.ParentCommentNumber == x.Number);
                    if (model.SubCommentCount == 1)
                    {
                        var subuser = db.Single<User>(y => y.Number == parentComments[0].CreateUserNumber);
                        var comment = db.Single<Comment>(y => y.Number == parentComments[0].Number);
                        if (subuser == null && comment == null)
                        {
                            model.SubCommentCount = 0;
                        }
                        if (subuser != null)
                        {
                            model.SubUserName = subuser.NickName;
                        }
                        if (comment != null)
                        {
                            model.SubSummary = comment.Summary;
                        }
                    }
                    model.ArticleNumber = x.ArticleNumber;
                    model.IsZan = zans.Count(y => y.CommentNumber == x.Number);
                    newlist.Add(model);
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
                LogHelper.ErrorLoger.Error("Api_Comment_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 评论列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Comment/All_1_3")]
        public string All_1_3()
        {
            ApiResult result = new ApiResult();
            try
            {
                var pager = new Pager();
                var id = ZNRequest.GetInt("NewId");
                var ArticleUserNumber = ZNRequest.GetString("ArticleUserNumber");
                if (string.IsNullOrWhiteSpace(ArticleUserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var query = new SubSonic.Query.Select(provider).From<Comment>().Where<Comment>(x => (x.ArticleUserNumber == ArticleUserNumber && x.ArticleUserNumber == "") || x.ParentUserNumber == ArticleUserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                if (recordCount == 1 && id > 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                query = query.And("ID").IsNotEqualTo(id);

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Comment>();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).ToArray()).ExecuteTypedList<User>();
                var parentComments = new SubSonic.Query.Select(provider, "ID", "ParentCommentNumber", "Number", "CreateUserNumber").From<Comment>().Where("ParentCommentNumber").In(list.Select(x => x.Number).ToArray()).ExecuteTypedList<Comment>();
                var zans = db.Find<CommentZan>(x => x.CreateUserNumber == ArticleUserNumber).ToList();
                List<CommentJson> newlist = new List<CommentJson>();
                list.ForEach(x =>
                {
                    CommentJson model = new CommentJson();
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (user == null)
                    {
                        return;
                    }
                    model.ID = x.ID;
                    model.Summary = x.Summary;
                    model.Goods = x.Goods;
                    model.Number = x.Number;
                    model.CreateDateText = FormatTime(x.CreateDate);
                    model.ShowPosition = x.ShowPosition;
                    model.City = x.City;
                    if (string.IsNullOrWhiteSpace(model.City))
                    {
                        model.ShowPosition = 0;
                    }
                    model.UserID = user.ID;
                    model.UserNumber = user.Number;
                    model.NickName = user.NickName;
                    model.Avatar = user.Avatar;
                    model.SubCommentCount = parentComments.Count(y => y.ParentCommentNumber == x.Number);
                    if (model.SubCommentCount == 1)
                    {
                        var subuser = db.Single<User>(y => y.Number == parentComments[0].CreateUserNumber);
                        var comment = db.Single<Comment>(y => y.Number == parentComments[0].Number);
                        if (subuser == null && comment == null)
                        {
                            model.SubCommentCount = 0;
                        }
                        if (subuser != null)
                        {
                            model.SubUserName = subuser.NickName;
                        }
                        if (comment != null)
                        {
                            model.SubSummary = comment.Summary;
                        }
                    }
                    model.ArticleNumber = x.ArticleNumber;
                    model.IsZan = zans.Count(y => y.CommentNumber == x.Number);
                    newlist.Add(model);
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
                LogHelper.ErrorLoger.Error("Api_Comment_All_1_3:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 文章评论
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Comment/ArticleComment")]
        public string ArticleComment()
        {
            ApiResult result = new ApiResult();
            try
            {
                var pager = new Pager();
                var id = ZNRequest.GetInt("NewId");
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                if (string.IsNullOrWhiteSpace(ArticleNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var UserNumber = ZNRequest.GetString("UserNumber");

                var query = new SubSonic.Query.Select(provider).From<Comment>().Where<Comment>(x => x.ArticleNumber == ArticleNumber && x.ParentCommentNumber == "");
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                if (recordCount == 1 && id > 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                query = query.And("ID").IsNotEqualTo(id);

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;

                var isNew = ZNRequest.GetInt("New");
                var list = new List<Comment>();
                if (isNew > 0)
                {
                    list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Comment>();
                }
                else
                {
                    list = query.Paged(pager.Index, pager.Size).OrderAsc("ID").ExecuteTypedList<Comment>();
                }
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).Distinct().ToArray()).ExecuteTypedList<User>();
                var parentComments = new SubSonic.Query.Select(provider, "ID", "ParentCommentNumber", "Number", "CreateUserNumber").From<Comment>().Where("ParentCommentNumber").In(list.Select(x => x.Number).ToArray()).ExecuteTypedList<Comment>();
                var zans = db.Find<CommentZan>(x => x.CreateUserNumber == UserNumber).ToList();
                List<CommentJson> newlist = new List<CommentJson>();
                list.ForEach(x =>
                {
                    CommentJson model = new CommentJson();
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (user == null)
                    {
                        return;
                    }
                    model.ID = x.ID;
                    model.Summary = x.Summary;
                    model.Goods = x.Goods;
                    model.Number = x.Number;
                    model.CreateDateText = FormatTime(x.CreateDate);
                    model.ShowPosition = x.ShowPosition;
                    model.City = x.City;
                    if (string.IsNullOrWhiteSpace(model.City))
                    {
                        model.ShowPosition = 0;
                    }
                    model.UserID = user.ID;
                    model.UserNumber = user.Number;
                    model.NickName = user.NickName;
                    model.Avatar = user.Avatar;
                    model.SubCommentCount = parentComments.Count(y => y.ParentCommentNumber == x.Number);
                    if (model.SubCommentCount == 1)
                    {
                        var subuser = db.Single<User>(y => y.Number == parentComments[0].CreateUserNumber);
                        var comment = db.Single<Comment>(y => y.Number == parentComments[0].Number);
                        if (subuser == null && comment == null)
                        {
                            model.SubCommentCount = 0;
                        }
                        if (subuser != null)
                        {
                            model.SubUserName = subuser.NickName;
                        }
                        if (comment != null)
                        {
                            model.SubSummary = comment.Summary;
                        }
                    }
                    model.ArticleNumber = x.ArticleNumber;
                    model.IsZan = zans.Count(y => y.CommentNumber == x.Number);
                    newlist.Add(model);
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
                LogHelper.ErrorLoger.Error("Api_ArticleComment_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 文章评论回复
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Comment/SubComment")]
        public string SubComment()
        {
            ApiResult result = new ApiResult();
            try
            {
                var pager = new Pager();
                var Number = ZNRequest.GetString("Number");
                if (string.IsNullOrWhiteSpace(Number))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var UserNumber = ZNRequest.GetString("UserNumber");

                var query = new SubSonic.Query.Select(provider).From<Comment>().Where<Comment>(x => x.ParentCommentNumber == Number);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;

                var list = query.Paged(pager.Index, pager.Size).OrderAsc("ID").ExecuteTypedList<Comment>();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).Distinct().ToArray()).ExecuteTypedList<User>();
                var parentComments = new SubSonic.Query.Select(provider, "ID", "ParentCommentNumber", "Number", "CreateUserNumber").From<Comment>().Where("ParentCommentNumber").In(list.Select(x => x.Number).ToArray()).ExecuteTypedList<Comment>();
                var zans = db.Find<CommentZan>(x => x.CreateUserNumber == UserNumber).ToList();
                List<CommentJson> newlist = new List<CommentJson>();
                list.ForEach(x =>
                {
                    CommentJson model = new CommentJson();
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (user == null)
                    {
                        return;
                    }
                    model.ID = x.ID;
                    model.Summary = x.Summary;
                    model.Goods = x.Goods;
                    model.Number = x.Number;
                    model.CreateDateText = x.CreateDate.ToString("yyyy-MM-dd");
                    model.ShowPosition = x.ShowPosition;
                    model.City = x.City;
                    if (string.IsNullOrWhiteSpace(model.City))
                    {
                        model.ShowPosition = 0;
                    }
                    model.UserID = user.ID;
                    model.UserNumber = user.Number;
                    model.NickName = user.NickName;
                    model.Avatar = user.Avatar;
                    model.SubCommentCount = parentComments.Count(y => y.ParentCommentNumber == x.Number);
                    if (model.SubCommentCount == 1)
                    {
                        var subuser = db.Single<User>(y => y.Number == parentComments[0].CreateUserNumber);
                        var comment = db.Single<Comment>(y => y.Number == parentComments[0].Number);
                        if (subuser == null && comment == null)
                        {
                            model.SubCommentCount = 0;
                        }
                        if (subuser != null)
                        {
                            model.SubUserName = subuser.NickName;
                        }
                        if (comment != null)
                        {
                            model.SubSummary = comment.Summary;
                        }
                    }
                    model.ArticleNumber = x.ArticleNumber;
                    model.IsZan = zans.Count(y => y.CommentNumber == x.Number);
                    newlist.Add(model);
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
                LogHelper.ErrorLoger.Error("Api_Comment_SubComment:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}