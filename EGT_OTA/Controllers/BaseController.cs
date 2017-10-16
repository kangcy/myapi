using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using EGT_OTA.Models;
using CommonTools;
using SubSonic.Repository;
using System.Text.RegularExpressions;
using EGT_OTA.Helper;
using System.Drawing;
using EGT_OTA.Helper.Config;
using SubSonic.DataProviders;
using System.Drawing.Imaging;
using IRedis;
using EGT_OTA.Redis;

namespace EGT_OTA.Controllers
{
    public class BaseController : Controller
    {
        protected readonly SimpleRepository db = BasicRepository.GetRepo();
        protected readonly IDataProvider provider = BasicRepository.GetProvider();
        protected static readonly RedisBase redis = RedisHelper.Redis;

        //默认管理员账号
        protected readonly string Admin_Name = System.Web.Configuration.WebConfigurationManager.AppSettings["admin_name"];
        protected readonly string Admin_Password = System.Web.Configuration.WebConfigurationManager.AppSettings["admin_password"];
        protected readonly string Base_Url = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"];

        //提现金额
        protected readonly int Apply_Money = Tools.SafeInt(System.Configuration.ConfigurationManager.AppSettings["applymoney"]);

        /// <summary>
        /// 分页基础类
        /// </summary>
        public class Pager
        {
            public int Index { get; set; }
            public int Size { get; set; }

            public Pager()
            {
                this.Index = ZNRequest.GetInt("page", 1);
                this.Size = ZNRequest.GetInt("rows", 15);
            }
        }

        /// <summary>
        /// Api返回结果
        /// </summary>
        public class ApiResult
        {
            public ApiResult()
            {
                this.result = false;
                this.code = 0;
                this.message = string.Empty;
            }

            public bool result { get; set; }

            public int code { get; set; }

            public object message { get; set; }
        }

        /// <summary>
        /// 解码
        /// </summary>
        protected string UrlDecode(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }
            return System.Web.HttpContext.Current.Server.UrlDecode(msg);
        }

