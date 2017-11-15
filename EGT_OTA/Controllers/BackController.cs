using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Models;
using EGT_OTA.Helper;
using EGT_OTA.Controllers.Filter;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 后台
    /// </summary>
    public class BackController : BaseController
    {
        public ActionResult Login()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 登录
        /// 初始密码：s84VDKwjLE8=  （123456）
        /// </summary>
        public ActionResult UserLogin()
        {
            try
            {
                var phone = ZNRequest.GetString("phone");
                var password = ZNRequest.GetString("password");
                if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { result = false, message = "请填写用户账号和密码" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Phone == phone);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }

                if (user.Password != DesEncryptHelper.Encrypt(password))
                {
                    return Json(new { result = false, message = "密码错误" }, JsonRequestBehavior.AllowGet);
                }
                if (user.UserRole != Enum_UserRole.Administrator && user.UserRole != Enum_UserRole.SuperAdministrator)
                {
                    return Json(new { result = false, message = "没有权限", code = Enum_ErrorCode.NoPower }, JsonRequestBehavior.AllowGet);
                }
                CookieHelper.SetCookie("Back", user.WeiXin + user.QQ);
                return Json(new { result = true, message = user.WeiXin + user.QQ, xwp = user.Number }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_CheckLogin:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public ActionResult LoginOut()
        {
            CookieHelper.ClearCookie("Back");
            return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        [BackPower]
        public ActionResult Password()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public ActionResult ResetPassword()
        {
            try
            {
                var password = ZNRequest.GetString("password");
                var passwordagain = ZNRequest.GetString("passwordagain");
                if (string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { result = false, message = "请填写新密码" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(passwordagain))
                {
                    return Json(new { result = false, message = "请确认新密码" }, JsonRequestBehavior.AllowGet);
                }
                if (password != passwordagain)
                {
                    return Json(new { result = false, message = "两次密码不一致" }, JsonRequestBehavior.AllowGet);
                }

                var usernumber = ZNRequest.GetString("xwp");

                User user = db.Single<User>(x => x.Number == usernumber);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                if (user.UserRole != Enum_UserRole.Administrator && user.UserRole != Enum_UserRole.SuperAdministrator)
                {
                    return Json(new { result = false, message = "没有权限", code = Enum_ErrorCode.NoPower }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Password").EqualTo(DesEncryptHelper.Encrypt(password)).Where<User>(x => x.ID == user.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ResetPassword:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        #region  文章

        /// <summary>
        /// 文章
        /// </summary>
        [BackPower]
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
            var articlenumber = ZNRequest.GetString("articlenumber");
            if (!string.IsNullOrWhiteSpace(articlenumber))
            {
                query = query.And("Number").IsEqualTo(articlenumber);
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
            var recommend = ZNRequest.GetInt("recommend", -1);
            if (recommend > -1)
            {
                query = query.And("Recommend").IsEqualTo(recommend);
            }
            var articletype = ZNRequest.GetInt("articletype", -1);
            if (articletype > -1)
            {
                query = query.And("TypeID").IsEqualTo(articletype);
            }
            var template = ZNRequest.GetInt("template", -1);
            if (template > -1)
            {
                if (template == 2)
                {
                    query = query.And("Template").IsGreaterThanOrEqualTo(template);
                }
                else
                {
                    query = query.And("Template").IsEqualTo(template);
                }
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
                        var type = articletypes.FirstOrDefault(y => y.ID == x.TypeID);
                        x.TypeName = type == null ? "" : type.Name;
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
            ViewBag.recommend = recommend;
            ViewBag.articlepower = articlepower;
            ViewBag.template = template;
            ViewBag.RecordCount = recordCount;
            ViewBag.CurrPage = pager.Index;
            ViewBag.PageSize = pager.Size;
            ViewBag.List = list;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        /// <summary>
        /// 文章置顶、取消置顶、推荐、取消推荐
        /// </summary>
        public ActionResult ArticleTop()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                var recommend = ZNRequest.GetInt("status");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(provider).Set("Recommend").EqualTo(recommend).Where<Article>(x => x.ID == article.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleTop:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 文章取消审核
        /// </summary>
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
                var result = new SubSonic.Query.Update<Article>(provider).Set("Status").EqualTo(Enum_Status.Delete).Where<Article>(x => x.ID == article.ID).Execute() > 0;
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

        /// <summary>
        /// 详情
        /// </summary>
        public ActionResult ArticleInfo()
        {
            try
            {
                var number = ZNRequest.GetString("key");
                if (string.IsNullOrWhiteSpace(number))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                Article model = db.Single<Article>(x => x.Number == number);
                if (model == null)
                {
                    return Json(new { result = false, message = "信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var usernumber = ZNRequest.GetString("xwp");
                if (string.IsNullOrWhiteSpace(usernumber))
                {
                    return Json(new { result = false, message = "无访问权限" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.Number == usernumber);
                if (user == null)
                {
                    return Json(new { result = false, message = "无访问权限" }, JsonRequestBehavior.AllowGet);
                }
                if (user.UserRole != Enum_UserRole.Administrator && user.UserRole != Enum_UserRole.SuperAdministrator)
                {
                    return Json(new { result = false, message = "无访问权限" }, JsonRequestBehavior.AllowGet);
                }

                //创建人
                User createUser = db.Single<User>(x => x.Number == model.CreateUserNumber);
                if (createUser != null)
                {
                    model.NickName = createUser == null ? "" : createUser.NickName;
                    model.Avatar = createUser == null ? "" : createUser.Avatar;
                    model.AutoMusic = createUser.AutoMusic;
                    model.ShareNick = createUser.ShareNick;
                }

                //文章部分
                model.ArticlePart = new SubSonic.Query.Select(provider).From<ArticlePart>().Where<ArticlePart>(x => x.ArticleNumber == model.Number).OrderAsc("SortID").ExecuteTypedList<ArticlePart>();

                model.CreateDateText = model.CreateDate.ToString("yyyy-MM-dd");

                //模板配置
                model.BackgroundJson = db.Single<Background>(x => x.ArticleNumber == model.Number && x.IsUsed == Enum_Used.Approved);
                if (model.Template >= 0)
                {
                    model.TemplateJson = GetArticleTemplate().FirstOrDefault(x => x.ID == model.Template);
                }

                if (model.ColorTemplate > 0)
                {
                    model.ColorTemplateJson = GetColorTemplate().FirstOrDefault(x => x.ID == model.ColorTemplate);
                }

                //漂浮装扮
                var custom = db.Single<ArticleCustom>(x => x.ArticleNumber == model.Number);
                if (custom != null)
                {
                    model.Showy = custom.ShowyUrl;
                    model.MusicID = custom.MusicID;
                    model.MusicName = custom.MusicName;
                    model.MusicUrl = custom.MusicUrl;
                    model.Transparency = custom.Transparency;
                    model.MarginTop = custom.MarginTop;
                }

                return Json(new { result = true, message = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("HomeController_BackInfo:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 文章编辑
        /// </summary>
        public ActionResult ArticleEdit()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                var articletype = ZNRequest.GetInt("articletype");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章不存在" }, JsonRequestBehavior.AllowGet);
                }

                var articletypes = GetArticleType();
                var type = articletypes.FirstOrDefault(y => y.ID == articletype);
                if (type == null)
                {
                    return Json(new { result = false, message = "分类不存在" }, JsonRequestBehavior.AllowGet);
                }

                var result = new SubSonic.Query.Update<Article>(provider).Set("TypeID").EqualTo(articletype).Where<Article>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleEdit:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重新定位
        /// </summary>
        public ActionResult ArticleLocation()
        {
            var message = "";
            try
            {
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                Article model = db.Single<Article>(x => x.Number == ArticleNumber);
                if (model == null)
                {
                    return Json(new { result = false, message = "文章不存在" }, JsonRequestBehavior.AllowGet);
                }
                var Province = ZNRequest.GetString("Province");
                var City = ZNRequest.GetString("City");
                var District = ZNRequest.GetString("District");
                var Street = ZNRequest.GetString("Street");
                var DetailName = ZNRequest.GetString("DetailName");
                var CityCode = ZNRequest.GetString("CityCode");
                var Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                var Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                var result = new SubSonic.Query.Update<Article>(provider)
                    .Set("Province").EqualTo(Province)
                    .Set("City").EqualTo(City)
                    .Set("District").EqualTo(District)
                    .Set("Street").EqualTo(Street)
                    .Set("DetailName").EqualTo(DetailName)
                    .Set("CityCode").EqualTo(CityCode)
                    .Set("Latitude").EqualTo(Latitude)
                    .Set("Longitude").EqualTo(Longitude)
                    .Where<Article>(x => x.Number == ArticleNumber).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_ArticleLocation:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  投稿

        /// <summary>
        /// 投稿文章
        /// </summary>
        [BackPower]
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
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        /// <summary>
        /// 投稿审核通过
        /// </summary>
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
                var result = new SubSonic.Query.Update<ArticleRecommend>(provider).Set("Status").EqualTo(Enum_Status.Delete).Where<ArticleRecommend>(x => x.ID == id).Execute() > 0;

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
        [BackPower]
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
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        /// <summary>
        /// 评论删除
        /// </summary>
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
        [BackPower]
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
                query = query.And("RelatedNumber").IsEqualTo(usernumber);
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
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        /// <summary>
        /// 用户推荐
        /// </summary>
        public ActionResult UserRecommend()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                User model = db.Single<User>(x => x.ID == id);
                if (model == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("IsRecommend").EqualTo(Enum_Recommend.Approved).Where<User>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_UserRecommend:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户取消推荐
        /// </summary>
        public ActionResult UserNoRecommend()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                User model = db.Single<User>(x => x.ID == id);
                if (model == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("IsRecommend").EqualTo(Enum_Recommend.Audit).Where<User>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_UserNoRecommend:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户删除
        /// </summary>
        public ActionResult UserDelete()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                User model = db.Single<User>(x => x.ID == id);
                if (model == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Status").EqualTo(Enum_Status.Audit).Where<User>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_UserDelete:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户激活
        /// </summary>
        public ActionResult UserApproved()
        {
            var message = "";
            try
            {
                var id = ZNRequest.GetInt("id");
                User model = db.Single<User>(x => x.ID == id);
                if (model == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<User>(provider).Set("Status").EqualTo(Enum_Status.Approved).Where<User>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_UserApproved:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重新定位
        /// </summary>
        public ActionResult UserLocation()
        {
            var message = "";
            try
            {
                var UserNumber = ZNRequest.GetString("UserNumber");
                User model = db.Single<User>(x => x.Number == UserNumber);
                if (model == null)
                {
                    return Json(new { result = false, message = "用户不存在" }, JsonRequestBehavior.AllowGet);
                }
                var Province = ZNRequest.GetString("Province");
                var City = ZNRequest.GetString("City");
                var District = ZNRequest.GetString("District");
                var Street = ZNRequest.GetString("Street");
                var DetailName = ZNRequest.GetString("DetailName");
                var CityCode = ZNRequest.GetString("CityCode");
                var Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                var Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                var result = new SubSonic.Query.Update<User>(provider)
                    .Set("Province").EqualTo(Province)
                    .Set("City").EqualTo(City)
                    .Set("District").EqualTo(District)
                    .Set("Street").EqualTo(Street)
                    .Set("DetailName").EqualTo(DetailName)
                    .Set("CityCode").EqualTo(CityCode)
                    .Set("Latitude").EqualTo(Latitude)
                    .Set("Longitude").EqualTo(Longitude)
                    .Where<User>(x => x.Number == UserNumber).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Back_UserLocation:" + ex.Message);
                message = ex.Message;
            }
            return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
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
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        #endregion

        #region  反馈

        /// <summary>
        /// 反馈
        /// </summary>
        [BackPower]
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
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        /// <summary>
        /// 反馈处理
        /// </summary>
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

        #endregion

        #region  红包

        /// <summary>
        /// 红包
        /// </summary>
        public ActionResult Red()
        {
            var pager = new Pager();
            var query = new SubSonic.Query.Select(provider).From<Red>().Where<Red>(x => x.ID > 0);
            var tousernumber = ZNRequest.GetString("tousernumber");
            if (!string.IsNullOrWhiteSpace(tousernumber))
            {
                query = query.And("ToUserNumber").IsEqualTo(tousernumber);
            }
            var redtype = ZNRequest.GetInt("redtype", -1);
            if (redtype > -1)
            {
                query = query.And("RedType").IsEqualTo(redtype);
            }
            var status = ZNRequest.GetInt("status", -1);
            if (status > -1)
            {
                query = query.And("Status").IsEqualTo(status);
            }
            var recordCount = query.GetRecordCount();
            var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
            var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Red>();

            List<RedJson> newlist = new List<RedJson>();

            if (list.Count > 0)
            {
                //被打赏人
                var tousers = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Number").From<User>().Where("Number").In(list.Select(x => x.ToUserNumber).ToArray()).ExecuteTypedList<User>();

                list.ForEach(x =>
                {
                    var touser = tousers.FirstOrDefault(y => y.Number == x.ToUserNumber);
                    if (touser == null)
                    {
                        return;
                    }
                    var model = new RedJson();
                    model.Price = x.Price;
                    model.RedType = x.RedType;
                    model.Status = x.Status;
                    model.CreateDateText = x.CreateDate.ToString("yyyy-MM-dd hh:mm:ss");
                    model.UserID = touser.ID;
                    model.UserName = touser.NickName;
                    model.UserAvatar = touser.Avatar;
                    model.ToUserNumber = x.ToUserNumber;
                    model.ID = x.ID;
                    model.Number = x.Number;
                    newlist.Add(model);
                });
            }
            ViewBag.tousernumber = tousernumber;
            ViewBag.redtype = redtype;
            ViewBag.status = status;
            ViewBag.RecordCount = recordCount;
            ViewBag.CurrPage = pager.Index;
            ViewBag.PageSize = pager.Size;
            ViewBag.List = newlist;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        #endregion

        #region 创建

        /// <summary>
        /// 创建
        /// </summary>
        [BackPower]
        public ActionResult Add()
        {
            var id = ZNRequest.GetInt("id");
            var number = ZNRequest.GetString("xwp");
            var user = db.Single<User>(x => x.Number == number);

            //我的关联账号
            var users = db.Find<User>(x => x.RelatedNumber == number).ToList();
            users.ForEach(x =>
            {
                x.NickName = BaseHelper.FilterEmoji(x.NickName);
            });
            ViewBag.Users = users;

            ViewBag.NickName = user.NickName;
            ViewBag.Avatar = user.Avatar;
            ViewBag.UserNumber = user.Number;
            ViewBag.UserID = user.ID;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            ViewBag.ArticleID = id;
            return View();
        }

        /// <summary>
        /// 音乐
        /// </summary>
        public ActionResult Music()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 图片
        /// </summary>
        public ActionResult Pic()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 漂浮
        /// </summary>
        public ActionResult Showy()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 高德地图
        /// </summary>
        public ActionResult Map()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 自定义
        /// </summary>
        public ActionResult Custom()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 自定义
        /// </summary>
        public ActionResult CustomSetting()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 视频
        /// </summary>
        public ActionResult EditVideo()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 视频
        /// </summary>
        public ActionResult VideoNotice()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 视频预览
        /// </summary>
        public ActionResult VideoPreview()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        [BackPower]
        public ActionResult AddComment()
        {
            var number = ZNRequest.GetString("xwp");
            var users = db.Find<User>(x => x.RelatedNumber == number).ToList();
            users.ForEach(x =>
            {
                x.NickName = BaseHelper.FilterEmoji(x.NickName);
            });
            ViewBag.Users = users;
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            return View();
        }

        /// <summary>
        /// 关联账号注册
        /// </summary>
        [BackPower]
        public ActionResult TemporaryReg()
        {
            ViewBag.RootUrl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
            ViewBag.key = ZNRequest.GetString("key");
            ViewBag.xwp = ZNRequest.GetString("xwp");
            return View();
        }

        #endregion
    }
}
