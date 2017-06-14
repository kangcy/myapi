using EGT_OTA.Helper;
using EGT_OTA.Models;
using SubSonic.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGT_OTA.Controllers
{
    public class AppHelper
    {
        public static readonly SimpleRepository db = BasicRepository.GetRepo();
        public static readonly SimpleRepository logdb = LogRepository.GetRepo();

        #region  推送

        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="usernumber">推送目标用户编号</param>
        /// <param name="id">对象ID（文章ID）</param>
        /// <param name="number">对象Number</param>
        /// <param name="name">用户名称/文章标题</param>
        /// <param name="pushtype">推送类型</param>
        public void Push(string usernumber, int id, string number, string name, int pushtype)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usernumber))
                {
                    return;
                }
                var user = db.Single<User>(x => x.Number == usernumber);
                if (user == null)
                {
                    return;
                }
                if (user.ShowPush == 0)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(user.ClientID))
                {
                    return;
                }
                PushHelper message = new PushHelper(new List<string>() { user.ClientID });

                var beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var endTime = DateTime.Now.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss");
                var result = "";
                switch (pushtype)
                {
                    case Enum_PushType.Article:
                        result = message.PushTemplate(Enum_Push.Single, "小微篇文章推荐啦", name, "", "", Enum_PushType.Article + "|" + id, beginTime, endTime);
                        break;
                    case Enum_PushType.Comment:
                        result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有好友评论啦，快去看看吧", "", "", Enum_PushType.Comment + "|" + id + "|" + number, beginTime, endTime);
                        break;
                    case Enum_PushType.Money:
                        result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有好友打赏啦，快去看看吧", "", "", Enum_PushType.Money.ToString(), beginTime, endTime);
                        break;
                    case Enum_PushType.Fan:
                        result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有新的粉丝啦，快去看看吧", "", "", Enum_PushType.Fan.ToString(), beginTime, endTime);
                        break;
                    case Enum_PushType.FanArticle:
                        result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有好友发文啦，快去看看吧", "", "", Enum_PushType.FanArticle + "|" + id, beginTime, endTime);
                        break;
                    case Enum_PushType.Update:
                        result = message.PushTemplate(Enum_Push.Single, "小微篇更新啦", "更新内容", "", "", "10", beginTime, endTime);
                        break;
                    default:
                        break;
                }

                //推送记录
                PushLog log = new PushLog();
                log.Number = usernumber;
                log.ObjectID = id;
                log.ObjectName = name;
                log.ObjectNumber = number;
                log.PushResult = result;
                log.PushType = pushtype;
                log.CreateTime = DateTime.Now;
                logdb.Add<PushLog>(log);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("推送失败：" + ex.Message);
            }
        }

        #endregion
    }
}