using EGT_OTA.Helper;
using EGT_OTA.Models;
using EGT_OTA.Redis;
using IRedis;
using SubSonic.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EGT_OTA.Controllers
{
    public class AppHelper
    {
        protected static readonly string Base_Url = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"];
        public static readonly SimpleRepository db = BasicRepository.GetRepo();
        public static readonly SimpleRepository logdb = LogRepository.GetRepo();
        protected static readonly RedisBase redis = RedisHelper.Redis;

        #region  推送

        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="usernumber">推送目标用户编号</param>
        /// <param name="id">对象ID（文章ID）</param>
        /// <param name="number">对象Number</param>
        /// <param name="name">用户名称/文章标题</param>
        /// <param name="pushtype">推送类型</param>
        /// <param name="ispush">是否推送</param>
        public void Push(string usernumber, int id, string number, string name, int pushtype, bool ispush = true)
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

                var result = "";
                if (ispush)
                {
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

                    var url = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"];

                    switch (pushtype)
                    {
                        case Enum_PushType.Article:
                            result = message.PushTemplate(Enum_Push.Single, "小微篇文章推荐啦", name, "", url + "Images/push.png", Enum_PushType.Article + "|" + id, beginTime, endTime);
                            break;
                        case Enum_PushType.Comment:
                            result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有好友评论啦，快去看看吧", "", url + "Images/push.png", Enum_PushType.Comment + "|" + id + "|" + number, beginTime, endTime);
                            break;
                        case Enum_PushType.Money:
                            result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有好友打赏啦，快去看看吧", "", url + "Images/push.png", Enum_PushType.Money.ToString(), beginTime, endTime);
                            break;
                        case Enum_PushType.Red:
                            result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "小微篇送您一个红包，快去看看吧", "", url + "Images/push.png", Enum_PushType.Red.ToString(), beginTime, endTime);
                            break;
                        case Enum_PushType.Fan:
                            result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有新的粉丝啦，快去看看吧", "", url + "Images/push.png", Enum_PushType.Fan.ToString(), beginTime, endTime);
                            break;
                        case Enum_PushType.FanArticle:
                            result = message.PushTemplate(Enum_Push.Single, "小微篇提醒您", "有好友发文啦，快去看看吧", "", url + "Images/push.png", Enum_PushType.FanArticle + "|" + id, beginTime, endTime);
                            break;
                        case Enum_PushType.Update:
                            result = message.PushTemplate(Enum_Push.Single, "小微篇更新啦", "更新内容", "", url + "Images/push.png", "10", beginTime, endTime);
                            break;
                        default:
                            break;
                    }
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

        /// <summary>
        /// 文章模板（新）
        /// </summary>
        public static List<ArticleTemplate> GetArticleTemplate()
        {
            List<ArticleTemplate> list = new List<ArticleTemplate>();
            if (CacheHelper.Exists("ArticleTemplate"))
            {
                list = (List<ArticleTemplate>)CacheHelper.GetCache("ArticleTemplate");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/articletemp.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ArticleTemplate>>(str);
                var baseurl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
                list.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.BackgroundImage))
                    {
                        if (!x.BackgroundImage.ToLower().StartsWith("http"))
                        {
                            x.BackgroundImage = baseurl + x.BackgroundImage;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(x.ThumbImage))
                    {
                        if (!x.ThumbImage.ToLower().StartsWith("http"))
                        {
                            x.ThumbImage = baseurl + x.ThumbImage;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(x.BottomImage))
                    {
                        if (!x.BottomImage.ToLower().StartsWith("http"))
                        {
                            x.BottomImage = baseurl + x.BottomImage;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(x.Showy))
                    {
                        if (!x.Showy.ToLower().StartsWith("http"))
                        {
                            x.Showy = baseurl + x.Showy;
                        }
                    }
                    if (x.TopImage != null)
                    {
                        if (x.TopImage.Count > 0)
                        {
                            x.TopImage.ForEach(y =>
                            {
                                if (!y.Url.ToLower().StartsWith("http"))
                                {
                                    y.Url = baseurl + y.Url;
                                }
                            });
                        }
                    }
                });
                CacheHelper.Insert("ArticleTemplate", list);
            }
            return list;
        }

        /// <summary>
        /// 主题色模板
        /// </summary>
        public static List<ColorTemplate> GetColorTemplate()
        {
            List<ColorTemplate> list = new List<ColorTemplate>();
            if (CacheHelper.Exists("ColorTemplate"))
            {
                list = (List<ColorTemplate>)CacheHelper.GetCache("ColorTemplate");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/colortemp.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ColorTemplate>>(str);
                CacheHelper.Insert("ColorTemplate", list);
            }
            return list;
        }

        /// <summary>
        /// 文章类型
        /// </summary>
        public static List<ArticleType> GetArticleType()
        {
            List<ArticleType> list = new List<ArticleType>();
            if (CacheHelper.Exists("ArticleType"))
            {
                list = (List<ArticleType>)CacheHelper.GetCache("ArticleType");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/articletype.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ArticleType>>(str);
                CacheHelper.Insert("ArticleType", list);
            }
            return list.FindAll(x => x.Status == Enum_Status.Approved);
        }

        /// <summary>
        /// 敏感词
        /// </summary>
        public static List<string> GetDirtyWord()
        {
            try
            {
                List<string> list = redis.HashGetAllValues<string>("DirtyWord");
                if (list.Count == 0)
                {
                    string str = string.Empty;
                    string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/dirtyword.config");
                    if (System.IO.File.Exists(filePath))
                    {
                        StreamReader sr = new StreamReader(filePath, Encoding.Default);
                        str = sr.ReadToEnd();
                        sr.Close();
                    }
                    list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(str);

                    var index = 0;
                    list.ForEach(x =>
                    {
                        redis.HashSet<string>("DirtyWord", index++.ToString(), x);
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }


        /// <summary>
        /// 判断是否包含敏感词
        /// </summary>
        public static string HasDirtyWord(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "";
            }
            content = content.Trim();
            var list = AppHelper.GetDirtyWord();
            for (var i = 0; i < list.Count; i++)
            {
                if (content.Contains(list[i]))
                {
                    return list[i];
                }
            }
            return "";
        }

        /// <summary>
        /// Banner
        /// </summary>
        public static List<Banner> GetBanner()
        {
            List<Banner> list = new List<Banner>();
            if (CacheHelper.Exists("Banner"))
            {
                list = (List<Banner>)CacheHelper.GetCache("Banner");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/banner.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Banner>>(str);
                CacheHelper.Insert("Banner", list);
            }
            return list;
        }

        /// <summary>
        /// 文章模板（旧）
        /// </summary>
        public static List<Template> GetArticleTemp()
        {
            List<Template> list = new List<Template>();
            if (CacheHelper.Exists("Template"))
            {
                list = (List<Template>)CacheHelper.GetCache("Template");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/template.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Template>>(str);
                var baseurl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
                list.ForEach(x =>
                {
                    x.ThumbUrl = baseurl + x.ThumbUrl;
                    x.Cover = baseurl + x.Cover;
                });
                CacheHelper.Insert("Template", list);
            }
            return list;
        }

        /// <summary>
        /// Search
        /// </summary>
        public static List<string> GetSearch()
        {
            List<string> list = new List<string>();
            if (CacheHelper.Exists("Search"))
            {
                list = (List<string>)CacheHelper.GetCache("Search");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/search.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(str);
                CacheHelper.Insert("Search", list);
            }
            return list;
        }

        /// <summary>
        /// MusicSearch
        /// </summary>
        public static List<string> GetMusicSearch()
        {
            List<string> list = new List<string>();
            if (CacheHelper.Exists("MusicSearch"))
            {
                list = (List<string>)CacheHelper.GetCache("MusicSearch");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/musicsearch.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(str);
                CacheHelper.Insert("MusicSearch", list);
            }
            return list;
        }


        /// <summary>
        /// Showy
        /// </summary>
        public static List<Showy> GetShowy()
        {
            List<Showy> list = new List<Showy>();
            if (CacheHelper.Exists("Showy"))
            {
                list = (List<Showy>)CacheHelper.GetCache("Showy");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/showy.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Showy>>(str);
                list.ForEach(x =>
                {
                    x.ShowyCover.ForEach(y =>
                    {
                        y.Cover = Base_Url + y.Cover;
                    });
                });
                CacheHelper.Insert("Showy", list);
            }
            return list;
        }

        /// <summary>
        /// 求助或建议
        /// </summary>
        public static List<Help> GetHelp()
        {
            List<Help> list = new List<Help>();
            if (CacheHelper.Exists("Help"))
            {
                list = (List<Help>)CacheHelper.GetCache("Help");
            }
            else
            {
                list = InitHelp();
                CacheHelper.Insert("Help", list);
            }
            return list;
        }

        /// <summary>
        /// 初始化帮助类型
        /// </summary>
        public static List<HelpType> InitHelpType()
        {
            List<HelpType> helpTypes = new List<HelpType>() { };
            helpTypes.Add(new HelpType(1, "使用说明", Base_Url + "Images/Help/1.png"));
            helpTypes.Add(new HelpType(2, "常见使用问题", Base_Url + "Images/Help/2.png"));
            helpTypes.Add(new HelpType(3, "账户问题", Base_Url + "Images/Help/3.png"));
            helpTypes.Add(new HelpType(4, "投稿相关问题", Base_Url + "Images/Help/4.png"));
            helpTypes.Add(new HelpType(5, "音乐与视频相关", Base_Url + "Images/Help/5.png"));
            helpTypes.Add(new HelpType(6, "文章丢失相关问题", Base_Url + "Images/Help/6.png"));
            helpTypes.Add(new HelpType(7, "互动相关问题", Base_Url + "Images/Help/7.png"));
            helpTypes.Add(new HelpType(8, "打赏相关", Base_Url + "Images/Help/8.png"));
            return helpTypes;
        }

        /// <summary>
        /// 初始化帮助
        /// </summary>
        public static List<Help> InitHelp()
        {
            List<Help> helps = new List<Help>() { };
            helps.Add(new Help(2, 21, "如何开始制作小微篇？", "在主界面点击最下方的加号，即可开始创作。导入图片后，可以任意编辑文字，添加和调整段落。编辑完后可以设置模板，点击“分享”即可分享到朋友圈等平台。"));
            helps.Add(new Help(2, 22, "如何设置文章封面？", "在小微篇编辑界面，点击封面区域右下角的白色带有“左右箭头”样式的按钮，选择您所需的封面图片。"));
            helps.Add(new Help(2, 23, "如何使用模板？", "在编辑页面点击完成文章后，点击右下角模板字样，可以选择不同模板，模板9个一组，有多组可供选择。"));
            helps.Add(new Help(2, 24, "如何在图片上设置“水印”？", "在小微篇的“设置-通用设置”里打开“图片水印”，新插入的图片在发布时会自动生成水印。"));
            helps.Add(new Help(2, 25, "如何保存小微篇文章？", "只要一进发布的小微篇文章会永久保存在小微篇云端账户里。小微篇分享时可以复制链接得到网页地址，网址在浏览器里打开后可以保存电脑里。"));
            helps.Add(new Help(2, 26, "如何让文章带有超链接？", "在段落的文本编辑界面里，可以添加网页链接。设置好链接后可以在完成的文章里使用超链接功能。"));
            helps.Add(new Help(2, 27, "如何收藏喜欢的文章，以及如何取消收藏？", "浏览别人的文章时，点击“分享”按钮，有收藏文章的功能。收藏后在同样操作位置可以取消收藏操作。"));
            helps.Add(new Help(2, 28, "如何分享我的专栏？", "每个小微篇用户都拥有自己的专栏，公开发布的文章会出现在专栏里。在小微篇“我的里面”右上角为“分享专栏”功能按钮。从小微篇编写文章的作者位置可进入作者的专栏。"));
            helps.Add(new Help(2, 29, "阅读权限具体含义？", "小微篇文章针对不同用户需求，设定了4种可阅读权限。<br />公开：您的文章会被所有人看见，而且会出现在您的个人专栏里。<br />不公开：不公开的文章不会出现在您的个人专栏里，通过您个人的分享渠道去传播。<br />加密：您可以给文章设置一个密码，分享后必须用密码口令才能查看。<br />私密：文章仅能自己看见，不可分享，公开后的文章可以改为“私密”状态让文章链接失效。<br />这4种文章可以按需要互相转变。"));
            helps.Add(new Help(2, 210, "关于文章上显示的时间？", "小微篇文章的时间是以文章初次发布时间自动生成，不可更改。"));
            helps.Add(new Help(2, 211, "小微篇占用空间过大的情况？", "小微篇为您最大限度的节省了空间，还可以在“设置-通用设置”里清理缓存，释放更多空间。"));
            helps.Add(new Help(2, 212, "小微篇文章中出现的广告情况？", "分享出去的热门文章会在屏幕中间适当出现广告，轻轻点击关闭即可。请谅解！<br />若是在屏幕下方有恶俗广告内容的，非小微篇所为，是流量被运营商网络所恶意劫持。"));

            helps.Add(new Help(3, 31, "如何获取我的小微篇号？", "在小微篇“设置-账户”里可以看到小微篇号。"));
            helps.Add(new Help(3, 32, "如何找回小微篇号？", "如你忘记自己的小微篇号，可以从您发布分享的文章里，通过点击文章作者进入专栏查看小微篇号。"));
            helps.Add(new Help(3, 33, "为何账户会被查封？", "小微篇不允许发布违规内容的文章，情节严重者会被查封小微篇账户。<br />关于违规文章，请参看：文章违规内容说明。"));

            helps.Add(new Help(4, 41, "投稿相关问题-上传文章出现“精”是什么意思？", "公开发布的文章可以投稿到小微篇，投稿被采用和编辑选中的文章，在作者的个人专栏页面会出现“精”字标识。"));
            helps.Add(new Help(4, 42, "什么样的文章才可以上推荐？", "每天投稿的文章很多，编辑在投稿文章里选择部分内容到小微篇的“发现”里。<br />每天上推荐的文章数量有限，如未被选中，不代表您的作品不优秀。"));
            helps.Add(new Help(4, 43, "如何向小微篇投稿（投稿方式，投稿间隔时间等）？", "文章的“操作”按钮里有“投稿”功能按钮，每次投稿需间隔7天，请注意投稿时间。"));

            helps.Add(new Help(5, 51, "如何添加背景音乐", "在小微篇编辑页面里，点击封面区域左下角的白色“音符”样式的按钮，就可以进入背景音乐选择界面。"));
            helps.Add(new Help(5, 52, "如何加载视频进小微篇文章？", "点击段落间的“加号”，第三个按钮就是添加视频按钮。"));
            helps.Add(new Help(5, 53, "本地音乐无法使用？", "小微篇目前不支持本地音乐上传服务，今后会去完善此功能。"));
            helps.Add(new Help(5, 54, "音乐如何自动播放？", "在小微篇“设置-通用设置”里，可以设置音乐为自动播放。当滑动查找文章时会自动触发音乐播放。"));
            helps.Add(new Help(5, 55, "找不到需要的音乐", "小微篇对接的音乐库有海量的内容，但不是每首歌都有，希望有适合您的音乐。"));

            helps.Add(new Help(6, 61, "删除文章需要恢复？", "若您不小心误删了自己文章，请在“求助或建议”里告知我们您所需恢复文章的标题，我们可以试着帮您找回。"));
            helps.Add(new Help(6, 62, "文章被查封了？", "小微篇会查封不和规定的文章，若有异议可以向小微篇申诉解封文章。<br />需要将文章分享链接发给我们，我们进行复核。"));

            helps.Add(new Help(7, 71, "如何找到其他小微篇用户？", "在小微篇“发现”里浏览文章时，点击文章的作者信息或者界面右上角的“作者专栏”，可以进入该作者的专栏里去关注他。<br />同时您可以在小微篇首页顶部搜索栏，点击搜索用户的昵称或小微篇号，关注更多朋友。"));
            helps.Add(new Help(7, 72, "如何点赞和评论？", "小微篇文章支持在APP内点赞和评论。同时也支持在微信浏览时点赞。"));
            helps.Add(new Help(7, 73, "如何回答别人的评论和删除恶意评论？", "在评论中点击对方的评论，就可以针对这条评论进行回复了。<br />作者可以删除自己文章评论里他人的恶意评论。"));
            helps.Add(new Help(7, 74, "如何找到更多小微篇文章内容？", "除了在小微篇首页浏览各个分类里推荐的内容外，您可以在小微篇首页顶部搜索栏，输入您想要获取的关键词，发现更多小微篇内容。"));

            helps.Add(new Help(8, 81, "文章赞赏功能如何开启？", "在小微篇“设置-我的打赏”里开启“启用打赏”，您的所有文章在微信浏览时都会有被赞赏功能。"));
            helps.Add(new Help(8, 82, "赞赏的费用是给谁的？", "赞赏的费用是打给作者的，费用会先保存在小微篇里，达到300元后用户可以申请提现。"));
            helps.Add(new Help(8, 83, "收到赞赏后如何提现？", "赞赏金额达到300元就可以申请提现。在您申请提现后，我们会在5个工作日内将款打到您的账户，请务必填写已实名认证的支付宝账户信息。"));

            return helps;
        }
    }
}