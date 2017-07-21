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
            var message = "";
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
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
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
                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Title", "TypeID", "City", "Province", "District", "Street", "DetailName", "Submission").From<Article>().Where("Number").In(articleids.ToArray()).ExecuteTypedList<Article>();
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
                        model.Submission = article.Submission;
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
                        model.Status = x.Status;
                        model.ID = x.ID;
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
        /// 投稿审核通过
        /// </summary>
        [BackPower]
        public ActionResult RecommendArticleAudit()
        {
            var message = "";
            try
            {
                //审核记录
                var id = ZNRequest.GetInt("id");
                var recommend = db.Single<ArticleRecommend>(x => x.ID == id);
                if (recommend == null)
                {
                    return Json(new { result = false, message = "投稿信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<ArticleRecommend>(provider).Set("Status").EqualTo(Enum_Status.Approved).Where<ArticleRecommend>(x => x.ID == id).Execute() > 0;

                //文章审核通过
                result = new SubSonic.Query.Update<Article>(provider).Set("Submission").EqualTo(Enum_Submission.Approved).Where<Article>(x => x.Number == recommend.ArticleNumber).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_RecommendArticleAudit:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 投稿审核打回
        /// </summary>
        [BackPower]
        public ActionResult RecommendArticleNoAudit()
        {
            var message = "";
            try
            {
                //审核记录
                var id = ZNRequest.GetInt("id");
                var recommend = db.Single<ArticleRecommend>(x => x.ID == id);
                if (recommend == null)
                {
                    return Json(new { result = false, message = "投稿信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<ArticleRecommend>(provider).Set("Status").EqualTo(Enum_Status.DELETE).Where<ArticleRecommend>(x => x.ID == id).Execute() > 0;

                //文章审核不过
                result = new SubSonic.Query.Update<Article>(provider).Set("Submission").EqualTo(Enum_Submission.None).Where<Article>(x => x.Number == recommend.ArticleNumber).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleAudit:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  评论

        /// <summary>
        /// 评论
        /// </summary>
        public ActionResult Comment()
        {
            var pager = new Pager();
            var query = new SubSonic.Query.Select(provider).From<Comment>().Where<Comment>(x => x.ID > 0);
            var articlenumber = ZNRequest.GetString("articlenumber");
            if (!string.IsNullOrWhiteSpace(articlenumber))
            {
                query = query.And("ArticleNumber").IsEqualTo(articlenumber);
            }
            var usernumber = ZNRequest.GetString("usernumber");
            if (!string.IsNullOrWhiteSpace(usernumber))
            {
                query = query.And("CreateUserNumber").IsEqualTo(usernumber);
            }
            var recordCount = query.GetRecordCount();
            var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
            var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Comment>();

            List<Comment> newlist = new List<Models.Comment>();
            if (list.Count > 0)
            {
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).ToArray()).ExecuteTypedList<User>();
                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Title").From<Article>().Where("Number").In(list.Select(x => x.ArticleNumber).ToArray()).ExecuteTypedList<Article>();
                list.ForEach(x =>
                {
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    var article = articles.FirstOrDefault(y => y.Number == x.ArticleNumber);
                    if (user == null || article == null)
                    {
                        return;
                    }
                    var model = x;
                    model.CreateDateText = x.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                    model.UserID = user.ID;
                    model.NickName = user.NickName;
                    model.Avatar = user.Avatar;
                    model.Title = article.Title;
                    newlist.Add(model);
                });
            }
            ViewBag.articlenumber = articlenumber;
            ViewBag.usernumber = usernumber;
            ViewBag.RecordCount = recordCount;
            ViewBag.CurrPage = pager.Index;
            ViewBag.PageSize = pager.Size;
            ViewBag.List = newlist;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 评论删除
        /// </summary>
        [BackPower]
        public ActionResult CommentDelete()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                var comment = db.Single<Comment>(x => x.ID == id);
                if (comment == null)
                {
                    return Json(new { result = false, message = "评论信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = db.Delete<Comment>(id);
                if (result > 0)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_CommentDelete:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

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

        /// <summary>
        /// 订单
        /// </summary>
        public ActionResult Order()
        {
            var pager = new Pager();
            var query = new SubSonic.Query.Select(provider).From<Order>().Where<Order>(x => x.ID > 0);
            var articlenumber = ZNRequest.GetString("articlenumber");
            if (!string.IsNullOrWhiteSpace(articlenumber))
            {
                query = query.And("ToArticleNumber").IsEqualTo(articlenumber);
            }
            var usernumber = ZNRequest.GetString("usernumber");
            if (!string.IsNullOrWhiteSpace(usernumber))
            {
                query = query.And("CreateUserNumber").IsEqualTo(usernumber);
            }
            var tousernumber = ZNRequest.GetString("tousernumber");
            if (!string.IsNullOrWhiteSpace(tousernumber))
            {
                query = query.And("ToUserNumber").IsEqualTo(tousernumber);
            }
            //状态
            var status = ZNRequest.GetInt("status", -1);
            if (status > -1)
            {
                query = query.And("Status").IsEqualTo(status);
            }
            var recordCount = query.GetRecordCount();
            var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
            var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Order>();

            List<OrderJson2> newlist = new List<OrderJson2>();

            if (list.Count > 0)
            {
                //打赏人
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).ToArray()).ExecuteTypedList<User>();

                //被打赏人
                var tousers = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.ToUserNumber).ToArray()).ExecuteTypedList<User>();

                //被打赏文章
                var articles = new SubSonic.Query.Select(provider, "ID", "Number", "Title").From<Article>().Where("Number").In(list.Select(x => x.ToArticleNumber).ToArray()).ExecuteTypedList<Article>();

                list.ForEach(x =>
                {
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    var touser = tousers.FirstOrDefault(y => y.Number == x.ToUserNumber);
                    var article = articles.FirstOrDefault(y => y.Number == x.ToArticleNumber);
                    if (user == null || touser == null)
                    {
                        return;
                    }
                    var model = new OrderJson2();
                    model.Price = x.Price;
                    model.PayType = x.PayType;
                    model.Status = x.Status;
                    model.Anony = x.Anony;
                    model.CreateDate = x.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                    model.Title = article == null ? "" : article.Title;
                    model.ArticleNumber = article == null ? "" : article.Number;
                    model.FromUserID = user.ID;
                    model.FromUserName = user.NickName;
                    model.FromUserAvatar = user.Avatar;
                    model.FromUserNumber = user.Number;
                    model.ToUserID = touser.ID;
                    model.ToUserName = touser.NickName;
                    model.ToUserAvatar = touser.Avatar;
                    model.ToUserNumber = touser.Number;
                    newlist.Add(model);
                });
            }
            ViewBag.articlenumber = articlenumber;
            ViewBag.usernumber = usernumber;
            ViewBag.tousernumber = tousernumber;
            ViewBag.RecordCount = recordCount;
            ViewBag.CurrPage = pager.Index;
            ViewBag.PageSize = pager.Size;
            ViewBag.List = newlist;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        #endregion

        #region  反馈

        /// <summary>
        /// 反馈
        /// </summary>
        public ActionResult FeedBack()
        {
            var pager = new Pager();
            var query = new SubSonic.Query.Select(provider).From<FeedBack>().Where<FeedBack>(x => x.ID > 0);

            var usernumber = ZNRequest.GetString("usernumber");
            if (!string.IsNullOrWhiteSpace(usernumber))
            {
                query = query.And("CreateUserNumber").IsEqualTo(usernumber);
            }

            //状态
            var status = ZNRequest.GetInt("status", -1);
            if (status > -1)
            {
                query = query.And("Status").IsEqualTo(status);
            }

            var recordCount = query.GetRecordCount();
            var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
            var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<FeedBack>();
            List<FeedBack> newlist = new List<FeedBack>();
            if (list.Count > 0)
            {
                var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.CreateUserNumber).ToArray()).ExecuteTypedList<User>();
                list.ForEach(x =>
                {
                    var user = users.FirstOrDefault(y => y.Number == x.CreateUserNumber);
                    if (user == null)
                    {
                        return;
                    }
                    var model = x;
                    model.CreateDateText = x.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                    model.NickName = user.NickName;
                    model.Avatar = user.Avatar;
                    newlist.Add(model);
                });
            }
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
        /// 反馈处理
        /// </summary>
        [BackPower]
        public ActionResult FeedBackDeal()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                var recommend = db.Single<FeedBack>(x => x.ID == id);
                if (recommend == null)
                {
                    return Json(new { result = false, message = "投稿信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<FeedBack>(provider).Set("Status").EqualTo(Enum_Deal.Approved).Where<FeedBack>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_FeedBackDeal:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 反馈删除
        /// </summary>
        [BackPower]
        public ActionResult FeedBackDelete()
        {
            var message = "";
            try
            {
                //审核记录
                var id = ZNRequest.GetInt("id");
                var recommend = db.Single<FeedBack>(x => x.ID == id);
                if (recommend == null)
                {
                    return Json(new { result = false, message = "投稿信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<FeedBack>(provider).Set("Status").EqualTo(Enum_Deal.DELETE).Where<FeedBack>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_FeedBackDelete:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  日志

        #endregion
    }
}
