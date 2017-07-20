using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Models;
using EGT_OTA.Helper;
using EGT_OTA.Controllers.Filter;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 后台
    /// </summary>
    public class BackController : BaseController
    {
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录审核
        /// </summary>
        [BackPower]
        public ActionResult CheckLogin()
        {
            try
            {
                return Json(new { result = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_CheckLogin:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        #region  文章

        /// <summary>
        /// 文章
        /// </summary>
        public ActionResult Article()
        {
            var pager = new Pager();
            var query = new SubSonic.Query.Select(provider).From<Article>().Where<Article>(x => x.ID > 0);
            var title = SqlFilter(ZNRequest.GetString("Title"));
            if (!string.IsNullOrWhiteSpace(title))
            {
                query.And("Title").Like("%" + title + "%");
            }
            var usernumber = ZNRequest.GetString("usernumber");
            if (!string.IsNullOrWhiteSpace(usernumber))
            {
                query = query.And("CreateUserNumber").IsEqualTo(usernumber);
            }
            var status = ZNRequest.GetInt("status", -1);
            if (status > -1)
            {
                query = query.And("Status").IsEqualTo(status);
            }
            var submission = ZNRequest.GetInt("submission", -1);
            if (submission > -1)
            {
                query = query.And("Submission").IsEqualTo(submission);
            }
            var articlepower = ZNRequest.GetInt("articlepower", -1);
            if (articlepower > -1)
            {
                query = query.And("ArticlePower").IsEqualTo(articlepower);
            }

            var recordCount = query.GetRecordCount();
            var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Article>();


            if (list.Count > 0)
            {
                List<string> userids = new List<string>();
                list.ForEach(x =>
                {
                    userids.Add(x.CreateUserNumber);
                });
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Cover", "Number").From<User>().Where("Number").In(userids.ToArray()).ExecuteTypedList<User>();
                var articletypes = GetArticleType();

                list.ForEach(x =>
                {
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (user != null)
                    {
                        var articletype = articletypes.FirstOrDefault(y => y.ID == x.TypeID);
                        x.TypeName = articletype == null ? "" : articletype.Name;
                        x.UserID = user.ID;
                        x.NickName = user.NickName;
                        x.Avatar = user.Avatar;
                        x.UserCover = user.Cover;
                    }
                    x.CreateDateText = x.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                });
            }
            ViewBag.title = title;
            ViewBag.usernumber = usernumber;
            ViewBag.status = status;
            ViewBag.submission = submission;
            ViewBag.articlepower = articlepower;
            ViewBag.RecordCount = recordCount;
            ViewBag.CurrPage = pager.Index;
            ViewBag.PageSize = pager.Size;
            ViewBag.List = list;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 文章取消审核
        /// </summary>
        [BackPower]
        public ActionResult ArticleAudit()
        {
            try
            {
                var id = ZNRequest.GetInt("id");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(provider).Set("Status").EqualTo(Enum_Status.Audit).Where<Article>(x => x.ID == article.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleAudit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 文章删除
        /// </summary>
        [BackPower]
        public ActionResult ArticleDelete()
        {
            try
            {
                var id = ZNRequest.GetInt("ArticleID");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(provider).Set("Status").EqualTo(Enum_Status.DELETE).Where<Article>(x => x.ID == article.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleDelete:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  投稿

        /// <summary>
        /// 投稿文章
        /// </summary>
        public ActionResult RecommendArticle()
        {
            var pager = new Pager();
            var query = new SubSonic.Query.Select(provider).From<ArticleRecommend>().Where<ArticleRecommend>(x => x.ID > 0);
            var articlenumber = SqlFilter(ZNRequest.GetString("articlenumber"));
            if (!string.IsNullOrWhiteSpace(articlenumber))
            {
                query = query.And("ArticleNumber").IsEqualTo(articlenumber);
            }
            var usernumber = ZNRequest.GetString("usernumber");
            if (!string.IsNullOrWhiteSpace(usernumber))
            {
                query = query.And("CreateUserNumber").IsEqualTo(usernumber);
            }
            var status = ZNRequest.GetInt("status", -1);
            if (status > -1)
            {
                query = query.And("Status").IsEqualTo(status);
            }
            var recordCount = query.GetRecordCount();
            var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<ArticleRecommend>();

            List<ArticleRecommendJson> newlist = new List<ArticleRecommendJson>();

            if (list.Count > 0)
            {
                List<string> userids = new List<string>();
                List<string> articleids = new List<string>();
                list.ForEach(x =>
                {
                    userids.Add(x.CreateUserNumber);
                    articleids.Add(x.ArticleNumber);
                });
                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Title", "TypeID", "City", "Province", "District", "Street", "DetailName", "Status").From<Article>().Where("Number").In(articleids.ToArray()).ExecuteTypedList<Article>();
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number", "Sex").From<User>().Where("Number").In(userids.ToArray()).ExecuteTypedList<User>();
                var articletypes = GetArticleType();

                list.ForEach(x =>
                {
                    var article = articles.FirstOrDefault(y => y.Number == x.ArticleNumber);
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (user != null)
                    {
                        ArticleRecommendJson model = new ArticleRecommendJson();
                        var articletype = articletypes.FirstOrDefault(y => y.ID == article.TypeID);
                        model.TypeName = articletype == null ? "" : articletype.Name;
                        model.Title = article.Title;
                        model.Status = article.Status;
                        model.Province = article.Province;
                        model.City = article.City;
                        model.District = article.District;
                        model.Street = article.Street;
                        model.DetailName = article.DetailName;
                        model.ArticleNumber = article.Number;
                        model.NickName = user.NickName;
                        model.Avatar = user.Avatar;
                        model.Sex = user.Sex;
                        model.UserNumber = user.Number;
                        model.CreateDate = x.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                        newlist.Add(model);
                    }
                });
            }
            ViewBag.articlenumber = articlenumber;
            ViewBag.usernumber = usernumber;
            ViewBag.status = status;
            ViewBag.RecordCount = recordCount;
            ViewBag.CurrPage = pager.Index;
            ViewBag.PageSize = pager.Size;
            ViewBag.List = newlist;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 投稿审核
        /// </summary>
        [BackPower]
        public ActionResult RecommendArticleAudit()
        {
            try
            {
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<ArticleRecommend>().Where<ArticleRecommend>(x => x.Status == Enum_Status.Audit);
                var recordCount = query.GetRecordCount();
                if (recordCount == 0)
                {
                    return Json(new { result = true, records = 0, totalpage = 1 }, JsonRequestBehavior.AllowGet);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<ArticleRecommend>();
                return Json(new { result = true, currpage = pager.Index, records = recordCount, totalpage = totalPage, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleAudit:" + ex.Message);
            }
            return Json(new { result = true, records = 0, totalpage = 1 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  评论

        #endregion

        #region  用户

        /// <summary>
        /// 用户
        /// </summary>
        public ActionResult User()
        {
            var pager = new Pager();
            var query = new SubSonic.Query.Select(provider).From<User>().Where<User>(x => x.ID > 0);
            var nickname = SqlFilter(ZNRequest.GetString("nickname"));
            if (!string.IsNullOrWhiteSpace(nickname))
            {
                query.And("NickName").Like("%" + nickname + "%");
            }
            var usernumber = ZNRequest.GetString("usernumber");
            if (!string.IsNullOrWhiteSpace(usernumber))
            {
                query = query.And("Number").IsEqualTo(usernumber);
            }

            //状态
            var status = ZNRequest.GetInt("status", -1);
            if (status > -1)
            {
                query = query.And("Status").IsEqualTo(status);
            }

            //性别
            var sex = ZNRequest.GetInt("sex", -1);
            if (sex > -1)
            {
                query = query.And("Sex").IsEqualTo(sex);
            }

            //是否推荐
            var isrecommend = ZNRequest.GetInt("isrecommend", -1);
            if (isrecommend > -1)
            {
                query = query.And("IsRecommend").IsEqualTo(isrecommend);
            }

            //启用打赏
            var ispay = ZNRequest.GetInt("ispay", -1);
            if (ispay > -1)
            {
                query = query.And("IsPay").IsEqualTo(ispay);
            }

            var recordCount = query.GetRecordCount();
            var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<User>();

            ViewBag.nickname = nickname;
            ViewBag.usernumber = usernumber;
            ViewBag.status = status;
            ViewBag.sex = sex;
            ViewBag.isrecommend = isrecommend;
            ViewBag.ispay = ispay;
            ViewBag.RecordCount = recordCount;
            ViewBag.CurrPage = pager.Index;
            ViewBag.PageSize = pager.Size;
            ViewBag.List = list;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        #endregion

        #region  订单

        #endregion

        #region  反馈

        #endregion

        #region  日志

        #endregion
    }
}
