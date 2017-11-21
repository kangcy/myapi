﻿using System;
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
    public class ArticleController : BaseApiController
    {
        /// <summary>
        /// 文章详情
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Article/PcDetail")]
        public string PcDetail()
        {
            ApiResult result = new ApiResult();
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    result.code = Enum_ErrorCode.UnLogin;
                    result.message = "用户信息验证失败";
                    return JsonConvert.SerializeObject(result);
                }
                int id = ZNRequest.GetInt("ArticleID");
                Article model = db.Single<Article>(x => x.ID == id);
                if (model == null)
                {
                    model = new Article();
                    model.CreateDateText = DateTime.Now.ToString("yyyy-MM-dd");
                    model.Number = BuildNumber();
                    model.Recommend = Enum_ArticleRecommend.None;
                    model.ArticlePower = Enum_ArticlePower.Myself;
                    model.Submission = Enum_Submission.TemporaryApproved;
                }
                else
                {
                    //判断权限
                    if (user.UserRole == Enum_UserRole.Common)
                    {
                        if (user.Number != model.CreateUserNumber)
                        {
                            result.code = Enum_ErrorCode.NoPower;
                            result.message = "没有权限";
                            return JsonConvert.SerializeObject(result);
                        }
                    }

                    //浏览数
                    new SubSonic.Query.Update<Article>(provider).Set("Views").EqualTo(model.Views + 1).Where<Article>(x => x.ID == model.ID).Execute();
                    model.Pays = new SubSonic.Query.Select(provider).From<Order>().Where<Order>(x => x.ToArticleNumber == model.Number && x.Status == Enum_Status.Approved).GetRecordCount();
                    model.Keeps = new SubSonic.Query.Select(provider).From<Keep>().Where<Keep>(x => x.ArticleNumber == model.Number).GetRecordCount();
                    model.Comments = new SubSonic.Query.Select(provider).From<Comment>().Where<Comment>(x => x.ArticleNumber == model.Number).GetRecordCount();

                    //是否收藏
                    model.IsKeep = new SubSonic.Query.Select(provider, "ID").From<Keep>().Where<Keep>(x => x.CreateUserNumber == user.Number && x.ArticleNumber == model.Number).GetRecordCount() == 0 ? 0 : 1;

                    //是否关注
                    model.IsFollow = new SubSonic.Query.Select(provider, "ID").From<Fan>().Where<Fan>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == model.CreateUserNumber).GetRecordCount() == 0 ? 0 : 1;

                    //是否点赞
                    model.IsZan = new SubSonic.Query.Select(provider, "ID").From<ArticleZan>().Where<ArticleZan>(x => x.CreateUserNumber == user.Number && x.ArticleNumber == model.Number).GetRecordCount() == 0 ? 0 : 1;

                    //类型
                    ArticleType articleType = GetArticleType().FirstOrDefault<ArticleType>(x => x.ID == model.TypeID);
                    model.TypeName = articleType == null ? string.Empty : articleType.Name;

                    //文章部分
                    model.ArticlePart = db.Find<ArticlePart>(x => x.ArticleNumber == model.Number).OrderBy(x => x.SortID).ToList();

                    //漂浮装扮
                    var custom = db.Single<ArticleCustom>(x => x.ArticleNumber == model.Number);
                    if (custom != null)
                    {
                        model.Showy = custom.ShowyUrl;
                        model.MusicID = custom.MusicID;
                        model.MusicName = custom.MusicName;
                        model.MusicUrl = custom.MusicUrl;
                        model.Transparency = custom.Transparency;
                        model.MarginTop = custom.MarginTop;
                    }

                    model.CreateDateText = model.CreateDate.ToString("yyyy-MM-dd");
                }

                //创建人
                model.UserID = user.ID;
                model.NickName = user.NickName;
                model.Avatar = user.Avatar;
                model.AutoMusic = user.AutoMusic;
                model.UserCover = user.Cover;
                model.ShareNick = user.ShareNick;
                model.IsPay = user.IsPay;

                model.ShareUrl = System.Configuration.ConfigurationManager.AppSettings["share_url"] + model.Number;

                //模板配置
                model.BackgroundJson = db.Single<Background>(x => x.ArticleNumber == model.Number && x.IsUsed == Enum_Used.Approved);

                //模板预览
                var previewTemp = ZNRequest.GetInt("Template", -1);
                if (previewTemp >= 0)
                {
                    model.TemplateJson = GetArticleTemplate().FirstOrDefault(x => x.ID == previewTemp);
                }
                else
                {
                    if (model.Template >= 0)
                    {
                        model.TemplateJson = GetArticleTemplate().FirstOrDefault(x => x.ID == model.Template);
                    }
                }

                //主题色预览
                var previewColorTemp = ZNRequest.GetInt("ColorTemplate", 0);
                if (previewColorTemp > 0)
                {
                    model.ColorTemplateJson = GetColorTemplate().FirstOrDefault(x => x.ID == previewColorTemp);
                }
                else
                {
                    if (model.ColorTemplate > 0)
                    {
                        model.ColorTemplateJson = GetColorTemplate().FirstOrDefault(x => x.ID == model.ColorTemplate);
                    }
                }

                result.result = true;
                result.message = model;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Article_Detail:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Article/Detail")]
        public string Detail()
        {
            ApiResult result = new ApiResult();
            try
            {
                string UserNumber = ZNRequest.GetString("Number");
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var user = db.Single<User>(x => x.Number == UserNumber);
                if (user == null)
                {
                    result.code = Enum_ErrorCode.UnLogin;
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                int id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                Article model = db.Single<Article>(x => x.ID == id);
                if (model == null)
                {
                    result.message = "文章信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                if (model.Status == Enum_Status.Audit)
                {
                    result.message = "当前文章未通过审核";
                    return JsonConvert.SerializeObject(result);
                }

                //非本人
                if ((model.Status == Enum_Status.Delete || model.Status == Enum_Status.DeleteCompletely) && UserNumber != model.CreateUserNumber)
                {
                    result.code = Enum_ErrorCode.Delete;
                    result.message = "当前文章已删除";
                    return JsonConvert.SerializeObject(result);
                }

                //判断黑名单
                if (db.Exists<Black>(x => x.ToUserNumber == UserNumber && x.CreateUserNumber == model.CreateUserNumber))
                {
                    result.message = "没有访问权限";
                    return JsonConvert.SerializeObject(result);
                }

                string password = ZNRequest.GetString("ArticlePassword");

                //浏览数
                new SubSonic.Query.Update<Article>(provider).Set("Views").EqualTo(model.Views + 1).Where<Article>(x => x.ID == model.ID).Execute();
                model.Pays = new SubSonic.Query.Select(provider).From<Order>().Where<Order>(x => x.ToArticleNumber == model.Number && x.Status == Enum_Status.Approved).GetRecordCount();
                model.Keeps = new SubSonic.Query.Select(provider).From<Keep>().Where<Keep>(x => x.ArticleNumber == model.Number).GetRecordCount();
                model.Comments = new SubSonic.Query.Select(provider).From<Comment>().Where<Comment>(x => x.ArticleNumber == model.Number).GetRecordCount();

                //创建人
                User createUser = db.Single<User>(x => x.Number == model.CreateUserNumber);
                if (createUser != null)
                {
                    model.UserID = createUser.ID;
                    model.NickName = createUser.NickName;
                    model.Avatar = createUser.Avatar;
                    model.AutoMusic = createUser.AutoMusic;
                    model.UserCover = createUser.Cover;
                    model.ShareNick = createUser.ShareNick;
                    model.IsPay = createUser.IsPay;
                }


                //是否收藏
                model.IsKeep = new SubSonic.Query.Select(provider, "ID").From<Keep>().Where<Keep>(x => x.CreateUserNumber == UserNumber && x.ArticleNumber == model.Number).GetRecordCount() == 0 ? 0 : 1;

                //是否关注
                model.IsFollow = new SubSonic.Query.Select(provider, "ID").From<Fan>().Where<Fan>(x => x.CreateUserNumber == UserNumber && x.ToUserNumber == model.CreateUserNumber).GetRecordCount() == 0 ? 0 : 1;

                //是否点赞
                model.IsZan = new SubSonic.Query.Select(provider, "ID").From<ArticleZan>().Where<ArticleZan>(x => x.CreateUserNumber == UserNumber && x.ArticleNumber == model.Number).GetRecordCount() == 0 ? 0 : 1;

                //类型
                ArticleType articleType = GetArticleType().FirstOrDefault<ArticleType>(x => x.ID == model.TypeID);
                model.TypeName = articleType == null ? string.Empty : articleType.Name;

                //文章部分
                model.ArticlePart = db.Find<ArticlePart>(x => x.ArticleNumber == model.Number).OrderBy(x => x.SortID).ToList();

                model.CreateDateText = model.CreateDate.ToString("yyyy-MM-dd");
                model.ShareUrl = System.Configuration.ConfigurationManager.AppSettings["share_url"] + model.Number;

                //模板配置
                model.BackgroundJson = db.Single<Background>(x => x.ArticleNumber == model.Number && x.IsUsed == Enum_Used.Approved);

                //模板预览
                var previewTemp = ZNRequest.GetInt("Template", -1);
                if (previewTemp >= 0)
                {
                    model.TemplateJson = GetArticleTemplate().FirstOrDefault(x => x.ID == previewTemp);
                }
                else
                {
                    if (model.Template >= 0)
                    {
                        model.TemplateJson = GetArticleTemplate().FirstOrDefault(x => x.ID == model.Template);
                    }
                }

                //主题色预览
                var previewColorTemp = ZNRequest.GetInt("ColorTemplate", 0);
                if (previewColorTemp > 0)
                {
                    model.ColorTemplateJson = GetColorTemplate().FirstOrDefault(x => x.ID == previewColorTemp);
                }
                else
                {
                    if (model.ColorTemplate > 0)
                    {
                        model.ColorTemplateJson = GetColorTemplate().FirstOrDefault(x => x.ID == model.ColorTemplate);
                    }
                }

                //漂浮装扮
                var custom = db.Single<ArticleCustom>(x => x.ArticleNumber == model.Number);
                if (custom != null)
                {
                    model.Showy = custom.ShowyUrl;
                    model.MusicID = custom.MusicID;
                    model.MusicName = custom.MusicName;
                    model.MusicUrl = custom.MusicUrl;
                    model.Transparency = custom.Transparency;
                    model.MarginTop = custom.MarginTop;
                }

                result.result = true;
                result.message = model;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Article_Detail:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }


        /// <summary>
        /// 文章列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Article/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved);

                //昵称
                var title = SqlFilter(ZNRequest.GetString("Title"));
                if (!string.IsNullOrWhiteSpace(title))
                {
                    query.And("Title").Like("%" + title + "%");
                }
                var CreateUserNumber = ZNRequest.GetString("CreateUserNumber");
                if (!string.IsNullOrWhiteSpace(CreateUserNumber))
                {
                    query = query.And("CreateUserNumber").IsEqualTo(CreateUserNumber);
                }

                //其他用户的文章
                var CurrUserNumber = ZNRequest.GetString("CurrUserNumber");
                if (CreateUserNumber != CurrUserNumber || string.IsNullOrWhiteSpace(CreateUserNumber))
                {
                    query = query.And("ArticlePower").IsEqualTo(Enum_ArticlePower.Public);
                    query = query.And("Submission").IsGreaterThan(Enum_Submission.Audit);
                }

                //文章类型
                var TypeID = ZNRequest.GetInt("TypeID");
                if (TypeID > 0)
                {
                    query = query.And("TypeIDList").Like("%-0-" + TypeID.ToString() + "-%");
                }

                //过滤黑名单
                if (!string.IsNullOrWhiteSpace(CurrUserNumber))
                {
                    var black = db.Find<Black>(x => x.CreateUserNumber == CurrUserNumber);
                    if (black.Count > 0)
                    {
                        var userids = black.Select(x => x.ToUserNumber).ToArray();
                        query = query.And("CreateUserNumber").NotIn(userids);
                    }
                }
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;

                var sort = new string[] { "Recommend", "ID" };
                if (CreateUserNumber == CurrUserNumber)
                {
                    sort = new string[] { "ID" };
                }
                var Source = ZNRequest.GetString("Source");
                if (!string.IsNullOrWhiteSpace(Source))
                {
                    sort = new string[] { "Views" };
                }

                var list = query.Paged(pager.Index, pager.Size).OrderDesc(sort).ExecuteTypedList<Article>();
                List<ArticleJson> newlist = ArticleListInfo(list, CurrUserNumber);
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
                LogHelper.ErrorLoger.Error("Api_Article_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 已删除文章列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Article/DeleteAll")]
        public string DeleteAll()
        {
            ApiResult result = new ApiResult();
            try
            {
                var pager = new Pager();
                var UserNumber = ZNRequest.GetString("CreateUserNumber");
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var query = new SubSonic.Query.Select(provider).From<Article>().Where<Article>(x => x.Status == Enum_Status.Delete && x.CreateUserNumber == UserNumber);

                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc(new string[] { "UpdateDate" }).ExecuteTypedList<Article>();
                List<ArticleJson> newlist = ArticleListInfo(list, UserNumber);
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
                LogHelper.ErrorLoger.Error("Api_Article_DeleteAll:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 搜索文章
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Article/Search")]
        public string Search()
        {
            ApiResult result = new ApiResult();
            try
            {
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved && x.ArticlePower == Enum_ArticlePower.Public && x.Submission >= Enum_Submission.Approved);

                var title = SqlFilter(ZNRequest.GetString("Title"));
                if (!string.IsNullOrWhiteSpace(title))
                {
                    query.And("Title").IsNotNull().And("Title").Like("%" + title + "%");
                }

                var UserNumber = ZNRequest.GetString("UserNumber");
                if (!string.IsNullOrWhiteSpace(UserNumber))
                {
                    var black = db.Find<Black>(x => x.CreateUserNumber == UserNumber);
                    if (black.Count > 0)
                    {
                        var userids = black.Select(x => x.ToUserNumber).ToArray();
                        query = query.And("CreateUserNumber").NotIn(userids);
                    }
                }
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.result = true;
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc(new string[] { "Recommend", "Views" }).ExecuteTypedList<Article>();
                List<ArticleJson> newlist = ArticleListInfo(list, UserNumber);
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
                LogHelper.ErrorLoger.Error("Api_Article_Search:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 链接文章列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Article/LinkAll")]
        public string LinkAll()
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

                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider, "ID", "Number", "Title", "Cover", "CreateUserNumber", "CreateDate", "ArticlePower", "ArticlePowerPwd").From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved && x.Submission >= Enum_Submission.Approved);
                query = query.And("CreateUserNumber").IsEqualTo(user.Number);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;

                var sort = new string[] { "ID" };

                var list = query.Paged(pager.Index, pager.Size).OrderDesc(sort).ExecuteTypedList<Article>();

                var newlist = (from l in list
                               select new
                               {
                                   ID = l.ID,
                                   Number = l.Number,
                                   Title = l.Title,
                                   Cover = l.Cover,
                                   CreateDate = l.CreateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                                   ArticlePower = l.ArticlePower,
                                   ArticlePowerPwd = l.ArticlePowerPwd
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
                LogHelper.ErrorLoger.Error("Api_Article_LinkAll:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 链接文章列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Article/Public")]
        public string Public()
        {
            var query = new SubSonic.Query.Select(provider, "Number", "Title", "CreateDate", "ArticlePowerPwd").From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved && x.ArticlePower != Enum_ArticlePower.Myself && x.Submission != Enum_Submission.Approved);
            var list = query.ExecuteTypedList<Article>();

            var newlist = (from l in list
                           select new
                           {
                               Number = l.Number,
                               Title = l.Title,
                               ArticlePowerPwd = l.ArticlePowerPwd,
                               CreateDate = l.CreateDate.ToString("yyyy-MM-dd hh:mm:ss")
                           }).ToList();
            return Newtonsoft.Json.JsonConvert.SerializeObject(newlist);
        }
    }
}