        /// <summary>
        /// 编码
        /// </summary>
        protected string UrlEncode(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }
            return System.Web.HttpContext.Current.Server.UrlEncode(msg);
        }

        /// <summary>
        /// 防注入
        /// </summary>
        protected string SqlFilter(string inputString, bool nohtml = true, bool xss = true)
        {
            string SqlStr = @"script|and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";
            try
            {
                if (!string.IsNullOrEmpty(inputString))
                {
                    inputString = UrlDecode(inputString);
                    if (nohtml)
                    {
                        inputString = Tools.NoHTML(inputString);
                    }
                    inputString = Regex.Replace(inputString, @"\b(" + SqlStr + @")\b", string.Empty, RegexOptions.IgnoreCase);
                    if (nohtml)
                    {
                        inputString = inputString.Replace("&nbsp;", "");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SQL注入", ex);
            }
            if (xss)
            {
                return AntiXssChineseString.ChineseStringSanitize(EmotionHelper.EmotionFilter(inputString));
            }
            else
            {
                return EmotionHelper.EmotionFilter(inputString);
            }
        }

        /// <summary>
        /// 判断是否SQL注入
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        protected bool ProcessSqlStr(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return false;
            }
            var list = new List<string> { "script", "and", "or", "exec", "execute", "insert", "select", "delete", "update", "alter", "create", "drop", "count", "chr", "char", "asc", "mid", "substring", "master", "truncate", "declare", "xp_cmdshell", "restore", "backup", "net", "user", "localgroup", "administrators" };
            try
            {
                inputString = Tools.NoHTML(UrlDecode(inputString)).ToLower();
                for (var i = 0; i < list.Count; i++)
                {
                    if (inputString.Contains(list[i]))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error(inputString + ":" + ex.Message);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 生成随机编号
        /// </summary>
        /// <param name="length"></param>
        protected string BuildNumber()
        {
            return UnixTimeHelper.FromDateTime(DateTime.Now).ToString() + new Random().Next(10001, 99999).ToString();
        }

        /// <summary>
        /// 图片完整路径
        /// </summary>
        protected string GetFullUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Base_Url + "Images/default.png";
            }
            if (url.ToLower().StartsWith("http"))
            {
                return url;
            }
            return Base_Url + url;
        }

        /// <summary>
        /// APP访问用户信息
        /// </summary>
        protected User GetUserInfo()
        {
            var id = ZNRequest.GetInt("ID");
            if (id == 0)
            {
                return null;
            }
            return db.Single<User>(x => x.ID == id);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="content">原始字符串</param>
        protected string CutString(string content, int length)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }
            var nameByte = System.Text.Encoding.Default.GetBytes(content);
            if (nameByte.Length > length)
            {
                byte[] b = new byte[length];
                Array.Copy(nameByte, 0, b, 0, length);
                content = System.Text.Encoding.Default.GetString(b); //重新获取字符串
            }
            return content;
        }

        /// <summary>
        /// 标签
        /// </summary>
        protected List<Tag> GetTag()
        {
            List<Tag> list = new List<Tag>();
            if (CacheHelper.Exists("Tag"))
            {
                list = (List<Tag>)CacheHelper.GetCache("Tag");
            }
            else
            {
                list = db.All<Tag>().ToList();
                CacheHelper.Insert("Tag", list);
            }
            return list;
        }

        /// <summary>
        /// Showy
        /// </summary>
        protected List<Showy> GetShowy()
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
                CacheHelper.Insert("Showy", list);
            }
            return list;
        }

        /// <summary>
        /// Banner
        /// </summary>
        protected List<Banner> GetBanner()
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
        /// 音乐
        /// </summary>
        protected List<MusicJson> GetMusic()
        {
            List<MusicJson> list = new List<MusicJson>();

            list = redis.HashGetAllValues<MusicJson>("Music");


            if (CacheHelper.Exists("Music"))
            {
                list = (List<MusicJson>)CacheHelper.GetCache("Music");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Config/music.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MusicJson>>(str);
                CacheHelper.Insert("Music", list);
            }
            return list.FindAll(x => x.Status == Enum_Status.Approved);
        }

        /// <summary>
        /// 文章类型
        /// </summary>
        protected List<ArticleType> GetArticleType()
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
        /// 文章模板
        /// </summary>
        protected List<Template> GetArticleTemp()
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
        /// 文章模板（新）
        /// </summary>
        protected List<ArticleTemplate> GetArticleTemplate()
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
                        x.BackgroundImage = baseurl + x.BackgroundImage;
                    }
                    if (!string.IsNullOrWhiteSpace(x.ThumbImage))
                    {
                        x.ThumbImage = baseurl + x.ThumbImage;
                    }
                });
                CacheHelper.Insert("ArticleTemplate", list);
            }
            return list;
        }

        /// <summary>
        /// 敏感词
        /// </summary>
        protected List<string> GetDirtyWord()
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

        /// <summary>
        /// 判断是否包含敏感词
        /// </summary>
        protected string HasDirtyWord(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "";
            }
            content = content.Trim();
            var list = GetDirtyWord();
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
        /// 求助或建议
        /// </summary>
        protected List<Help> GetHelp()
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
        protected List<HelpType> InitHelpType()
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
        protected List<Help> InitHelp()
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

        /// <summary>
        /// 请求返回结果
        /// </summary>
        public class ResultJson
        {
            public bool State { get; set; }

            public string Message { get; set; }
        }

        #region  生成缩略图

        ///<summary>  
        /// 生成缩略图  
        /// </summary>  
        /// <param name="originalImagePath">源图对象</param>  
        /// <param name="mode">生成缩略图的方式</param>
        /// <param name="width">缩略图宽度</param>  
        /// <param name="height">缩略图高度</param> 
        /// <param name="height">是否添加水印（0：不添加,1：添加）</param>  
        /// <param name="height">缩略图保存路径</param> 
        protected void MakeThumbnail(Image originalImage, string mode, int width, int height, int isDraw, string thumbnailPath)
        {
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;//原图宽度
            int oh = originalImage.Height;//原图高度
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                  
                    break;
                case "W"://指定宽，高按比例                      
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例  
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                  
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            Image bitmap = new Bitmap(towidth, toheight);//新建一个bmp图片  
            Graphics g = Graphics.FromImage(bitmap);//新建一个画板  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;//设置高质量插值法  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//设置高质量,低速度呈现平滑程度  
            g.Clear(Color.Transparent);//清空画布并以透明背景色填充  
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);//在指定位置并且按指定大小绘制原图片的指定部分  
            try
            {
                ///添加水印
                if (isDraw == 1)
                {
                    bitmap = WaterMark(bitmap);
                }
                //以jpg格式保存缩略图  

                //#region  高质量

                //var myImageCodecInfo = GetEncoderInfo("image/jpeg");
                //var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                //var myEncoderParameters = new EncoderParameters(1);
                //var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                //myEncoderParameters.Param[0] = myEncoderParameter;
                //bitmap.Save(thumbnailPath, myImageCodecInfo, myEncoderParameters);

                //#endregion

                EncoderParameter p;
                EncoderParameters ps;
                ps = new EncoderParameters(1);
                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                ps.Param[0] = p;
                ImageCodecInfo ii = GetCodecInfo("image/jpeg");

                bitmap.Save(thumbnailPath, ii, ps);

                //bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                bitmap.Dispose();
                g.Dispose();
            }
        }

        #endregion

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }

        #region 水印

        /// <summary>
        /// 添加水印
        /// </summary>
        /// <param name="bitmap">原始图片</param>
        protected Image WaterMark(Image image)
        {
            ///读取水印配置
            CommonConfig.ConfigItem watermarkmodel = CommonConfig.Instance.GetConfig("WaterMark");

            if (watermarkmodel != null)
            {
                if (watermarkmodel.Cate == 1) //判断水印类型
                {
                    //水印图片
                    Image copyImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(watermarkmodel.ImageUrl));
                    int width = 0;
                    int height = 0;
                    switch (watermarkmodel.Location)
                    {
                        case 1: width = 0; height = 0; break;//左上角
                        case 2: width = (image.Width - copyImage.Width) / 2; height = 0; break;//左上居中
                        case 3: width = image.Width - copyImage.Width; height = 0; break;//右上
                        case 4: width = 0; height = (image.Height - copyImage.Height) / 2; break;//左中
                        case 5: width = (image.Width - copyImage.Width) / 2; height = (image.Height - copyImage.Height) / 2; break;//中间
                        case 6: width = image.Width - copyImage.Width; height = (image.Height - copyImage.Height) / 2; break;
                        case 7: width = 0; height = image.Height - copyImage.Height; break;
                        case 8: width = (image.Width - copyImage.Width) / 2; height = image.Height - copyImage.Height; break;
                        case 9: width = image.Width - copyImage.Width; height = image.Height - copyImage.Height; break;//右下角
                    }
                    Graphics g = Graphics.FromImage(image);
                    g.DrawImage(copyImage, new Rectangle(width, height, Convert.ToInt16(watermarkmodel.Width), Convert.ToInt16(watermarkmodel.Height)), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                    g.Dispose();
                    copyImage.Dispose();
                }
                else
                {
                    //文字水印
                    int width = 0;
                    int height = 0;
                    int fontwidth = Convert.ToInt32(watermarkmodel.FontSize * watermarkmodel.Word.Length);
                    int fontheight = Convert.ToInt32(watermarkmodel.FontSize);
                    switch (watermarkmodel.Location)
                    {
                        case 1: width = 0; height = 0; break;
                        case 2: width = (image.Width - fontwidth) / 2; height = 0; break;
                        case 3: width = image.Width - fontwidth; height = 0; break;
                        case 4: width = 0; height = (image.Height - fontheight) / 2; break;
                        case 5: width = (image.Width - fontwidth) / 2; height = (image.Height - fontheight) / 2; break;
                        case 6: width = image.Width - fontwidth; height = (image.Height - fontheight) / 2; break;
                        case 7: width = 0; height = image.Height - fontheight; break;
                        case 8: width = (image.Width - fontwidth) / 2; height = image.Height - fontheight; break;
                        case 9: width = image.Width - fontwidth; height = image.Height - fontheight; break;
                    }
                    Graphics g = Graphics.FromImage(image);
                    g.DrawImage(image, 0, 0, image.Width, image.Height);
                    Font f = new Font("Verdana", float.Parse(watermarkmodel.FontSize.ToString()));
                    Brush b = new SolidBrush(Color.White);
                    g.DrawString(watermarkmodel.Word, f, b, width, height);
                    g.Dispose();
                }
            }
            return image;
        }
        #endregion

        #region  文章列表

        protected List<ArticleJson> ArticleListInfo(List<Article> list, string usernumber = "")
        {
            //文章编号集合
            var array = list.Select(x => x.Number).ToArray();
            var articletypes = GetArticleType();
            var parts = new SubSonic.Query.Select(provider).From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic).And("ArticleNumber").In(array).OrderAsc("SortID").ExecuteTypedList<ArticlePart>();

            List<string> userids = new List<string>();
            list.ForEach(x =>
            {
                userids.Add(x.CreateUserNumber);
            });

            var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Cover", "Signature", "Number", "IsPay").From<User>().Where("Number").In(userids.ToArray()).ExecuteTypedList<User>();

            //判断是否关注、判断是否点赞、判断是否收藏
            var fans = new List<Fan>();
            var zans = new List<ArticleZan>();
            var keeps = new List<Keep>();
            if (!string.IsNullOrWhiteSpace(usernumber))
            {
                fans = db.Find<Fan>(x => x.CreateUserNumber == usernumber).ToList();
                zans = db.Find<ArticleZan>(x => x.CreateUserNumber == usernumber).ToList();
                keeps = db.Find<Keep>(x => x.CreateUserNumber == usernumber).ToList();
            }

            var tags = GetTag();

            List<ArticleJson> newlist = new List<ArticleJson>();
            list.ForEach(x =>
            {
                var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                if (user != null)
                {
                    ArticleJson model = new ArticleJson();
                    var articletype = articletypes.FirstOrDefault(y => y.ID == x.TypeID);
                    model.UserID = user.ID;
                    model.NickName = user.NickName;
                    model.Avatar = user.Avatar;
                    model.Signature = user.Signature;
                    model.UserCover = user.Cover;
                    model.IsPay = user.IsPay;
                    model.ArticleID = x.ID;
                    model.ArticleNumber = x.Number;
                    model.Title = x.Title;
                    model.Views = x.Views;
                    model.Goods = x.Goods;

                    //标签
                    model.TagList = new List<Tag>();
                    if (!string.IsNullOrWhiteSpace(x.Tag))
                    {
                        var tag = x.Tag.Split(',').ToList();
                        tag.ForEach(y =>
                        {
                            var id = Tools.SafeInt(y);
                            var item = tags.FirstOrDefault(z => z.ID == id);
                            if (item != null)
                            {
                                model.TagList.Add(item);
                            }
                        });
                    }
                    model.IsFollow = fans.Count(y => y.ToUserNumber == x.CreateUserNumber);
                    model.IsZan = zans.Count(y => y.ArticleNumber == x.Number);
                    model.IsKeep = keeps.Count(y => y.ArticleNumber == x.Number);
                    model.UserNumber = x.CreateUserNumber;
                    model.Cover = x.Cover;
                    model.CreateDate = x.CreateDate.ToString("yyyy-MM-dd hh:mm");
                    model.TypeName = articletype == null ? "" : articletype.Name;
                    model.ArticlePart = parts.Where(y => y.ArticleNumber == x.Number).OrderBy(y => y.ID).Take(4).ToList();
                    model.ArticlePower = x.ArticlePower;
                    model.Recommend = x.Recommend;
                    model.Province = x.Province;
                    model.City = x.City;
                    model.Submission = x.Submission;
                    newlist.Add(model);
                }
            });

            return newlist;
        }

        #endregion
    }
}
