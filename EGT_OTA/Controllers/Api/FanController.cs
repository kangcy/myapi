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
    public class FanController : BaseApiController
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [HttpGet]
        [Route("Api/Fan/Edit")]
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
                var number = ZNRequest.GetString("ToUserNumber");
                if (string.IsNullOrWhiteSpace(number))
                {
                    result.message = "信息异常,请刷新重试";
                    return JsonConvert.SerializeObject(result);
                }
                Fan model = db.Single<Fan>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == number);
                if (model == null)
                {
                    model = new Fan();
                    model.CreateUserNumber = user.Number;
                    model.ToUserNumber = number;
                    model.CreateDate = DateTime.Now;
                    model.CreateIP = Tools.GetClientIP;
                    var success = Tools.SafeInt(db.Add<Fan>(model)) > 0;
                    if (success)
                    {
                        user.Follows = db.Find<Fan>(x => x.CreateUserNumber == user.Number).Count;
                        result.result = true;
                        result.message = user.Follows;

                        //推送
                        new AppHelper().Push(number, 0, "", user.NickName, Enum_PushType.Fan);
                    }
                }
                else
                {
                    user.Follows = db.Find<Fan>(x => x.CreateUserNumber == user.Number).Count;
                    result.result = true;
                    result.message = user.Follows;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Fan_Edit:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpGet]
        [Route("Api/Fan/Delete")]
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
                var ToUserNumber = ZNRequest.GetString("ToUserNumber");
                var model = db.Single<Fan>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == ToUserNumber);
                if (model == null)
                {
                    result.message = "信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                if (model.CreateUserNumber != user.Number)
                {
                    result.message = "没有权限";
                    return JsonConvert.SerializeObject(result);
                }
                var success = db.Delete<Fan>(model.ID) > 0;
                if (success)
                {
                    result.result = true;
                    result.message = "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Fan_Delete:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 关注列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Fan/FollowsAll")]
        public string FollowsAll()
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
                var query = new SubSonic.Query.Select(provider).From<Fan>().Where<Fan>(x => x.CreateUserNumber == CreateUserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Fan>();
                var array = list.Select(x => x.ToUserNumber).Distinct().ToList();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Cover", "Signature", "Number").From<User>().Where<User>(x => x.Status == Enum_Status.Approved).And("Number").In(array.ToArray()).ExecuteTypedList<User>();

                var newlist = (from l in list
                               join u in users on l.ToUserNumber equals u.Number
                               select new
                               {
                                   ID = l.ID,
                                   CreateDate = FormatTime(l.CreateDate),
                                   UserID = u.ID,
                                   Cover = u.Cover,
                                   NickName = u.NickName,
                                   Signature = u.Signature,
                                   Avatar = u.Avatar,
                                   Number = u.Number
                               }).ToList();
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
                LogHelper.ErrorLoger.Error("Api_Fan_FollowsAll:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 粉丝列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Fan/FansAll")]
        public string FansAll()
        {
            ApiResult result = new ApiResult();
            try
            {
                var ToUserNumber = ZNRequest.GetString("ToUserNumber");
                if (string.IsNullOrWhiteSpace(ToUserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<Fan>().Where<Fan>(x => x.ToUserNumber == ToUserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Fan>();
                var array = list.Select(x => x.CreateUserNumber).Distinct().ToList();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Cover", "Signature", "Number").From<User>().Where<User>(x => x.Status == Enum_Status.Approved).And("Number").In(array.ToArray()).ExecuteTypedList<User>();
                var follows = db.Find<Fan>(x => x.CreateUserNumber == ToUserNumber).ToList();

                var newlist = (from l in list
                               join u in users on l.CreateUserNumber equals u.Number
                               select new
                               {
                                   ID = l.ID,
                                   CreateDate = FormatTime(l.CreateDate),
                                   UserID = u.ID,
                                   Cover = u.Cover,
                                   NickName = u.NickName,
                                   Signature = u.Signature,
                                   Avatar = u.Avatar,
                                   Number = u.Number,
                                   IsFollow = follows.Exists(x => x.ToUserNumber == u.Number) ? 1 : 0
                               }).ToList();
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
                LogHelper.ErrorLoger.Error("Api_Fan_FansAll:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 关注用户文章
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Fan/Article")]
        public string Article()
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

                var fans = db.Find<Fan>(x => x.CreateUserNumber == UserNumber).ToList();
                if (fans.Count == 0)
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved);
                query = query.And("ArticlePower").In(new int[] { Enum_ArticlePower.Public, Enum_ArticlePower.Password });
                query.And("CreateUserNumber").In(fans.Select(x => x.ToUserNumber).ToArray());
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc(new string[] { "ID" }).ExecuteTypedList<Article>();
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
                LogHelper.ErrorLoger.Error("Api_Fan_Article:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}