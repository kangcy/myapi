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
    public class PushController : BaseApiController
    {
        /// <summary>
        /// 推送全部
        /// </summary>
        [HttpGet]
        [Route("Api/Push/ALL")]
        public string ALL()
        {
            ApiResult result = new ApiResult();
            try
            {
                var title = ZNRequest.GetString("Title");
                var text = ZNRequest.GetString("Text");

                string clientId = "557e5625f84c82517457c43024b03b0c";
                PushHelper message = new PushHelper(new List<string>() { clientId });

                var beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss");
                //string msg1 = message.PushMessageToSingleByNotificationTemplate("XXX - 单用户", "您有新的任务，点击查看！", "", "", "", beginTime, endTime);
                //string msg2 = message.PushMessageToListByNotificationTemplate("XXX  - 多用户", "您有新的任务，点击查看！", "", "", "", beginTime, endTime);
                string msg3 = message.PushTemplate(Enum_Push.All, "XXX  - APP应用", "您有新的任务，点击查看啊！", "", "", "{id:1}", beginTime, endTime);

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_UserLog_Edit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 推送单个
        /// </summary>
        [HttpGet]
        [Route("Api/Push/Single")]
        public string Single()
        {
            ApiResult result = new ApiResult();
            try
            {
                //通知用户
                var number = ZNRequest.GetString("UserNumber");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return "参数异常";
                }
                var user = db.Single<User>(x => x.Number == number);
                if (user == null)
                {
                    return "用户不存在";
                }
                if (user.ShowPush == 0)
                {
                    return "未启用消息推送";
                }
                if (string.IsNullOrWhiteSpace(user.ClientID))
                {
                    return "设备号不存在";
                }
                PushHelper message = new PushHelper(new List<string>() { user.ClientID });

                var article = db.Find<Article>(x => x.Status == Enum_Status.Approved && x.ArticlePower == Enum_ArticlePower.Public);
                var num = new Random().Next(0, article.Count);

                var beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
                result.message = message.PushTemplate(Enum_Push.Single, "微篇文章推荐啦", article[num].Title, "", "", "0|" + article[num].ID, beginTime, endTime);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_UserLog_Edit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 系统推送
        /// </summary>
        [HttpGet]
        [Route("Api/Push/System")]
        public string System()
        {
            ApiResult result = new ApiResult();
            try
            {
                var title = ZNRequest.GetString("Title");
                var text = ZNRequest.GetString("Text");

                string clientId = "557e5625f84c82517457c43024b03b0c";
                //string clientId = "54f3c1dc7795e813a8e0cdb039becb6f";
                PushHelper message = new PushHelper(new List<string>() { clientId });


                var article = db.Find<Article>(x => x.Status == Enum_Status.Approved && x.ArticlePower == Enum_ArticlePower.Public);
                var num = new Random().Next(0, article.Count);

                var beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
                string msg1 = message.PushTemplate(Enum_Push.Single, "微篇文章推荐啦", article[num].Title, "", "", "0|" + article[num].ID, beginTime, endTime);
                //string msg2 = message.PushTemplate(Enum_Push.Multiple, "用户", "您有新的任务，点击查看！", "", "", "", beginTime, endTime);
                //string msg3 = message.PushTemplate(Enum_Push.All, "XXX  - APP应用", "您有新的任务，点击查看啊！", "", "", "{id:1}", beginTime, endTime);
                result.message = msg1;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_UserLog_Edit" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}