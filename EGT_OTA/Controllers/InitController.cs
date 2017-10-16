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
            InitArticleCustom();

            return Content("成功");
        }

        /// <summary>
        /// 批量处理文章自定义
        /// </summary>
        protected void InitArticleCustom()
        {
            var article = db.Find<Article>(x => x.MusicID > 0).ToList();
            article.ForEach(x =>
            {
                ArticleCustom custom = db.Single<ArticleCustom>(y => y.ArticleNumber == x.Number);
                if (custom == null)
                {
                    custom = new ArticleCustom();
                    custom.ArticleNumber = x.Number;
                }
                custom.MusicID = x.MusicID;
                custom.MusicName = x.MusicName;
                custom.MusicUrl = x.MusicUrl;
                if (custom.ID == 0)
                {
                    db.Add<ArticleCustom>(custom);
                }
                else
                {
                    db.Update<ArticleCustom>(custom);
                }
            });

            var background = db.All<Background>().ToList();
            background.ForEach(x =>
            {
                ArticleCustom custom = db.Single<ArticleCustom>(y => y.ArticleNumber == x.Number);
                if (custom == null)
                {
                    custom = new ArticleCustom();
                    custom.ArticleNumber = x.Number;
                }
                custom.Transparency = x.Transparency;
                if (custom.ID == 0)
                {
                    db.Add<ArticleCustom>(custom);
                }
                else
                {
                    db.Update<ArticleCustom>(custom);
                }
            });
        }
    }
}
