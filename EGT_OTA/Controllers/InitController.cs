using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Helper;
using EGT_OTA.Helper.Config;
using EGT_OTA.Helper.Search;
using EGT_OTA.Models;

namespace EGT_OTA.Controllers
{
    public class InitController : BaseController
    {
        protected string baseUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];

        /// <summary>
        /// 初始化数据
        /// </summary>
        public ActionResult Index()
        {
            InitMusicRedis();

            //InitUser();
            //InitArticle();

            return Content("成功");
        }

        /// <summary>
        /// 重置音乐索引
        /// </summary>
        /// <returns></returns>
        public ActionResult Build()
        {
            MusicSearch.IndexReset();
            return Content("成功");
        }

        /// <summary>
        /// 初始化音乐Redis
        /// </summary>
        protected void InitMusicRedis()
        {
            //Thread thread = new Thread(delegate()
            //{
            //    var length = redis.HashLength("MusicSearch");
            //    if (length == 0)
            //    {
            //        var startmusic = Tools.SafeInt(System.Web.Configuration.WebConfigurationManager.AppSettings["startmusic"]);
            //        var endmusic = Tools.SafeInt(System.Web.Configuration.WebConfigurationManager.AppSettings["endmusic"]);
            //        var recordCount = Tools.SafeInt(System.Web.Configuration.WebConfigurationManager.AppSettings["records"]);
            //        var pageSize = 1000;
            //        var query = new SubSonic.Query.Select(provider, "ID", "Name").From<Music>().Where<Music>(x => x.ID >= startmusic && x.ID <= endmusic);

            //        var totalPage = recordCount % pageSize == 0 ? recordCount / pageSize : recordCount / pageSize + 1;

            //        LogHelper.ErrorLoger.Error(recordCount + "," + totalPage);

            //        var index = 1;
            //        while (index <= totalPage)
            //        {
            //            var list = query.Paged(index, pageSize).OrderDesc("ID").ExecuteTypedList<MusicSearch>();
            //            list.ForEach(x =>
            //            {
            //                redis.HashSet<MusicSearch>("MusicSearch", x.ID.ToString(), x);
            //            });
            //            index++;
            //            Thread.Sleep(1000);
            //        }
            //    }
            //});
            //thread.IsBackground = true;
            //thread.Name = "同步音乐Redis线程";
            //thread.Start();
        }

