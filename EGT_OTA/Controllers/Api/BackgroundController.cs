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
    public class BackgroundController : BaseApiController
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [HttpGet]
        [Route("Api/Background/Edit")]
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
                var number = ZNRequest.GetString("ArticleNumber");
                var url = ZNRequest.GetString("Url");
                if (string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(url))
                {
                    result.message = "信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                var id = ZNRequest.GetInt("EditID");
                Background model = new Background();
                if (id > 0)
                {
                    model = db.Single<Background>(x => x.ID == id);
                }
                else
                {
                    model.Number = BuildNumber();
                    model.ArticleNumber = number;
                    model.CreateUserNumber = user.Number;
                    model.IsUsed = Enum_Used.Approved;
                }
                model.Full = ZNRequest.GetInt("Full");
                model.High = ZNRequest.GetInt("High");
                model.Transparency = ZNRequest.GetInt("Transparency");
                model.Url = url;
                var success = false;
                if (id == 0)
                {
                    success = Tools.SafeInt(db.Add<Background>(model)) > 0;
                    if (success)
                    {
                        //取消启用
                        var list = db.Find<Background>(x => x.ArticleNumber == number && x.Number != model.Number && x.IsUsed == Enum_Used.Approved).ToList();
                        if (list.Count > 0)
                        {
                            list.ForEach(x =>
                            {
                                x.IsUsed = Enum_Used.Audit;
                            });
                            db.UpdateMany<Background>(list);
                        }
                    }
                }
                else
                {
                    success = db.Update<Background>(model) > 0;
                }
                if (success)
                {
                    result.result = true;
                    result.message = model.Number;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Background_Edit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 详情
        /// </summary>
        [HttpGet]
        [Route("Api/Background/Info")]
        public string Info()
        {
            ApiResult result = new ApiResult();
            try
            {
                var number = ZNRequest.GetString("Number");
                if (string.IsNullOrWhiteSpace(number))
                {
                    result.message = "信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                Background model = db.Single<Background>(x => x.Number == number);
                if (model == null)
                {
                    model = new Background();
                }

                result.result = true;
                result.message = model;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Background_Info" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpGet]
        [Route("Api/Background/Delete")]
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
                var number = ZNRequest.GetString("Number");
                var model = db.Single<Background>(x => x.Number == number);
                if (model == null)
                {
                    result.message = "背景信息验证失败";
                    return JsonConvert.SerializeObject(result);
                }
                if (model.CreateUserNumber != user.Number)
                {
                    result.code = Enum_ErrorCode.NoPower;
                    result.message = "没有权限";
                    return JsonConvert.SerializeObject(result);
                }
                var success = db.Delete<Background>(model.ID) > 0;
                if (success)
                {
                    result.result = true;
                    result.message = model;
                }
                else
                {
                    result.message = "删除失败";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Background_Delete:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 使用
        /// </summary>
        [HttpGet]
        [Route("Api/Background/Used")]
        public string Used()
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
                var number = ZNRequest.GetString("Number");
                if (string.IsNullOrWhiteSpace(number))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var model = db.Single<Background>(x => x.Number == number);
                if (model == null)
                {
                    result.message = "信息异常";
                    return JsonConvert.SerializeObject(result);
                }
                model.IsUsed = Enum_Used.Approved;
                var success = db.Update<Background>(model) > 0;
                if (success)
                {
                    //取消启用
                    var list = db.Find<Background>(x => x.ArticleNumber == model.ArticleNumber && x.Number != model.Number && x.IsUsed == Enum_Used.Approved).ToList();
                    if (list.Count > 0)
                    {
                        list.ForEach(x =>
                        {
                            x.IsUsed = Enum_Used.Audit;
                        });
                        db.UpdateMany<Background>(list);
                    }
                    result.result = true;
                    result.message = model.Number;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Background_Edit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 列表
        /// </summary>
        [HttpGet]
        [Route("Api/Background/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var list = new List<Background>();
                var number = ZNRequest.GetString("ArticleNumber");
                if (!string.IsNullOrWhiteSpace(number))
                {
                    list = db.Find<Background>(x => x.ArticleNumber == number).OrderByDescending(x => x.IsUsed).ToList();
                }
                result.result = true;
                result.message = list;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Background_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}