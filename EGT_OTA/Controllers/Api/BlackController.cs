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
    public class BlackController : BaseApiController
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [HttpGet]
        [Route("Api/Black/Edit")]
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
                var ToUserNumber = ZNRequest.GetString("ToUserNumber");
                if (string.IsNullOrWhiteSpace(ToUserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var exist = db.Exists<Black>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == ToUserNumber);
                if (exist)
                {
                    user.Follows = db.Find<Fan>(x => x.CreateUserNumber == user.Number).Count;

                    result.result = true;
                    result.message = user.Follows;
                }
                Black model = new Black();
                model.ToUserNumber = ToUserNumber;
                model.CreateDate = DateTime.Now;
                model.CreateUserNumber = user.Number;
                model.CreateIP = Tools.GetClientIP;
                var success = Tools.SafeInt(db.Add<Black>(model)) > 0;
                if (success)
                {
                    //取消关注
                    var fan = db.Single<Fan>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == ToUserNumber);
                    if (fan != null)
                    {
                        db.Delete<Fan>(fan.ID);
                    }

                    user.Follows = db.Find<Fan>(x => x.CreateUserNumber == user.Number).Count;

                    result.result = true;
                    result.message = user.Follows;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Black_Edit:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpGet]
        [Route("Api/Black/Delete")]
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
                if (string.IsNullOrWhiteSpace(ToUserNumber))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var model = db.Single<Black>(x => x.CreateUserNumber == user.Number && x.ToUserNumber == ToUserNumber);
                if (model == null)
                {
                    result.message = "信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                var success = db.Delete<Black>(model.ID) > 0;
                if (success)
                {
                    result.result = true;
                    result.message = "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Black_Delete:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 黑名单列表
        /// </summary>
        [HttpGet]
        [Route("Api/Black/All")]
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
                var query = new SubSonic.Query.Select(provider).From<Black>().Where<Black>(x => x.CreateUserNumber == CreateUserNumber);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var list = query.OrderDesc("ID").ExecuteTypedList<Black>();
                var array = list.Select(x => x.ToUserNumber).ToArray();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Signature", "Number", "Cover").From<User>().Where("Number").In(array).ExecuteTypedList<User>();

                var newlist = (from b in list
                               join u in users on b.ToUserNumber equals u.Number
                               select new BlackJson
                               {
                                   ID = b.ID,
                                   CreateDate = b.CreateDate.ToString("yyyy-MM-dd"),
                                   UserID = u.ID,
                                   Number = u.Number,
                                   NickName = u.NickName,
                                   Avatar = u.Avatar,
                                   Signature = u.Signature,
                                   Cover = u.Cover
                               }).ToList();
                result.result = true;
                result.message = new
                {
                    currpage = 1,
                    records = newlist.Count,
                    totalpage = 1,
                    list = newlist
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Black_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}