        /// <summary>
        /// 初始化用户
        /// </summary>
        protected void InitUser()
        {
            var users = new List<User>();
            var password = DesEncryptHelper.Encrypt("123456");

            users.Add(new User
            {
                NickName = "你好陌生人",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/1.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "扬州市",
                Birthday = new DateTime(1991, 2, 15),
                Signature = "说出你的故事",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "灰色记忆",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/2.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1989, 2, 14),
                Signature = "压力山大啊",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "球球早点睡",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/3.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1995, 4, 7),
                Signature = "请停止你的表演",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "猫九",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/4.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "徐州市",
                Birthday = new DateTime(1991, 3, 23),
                Signature = "又日出就要面对日落",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "曲终人不散",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "连云港市",
                Birthday = new DateTime(1986, 3, 8),
                Signature = "一言一行，一世嚣张",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "泱泱",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/6.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "苏州市",
                Birthday = new DateTime(1995, 12, 3),
                Signature = "在薄情的世界里深情的活着",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "长安不见香渐远",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/7.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "常州市",
                Birthday = new DateTime(1994, 8, 1),
                Signature = "我美不美？",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "我叫爆姐",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/8.jpg"),
                Cover = "../images/header.png",
                Province = "辽宁省",
                City = "沈阳市",
                Birthday = new DateTime(1992, 4, 9),
                Signature = "我有一句妈卖批",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "旋BB",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/9.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1997, 5, 2),
                Signature = "请给我打赏，谢谢",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "Sisley",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/10.jpg"),
                Cover = "../images/header.png",
                Province = "辽宁省",
                City = "大连市",
                Birthday = new DateTime(1995, 2, 3),
                Signature = "宝宝心里苦，宝宝要抱抱",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "尘埃",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/11.jpg"),
                Cover = "../images/header.png",
                Province = "浙江省",
                City = "温州市",
                Birthday = new DateTime(1998, 4, 6),
                Signature = "感觉自己萌萌哒",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "尘埃",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/12.jpg"),
                Cover = "../images/header.png",
                Province = "浙江省",
                City = "温州市",
                Birthday = new DateTime(1998, 4, 6),
                Signature = "感觉自己萌萌哒",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "芽芊",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/13.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1998, 4, 9),
                Signature = "再来一个汉堡",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "好人",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/14.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1998, 6, 9),
                Signature = "卑鄙的男人离老娘远点",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "西兰花",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/15.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1994, 9, 6),
                Signature = "走街串巷还得靠颜值",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "正版李天天",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/16.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1987, 7, 25),
                Signature = "你的酒窝没有酒，我却醉的像条狗",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "MrEmbc",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/17.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1999, 6, 12),
                Signature = "说晚安的这个阶段大概要花两小时",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "像雾像雨又像风",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/18.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1993, 4, 25),
                Signature = "感觉要飞拉",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "喝露水的Abby",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/19.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "苏州市",
                Birthday = new DateTime(1995, 8, 9),
                Signature = "风雪过后的圣诞树",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "Apple",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/20.jpg"),
                Cover = "../images/header.png",
                Province = "四川省",
                City = "成都市",
                Birthday = new DateTime(1993, 9, 10),
                Signature = "让我召唤神龙",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "死神眼泪",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/21.jpg"),
                Cover = "../images/header.png",
                Province = "上海",
                City = "上海市",
                Birthday = new DateTime(1991, 8, 17),
                Signature = "吓得我瓜子都掉了",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "一个人的一切",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/22.jpg"),
                Cover = "../images/header.png",
                Province = "上海",
                City = "上海市",
                Birthday = new DateTime(1985, 8, 21),
                Signature = "你不来我不老",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "一张白纸",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/23.jpg"),
                Cover = "../images/header.png",
                Province = "上海",
                City = "上海市",
                Birthday = new DateTime(1995, 7, 9),
                Signature = "我又没骂你，为何喷我",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "南海龙王",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/24.jpg"),
                Cover = "../images/header.png",
                Province = "河北省",
                City = "石家庄市",
                Birthday = new DateTime(1980, 7, 8),
                Signature = "得饶狗时切饶狗",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "小魔仙",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/25.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "淮安市",
                Birthday = new DateTime(1996, 11, 23),
                Signature = "其实我想要的很简单，只是没人懂",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "月影前行",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/26.jpg"),
                Cover = "../images/header.png",
                Province = "上海",
                City = "上海市",
                Birthday = new DateTime(1991, 2, 3),
                Signature = "人生真是寂寞如雪，有趣",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "蜜糖",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/27.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1992, 5, 21),
                Signature = "你的甜言蜜语犹豫毒药",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "夏天",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/28.jpg"),
                Cover = "../images/header.png",
                Province = "上海",
                City = "上海市",
                Birthday = new DateTime(1992, 5, 14),
                Signature = "啦啦啦啦德玛西亚",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "月球的星星",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/29.jpg"),
                Cover = "../images/header.png",
                Province = "四川省",
                City = "成都市",
                Birthday = new DateTime(1989, 5, 14),
                Signature = "原来没脑子这么可怕",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "芒果",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/30.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1989, 5, 14),
                Signature = "天啦噜，好怕怕",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "风中劲草",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/31.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1980, 8, 8),
                Signature = "春天花会开，鸟儿自由自在",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "墨墨墨墨色",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/32.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1997, 10, 8),
                Signature = "我的运气都给你了",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "执念",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/33.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "苏州市",
                Birthday = new DateTime(1992, 4, 7),
                Signature = "你们还真是胆子大",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "Dean",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/34.jpg"),
                Cover = "../images/header.png",
                Province = "四川省",
                City = "成都市",
                Birthday = new DateTime(1993, 9, 7),
                Signature = "我是一个小二逼",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "懒人",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/35.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "苏州市",
                Birthday = new DateTime(1995, 2, 18),
                Signature = "卖花儿，卖花啦",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "懒人",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/36.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "苏州市",
                Birthday = new DateTime(1995, 2, 18),
                Signature = "卖花儿，卖花啦",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "美希",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/37.jpg"),
                Cover = "../images/header.png",
                Province = "上海",
                City = "上海市",
                Birthday = new DateTime(1998, 3, 17),
                Signature = "给你们看我的小宝贝",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "啊花",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/38.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1993, 4, 16),
                Signature = "这么久了都没人撩我",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "浪味仙",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Init/UserAvatar/39.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1998, 5, 16),
                Signature = "集美貌与才华于一身的女大学生",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "卡西莫多",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Init/UserAvatar/40.jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1991, 5, 14),
                Signature = "然并卵",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "曼陀罗",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1999, 2, 8),
                Signature = "么么哒",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "偶尼酱",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "苏州市",
                Birthday = new DateTime(1999, 3, 8),
                Signature = "南方知我梦 吹梦到西洲",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "其实你不懂",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "苏州市",
                Birthday = new DateTime(1996, 4, 19),
                Signature = "你真的懂我吗",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "静静的呆子",
                Password = password,
                Sex = 0,
                Avatar = Server.MapPath("~/Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "扬州市",
                Birthday = new DateTime(1987, 12, 18),
                Signature = "我知道我很帅",
                Number = BuildNumber()
            });

            users.Add(new User
            {
                NickName = "大果",
                Password = password,
                Sex = 1,
                Avatar = Server.MapPath("~/Images/User/sysavatar" + new Random().Next(1, 36) + ".jpg"),
                Cover = "../images/header.png",
                Province = "江苏省",
                City = "南京市",
                Birthday = new DateTime(1993, 12, 12),
                Signature = "一花一世界",
                Number = BuildNumber()
            });

            users.ForEach(x =>
            {
                x.Avatar = Thumb(x.Avatar, baseUrl, "UserAvatar", 0);
                Thread.Sleep(2000);
            });

            db.AddMany<User>(users);
        }

        /// <summary>
        /// 初始化文章
        /// </summary>
        protected void InitArticle()
        {
            var articles = new List<Article>();
            var articleParts = new List<ArticlePart>();

            articles.Add(new Article
            {
                Title = "测试文章1",
                Province = "江苏省",
                City = "南京市",
                Cover = Server.MapPath("~/Init/Article/1.jpg"),
                CreateUserNumber = "1",
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Views = new Random().Next(200, 10000),
                Goods = new Random().Next(100),
                Keeps = new Random().Next(100),
                ArticlePart = new List<ArticlePart>
                {
                    //new ArticlePart(string.Empty,Enum_ArticlePart.Pic,0,Server.MapPath("~/Init/Article/1.jpg")),
                    //new ArticlePart(string.Empty,Enum_ArticlePart.Pic,0,Server.MapPath("~/Init/Article/2.jpg")),
                    //new ArticlePart(string.Empty,Enum_ArticlePart.Pic,0,Server.MapPath("~/Init/Article/3.jpg"))
                }
            });

            articles.ForEach(x =>
            {
                x.Number = BuildNumber();
                x.Cover = Thumb(x.Cover, baseUrl, "Article", 0);
                Thread.Sleep(2000);

                x.ArticlePart.ForEach(y =>
                {
                    y.ArticleNumber = x.Number;

                    if (y.Types == Enum_ArticlePart.Pic)
                    {
                        y.Introduction = Thumb(y.Introduction, baseUrl, "Article", 0);
                        Thread.Sleep(2000);
                    }
                });
                articleParts.AddRange(x.ArticlePart);
            });

            db.AddMany<Article>(articles);
            db.AddMany<ArticlePart>(articleParts);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="url">原始图片本地路径</param>
        /// <param name="baseUrl">基本路径</param>
        /// <param name="standards">缩略图规格</param>
        /// <param name="isDraw">是否添加水印</param>
        protected string Thumb(string fileUrl, string baseUrl, string standards, int isDraw)
        {
            try
            {
                string random = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000);
                using (FileStream fsRead = new FileStream(fileUrl, FileMode.Open))
                {
                    byte[] heByte = new byte[(int)fsRead.Length];
                    fsRead.Read(heByte, 0, heByte.Length);
                    MemoryStream ms = new MemoryStream(heByte);

                    #region  保存缩略图

                    UploadConfig.ConfigItem config = UploadConfig.Instance.GetConfig(standards);
                    if (config != null)
                    {
                        //缩略图存放根目录
                        string strFile = System.Web.HttpContext.Current.Server.MapPath(config.SavePath) + "/" + DateTime.Now.ToString("yyyyMMdd");
                        if (!Directory.Exists(strFile))
                        {
                            Directory.CreateDirectory(strFile);
                        }
                        Image image = Image.FromStream(ms, true);
                        ///生成缩略图（多种规格的）
                        int i = 0;
                        foreach (UploadConfig.ThumbMode mode in config.ModeList)
                        {
                            ///保存缩略图地址
                            i++;
                            MakeThumbnail(image, mode.Mode, mode.Width, mode.Height, isDraw, strFile + "\\" + random + "_" + i.ToString() + ".jpg");
                        }
                        image.Dispose();
                    }

                    #endregion

                    #region  保存原图

                    string savePath = System.Web.HttpContext.Current.Server.MapPath("~/Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/");
                    if (!Directory.Exists(savePath))
                    {
                        Directory.CreateDirectory(savePath);
                    }
                    savePath = savePath + "\\" + random + "_0" + ".jpg";
                    Image image2 = Image.FromStream(ms, true);

                    //添加水印
                    if (isDraw == 1)
                    {
                        image2 = WaterMark(image2);
                    }
                    image2.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    image2.Dispose();

                    #endregion

                    fileUrl = baseUrl + "Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + random + "_0" + ".jpg";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("InitController_Init" + ex.Message, ex);
            }
            return fileUrl;
        }
    }
}
