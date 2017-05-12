using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using EGT_OTA.Models;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 1.PushMessageToSingle接口：支持对单个用户进行推送  
    /// 2.PushMessageToList接口：支持对多个用户进行推送，建议为50个用户  
    /// 3.PushMessageToApp接口：对单个应用下的所有用户进行推送，可根据省份，标签，机型过滤推送  
    /// </summary>
    public class PushHelper
    {
        /// <summary>  
        /// 构造函数中传入移动客户端的CLIENTID号  
        /// </summary>  
        /// <param name="clientId">获取的clientID </param>  
        public PushHelper(List<string> clientIdList)
        {
            Environment.SetEnvironmentVariable("needDetails", "true");
            CLIENTID = clientIdList;
        }

        #region  应用基本参数信息

        public string APPID = System.Web.Configuration.WebConfigurationManager.AppSettings["APPID"];
        public string APPKEY = System.Web.Configuration.WebConfigurationManager.AppSettings["APPKEY"];
        public string MASTERSECRET = System.Web.Configuration.WebConfigurationManager.AppSettings["MASTERSECRET"];
        public List<string> CLIENTID = new List<string>();
        public static string HOST = "http://sdk.open.api.igexin.com/apiex.htm";
        public static string DeviceToken = "";//填写IOS系统的DeviceToken 

        #endregion

        /// <summary>  
        /// 透传模板  
        /// </summary>  
        /// <param name="transContent">透传内容</param>  
        /// <param name="beginTM">客户端展示开始时间</param>  
        /// <param name="endTM">客户端展示结束时间</param>  
        public string PushTemplate(int pushType, string transContent, string beginTM, string endTM)
        {
            TransmissionTemplate template = TransmissionTemplate(transContent, beginTM, endTM);
            switch (pushType)
            {
                case Enum_Push.Single:
                    return PushSingle(template);
                case Enum_Push.Multiple:
                    return PushMultiple(template);
                case Enum_Push.All:
                    return PushAll(template);
                default:
                    return "false";
            }
        }

        /// <summary>  
        /// 通知链接模板  
        /// </summary>  
        /// <param name="title">通知栏标题</param>  
        /// <param name="text">通知栏内容</param>  
        /// <param name="logo">通知栏显示本地图片</param>  
        /// <param name="logoUrl">通知栏显示网络图标，如无法读取，则显示本地默认图标，可为空</param>  
        /// <param name="url">打开的链接地址</param>   
        public string PushTemplate(int pushType, string title, string text, string logo, string logoUrl, string url)
        {
            LinkTemplate template = LinkTemplate(title, text, logo, logoUrl, url);
            switch (pushType)
            {
                case Enum_Push.Single:
                    return PushSingle(template);
                case Enum_Push.Multiple:
                    return PushMultiple(template);
                case Enum_Push.All:
                    return PushAll(template);
                default:
                    return "false";
            }
        }

        /// <summary>  
        /// 通知透传模板  
        /// </summary>  
        /// <param name="title">通知栏标题</param>  
        /// <param name="text">通知栏内容</param>  
        /// <param name="logo">通知栏显示本地图片</param>  
        /// <param name="logoUrl">通知栏显示网络图标</param>  
        /// <param name="transContent">透传内容</param>  
        /// <param name="beginTM">客户端展示开始时间</param>  
        /// <param name="endTM">客户端展示结束时间</param>    
        public string PushTemplate(int pushType, string title, string text, string logo, string logoUrl, string transContent, string beginTM, string endTM)
        {
            NotificationTemplate template = NotificationTemplate(title, text, logo, logoUrl, transContent, beginTM, endTM);
            switch (pushType)
            {
                case Enum_Push.Single:
                    return PushSingle(template);
                case Enum_Push.Multiple:
                    return PushMultiple(template);
                case Enum_Push.All:
                    return PushAll(template);
                default:
                    return "false";
            }
        }

        /// <summary>  
        /// 通知弹框下载模板  
        /// </summary>  
        /// <param name="notyTitle">通知栏标题</param>  
        /// <param name="notyContent">通知栏内容</param>  
        /// <param name="notyIcon">通知栏显示本地图片</param>  
        /// <param name="logoUrl">通知栏显示网络图标</param>  
        /// <param name="popTitle">弹框显示标题</param>  
        /// <param name="popContent">弹框显示内容</param>  
        /// <param name="popImage">弹框显示图片</param>  
        /// <param name="popButton1">弹框左边按钮显示文本</param>  
        /// <param name="popButton2">弹框右边按钮显示文本</param>  
        /// <param name="loadTitle">通知栏显示下载标题</param>  
        /// <param name="loadIcon">通知栏显示下载图标,可为空</param>  
        /// <param name="loadUrl">下载地址，不可为空</param>  
        public string PushTemplate(int pushType, string notyTitle, string notyContent, string notyIcon, string logoUrl, string popTitle, string popContent, string popImage, string popButton1, string popButton2, string loadTitle, string loadIcon, string loadUrl)
        {
            NotyPopLoadTemplate template = NotyPopLoadTemplate(notyTitle, notyContent, notyIcon, logoUrl, popTitle, popContent, popImage, popButton1, popButton2, loadTitle, loadIcon, loadUrl);
            switch (pushType)
            {
                case Enum_Push.Single:
                    return PushSingle(template);
                case Enum_Push.Multiple:
                    return PushMultiple(template);
                case Enum_Push.All:
                    return PushAll(template);
                default:
                    return "false";
            }
        }

        /// <summary>
        /// 推送单个用户
        /// </summary>
        /// <param name="template">模板内容</param>
        public string PushSingle(ITemplate template)
        {
            IGtPush push = new IGtPush("", APPKEY, MASTERSECRET);
            SingleMessage message = new SingleMessage();
            message.IsOffline = true; // 用户当前不在线时，是否离线存储,可选  
            message.OfflineExpireTime = 1000 * 3600 * 12; // 离线有效时间，单位为毫秒，可选  
            message.Data = template;
            //message.PushNetWorkType = 1; //判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为非WIFI环境  
            Target target = new Target();
            target.appId = APPID;
            target.clientId = CLIENTID[0];
            return push.pushMessageToSingle(message, target);
        }

        /// <summary>
        /// 推送多个用户
        /// </summary>
        /// <returns></returns>
        public string PushMultiple(ITemplate template)
        {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            ListMessage message = new ListMessage();
            message.IsOffline = true; // 用户当前不在线时，是否离线存储,可选  
            message.OfflineExpireTime = 1000 * 3600 * 12;// 离线有效时间，单位为毫秒，可选  
            message.Data = template;
            //message.PushNetWorkType = 0;//判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为非WIFI环境  

            //设置接收者  
            List<Target> targetList = new List<Target>();
            for (var i = 0; i < CLIENTID.Count; i++)
            {
                Target target = new Target();
                target.appId = APPID;
                target.clientId = CLIENTID[i];
                targetList.Add(target);
            }

            String contentId = push.getContentId(message, "任务组名");
            String pushResult = push.pushMessageToList(contentId, targetList);
            return pushResult;
        }

        /// <summary>
        /// 推送所有用户
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public string PushAll(ITemplate template)
        {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            AppMessage message = new AppMessage();
            message.IsOffline = true;// 用户当前不在线时，是否离线存储,可选  
            message.OfflineExpireTime = 1000 * 3600 * 12;// 离线有效时间，单位为毫秒，可选  
            message.Data = template;
            //message.PushNetWorkType = 0; //判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为非WIFI环境  
            message.Speed = 1;

            List<String> appIdList = new List<string>();
            appIdList.Add(APPID);

            List<String> phoneTypeList = new List<string>();//通知接收者的手机操作系统类型  
            phoneTypeList.Add("ANDROID");
            //phoneTypeList.Add("IOS");  

            List<String> provinceList = new List<string>();//通知接收者所在省份  
            //provinceList.Add("浙江");  

            List<String> tagList = new List<string>();
            //tagList.Add("标签5");  

            message.AppIdList = appIdList;
            message.PhoneTypeList = phoneTypeList;
            message.ProvinceList = provinceList;
            message.TagList = tagList;
            return push.pushMessageToApp(message, "toAPP任务别名");
        }

        #region 2、四种消息模板

        /* 
         *  
         * 所有推送接口均支持四个消息模板，依次为透传模板，通知透传模板，通知链接模板，通知弹框下载模板 
         * 注：IOS离线推送需通过APN进行转发，需填写pushInfo字段，目前仅不支持通知弹框下载功能 
         * 
         */

        /// <summary>  
        /// 通知弹框下载模板动作内容，常用于下载apk更新软件等  
        /// </summary>  
        /// <param name="notyTitle">通知栏标题</param>  
        /// <param name="notyContent">通知栏内容</param>  
        /// <param name="notyIcon">通知栏显示本地图片</param>  
        /// <param name="logoUrl">通知栏显示网络图标</param>  
        /// <param name="popTitle">弹框显示标题</param>  
        /// <param name="popContent">弹框显示内容</param>  
        /// <param name="popImage">弹框显示图片</param>  
        /// <param name="popButton1">弹框左边按钮显示文本</param>  
        /// <param name="popButton2">弹框右边按钮显示文本</param>  
        /// <param name="loadTitle">通知栏显示下载标题</param>  
        /// <param name="loadIcon">通知栏显示下载图标,可为空</param>  
        /// <param name="loadUrl">下载地址，不可为空</param>  
        public NotyPopLoadTemplate NotyPopLoadTemplate(string notyTitle, string notyContent, string notyIcon, string logoUrl, string popTitle, string popContent, string popImage, string popButton1, string popButton2, string loadTitle, string loadIcon, string loadUrl)
        {
            NotyPopLoadTemplate template = new NotyPopLoadTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.NotyTitle = notyTitle; //通知栏标题  
            template.NotyContent = notyContent; //通知栏内容  
            template.NotyIcon = notyIcon; //通知栏显示本地图片,如icon.png  
            template.LogoURL = logoUrl; //通知栏显示网络图标，如http://www-igexin.qiniudn.com/wp-content/uploads/2013/08/logo_getui1.png  

            template.PopTitle = popTitle; //弹框显示标题  
            template.PopContent = popContent; //弹框显示内容  
            template.PopImage = popImage;  //弹框显示图片  
            template.PopButton1 = popButton1; //弹框左边按钮显示文本  
            template.PopButton2 = popButton2; //弹框右边按钮显示文本  

            template.LoadTitle = loadTitle;//通知栏显示下载标题  
            template.LoadIcon = loadIcon;//通知栏显示下载图标,可为空，如file://push.png  
            template.LoadUrl = loadUrl;//下载地址，不可为空，http://www.appchina.com/market/d/425201/cop.baidu_0/com.gexin.im.apk  

            template.IsActived = true;//应用安装完成后，是否自动启动  
            template.IsAutoInstall = true; //下载应用完成后，是否弹出安装界面，true：弹出安装界面，false：手动点击弹出安装界面  
            template.IsBelled = true;//接收到消息是否响铃，true：响铃 false：不响铃  
            template.IsVibrationed = true;//接收到消息是否震动，true：震动 false：不震动  
            template.IsCleared = true;//接收到消息是否可清除，true：可清除 false：不可清除  
            return template;
        }

        /// <summary>  
        /// 通知链接动作内容  
        /// </summary>  
        /// <param name="title">通知栏标题</param>  
        /// <param name="text">通知栏内容</param>  
        /// <param name="logo">通知栏显示本地图片</param>  
        /// <param name="logoUrl">通知栏显示网络图标，如无法读取，则显示本地默认图标，可为空</param>  
        /// <param name="url">打开的链接地址</param>  
        public LinkTemplate LinkTemplate(string title, string text, string logo, string logoUrl, string url)
        {
            LinkTemplate template = new LinkTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.Title = title;//通知栏标题  
            template.Text = text;//通知栏内容  
            template.Logo = logo;//通知栏显示本地图片  
            template.LogoURL = logoUrl;  //通知栏显示网络图标，如无法读取，则显示本地默认图标，可为空  
            template.Url = url; //打开的链接地址,如http://www.baidu.com  

            //iOS推送需要的pushInfo字段  
            //template.setPushInfo(actionLocKey, badge, message, sound, payload, locKey, locArgs, launchImage);  

            template.IsRing = true;//接收到消息是否响铃，true：响铃 false：不响铃  
            template.IsVibrate = true;//接收到消息是否震动，true：震动 false：不震动  
            template.IsClearable = true;//接收到消息是否可清除，true：可清除 false：不可清除  
            return template;
        }

        /// <summary>  
        /// 通知透传模板动作内容  
        /// </summary>  
        /// <param name="title">通知栏标题</param>  
        /// <param name="text">通知栏内容</param>  
        /// <param name="logo">通知栏显示本地图片</param>  
        /// <param name="logoUrl">通知栏显示网络图标</param>  
        /// <param name="transContent">透传内容</param>  
        /// <param name="beginTM">客户端展示开始时间</param>  
        /// <param name="endTM">客户端展示结束时间</param>   
        public NotificationTemplate NotificationTemplate(string title, string text, string logo, string logoUrl, string transContent, string beginTM, string endTM)
        {
            NotificationTemplate template = new NotificationTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.Title = title;//通知栏标题  
            template.Text = text;//通知栏内容  
            template.Logo = logo;//通知栏显示本地图片  
            template.LogoURL = logoUrl;//通知栏显示网络图标，如https://www.baidu.com/img/bd_logo1.png  

            template.TransmissionType = "1";//应用启动类型，1：强制应用启动  2：等待应用启动  
            template.TransmissionContent = transContent;//透传内容  
            //iOS推送需要的pushInfo字段  
            //template.setPushInfo(actionLocKey, badge, message, sound, payload, locKey, locArgs, launchImage);  

            //设置客户端展示时间  
            String begin = beginTM;
            String end = endTM;
            template.setDuration(begin, end);

            template.IsRing = true; //接收到消息是否响铃，true：响铃 false：不响铃  
            template.IsVibrate = true; //接收到消息是否震动，true：震动 false：不震动  
            template.IsClearable = true; //接收到消息是否可清除，true：可清除 false：不可清除  
            return template;
        }

        /// <summary>  
        /// 透传模板动作内容  
        /// </summary>  
        /// <param name="transContent">透传内容</param>  
        /// <param name="beginTM">客户端展示开始时间</param>  
        /// <param name="endTM">客户端展示结束时间</param>  
        /// <returns></returns>  
        public TransmissionTemplate TransmissionTemplate(string transContent, string beginTM, string endTM)
        {
            TransmissionTemplate template = new TransmissionTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.TransmissionType = "1"; //应用启动类型，1：强制应用启动 2：等待应用启动  
            template.TransmissionContent = transContent;  //透传内容  
            //iOS推送需要的pushInfo字段  
            //template.setPushInfo(actionLocKey, badge, message, sound, payload, locKey, locArgs, launchImage);  
            template.setPushInfo("1", 4, "2", "", "", "", "", "");
            //设置客户端展示时间  
            String begin = beginTM;
            String end = endTM;
            template.setDuration(begin, end);
            return template;
        }

        #endregion

        #region 3、获取用户当前状态

        /// <summary>  
        /// 获取用户当前状态  
        /// </summary>  
        /// <returns></returns>  
        public string GetUserStatus()
        {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            String ret = push.getClientIdStatus(APPID, CLIENTID[0]);
            return ret;
        }

        #endregion
    }
}