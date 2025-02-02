﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using CommonTools;
using SubSonic.Repository;

namespace EGT_OTA.Controllers
{
    public class PushController : BaseController
    {
        public static readonly SimpleRepository logdb = LogRepository.GetRepo();

        public ActionResult Index()
        {
            var result = string.Empty;
            //try
            //{
            //    Console.OutputEncoding = Encoding.GetEncoding(936);
            //    Environment.SetEnvironmentVariable("gexin_pushList_needDetails", "true");
            //    result = PushHelper.PushMessageToApp();
            //}
            //catch (Exception ex)
            //{
            //    result = ex.Message;
            //}

            string clientId = "557e5625f84c82517457c43024b03b0c";
            PushHelper message = new PushHelper(new List<string>() { clientId });

            var beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var endTime = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss");
            //string msg1 = message.PushMessageToSingleByNotificationTemplate("XXX - 单用户", "您有新的任务，点击查看！", "", "", "", beginTime, endTime);
            //string msg2 = message.PushMessageToListByNotificationTemplate("XXX  - 多用户", "您有新的任务，点击查看！", "", "", "", beginTime, endTime);
            string msg3 = message.PushTemplate(Enum_Push.All, "XXX  - APP应用", "您有新的任务，点击查看啊！", "", "", "{id:1}", beginTime, endTime);

            //解析输出结果  
            //{"taskId":"OSS-0420_ZiFBb3Sx7A7Pz7YWMwJdD9","result":"ok","status":"successed_online"} 在线状态  
            //{"taskId":"OSS-0420_2qtgpolflJAuYGSiGTfQ04","result":"ok","status":"successed_offline"} 离线状态  

            return Content(msg3);

            //return Content(msg1 + "</br>" + msg2 + "</br>" + msg3 + "</br>");
        }


        public ActionResult Distance()
        {
            var distance = DistanceHelper.GetDistance(31.97603, 118.761916, 31.97601, 118.761916);
            return Content(distance.ToString());
        }

        /// <summary>
        /// 获取推送信息
        /// </summary>
        public ActionResult All()
        {
            var number = ZNRequest.GetString("number");
            if (string.IsNullOrWhiteSpace(number))
            {
                return Json(new { result = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
            var list = logdb.Find<PushLog>(x => x.Number == number).GroupBy(x => x.PushType).ToList();
            var newlist = (from l in list
                           select new
                           {
                               PushType = l.Key,
                               PushCount = l.Count()
                           }).ToList();
            return Json(new { result = true, message = newlist }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 清除推送信息
        /// </summary>
        public ActionResult Clear()
        {
            var number = ZNRequest.GetString("number");
            var pushtype = ZNRequest.GetInt("pushtype");
            if (string.IsNullOrWhiteSpace(number))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var list = new List<PushLog>();

            list = logdb.Find<PushLog>(x => x.PushType == pushtype && x.Number == number).ToList();
            logdb.DeleteMany<PushLog>(list);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
