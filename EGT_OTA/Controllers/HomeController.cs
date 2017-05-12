using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Helper;
using EGT_OTA.Models;

namespace EGT_OTA.Controllers
{
    public class HomeController : BaseController
    {
        /// <summary>
        /// 跳转文章详情
        /// </summary>
        public ActionResult Short(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
            }
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["share_url"] + "Home/index.html?key=" + number);
        }

        /// <summary>
        /// 跳转用户详情
        /// </summary>
        public ActionResult UserShort(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
            }
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["share_url"] + "Home/user.html?key=" + number);
        }

        /// <summary>
        /// 详情
        /// </summary>
        public ActionResult Info()
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
                if (model.Status == Enum_Status.DELETE)
                {
                    var newmodel = new Article();
                    newmodel.ID = model.ID;
                    newmodel.Title = model.Title;
                    newmodel.ArticlePower = model.ArticlePower;
                    newmodel.Status = model.Status;
                    return Json(new { result = true, message = newmodel }, JsonRequestBehavior.AllowGet);
                }

                string password = ZNRequest.GetString("ArticlePassword");

                //私密
                if (model.ArticlePower == Enum_ArticlePower.Myself)
                {
                    var newmodel = new Article();
                    newmodel.ID = model.ID;
                    newmodel.Title = model.Title;
                    newmodel.ArticlePower = Enum_ArticlePower.Myself;
                    return Json(new { result = true, message = newmodel }, JsonRequestBehavior.AllowGet);
                }
                //加密
                if (model.ArticlePower == Enum_ArticlePower.Password && model.ArticlePowerPwd != password)
                {
                    var newmodel = new Article();
                    newmodel.ID = model.ID;
                    newmodel.Title = model.Title;
                    newmodel.ArticlePower = Enum_ArticlePower.Password;
                    return Json(new { result = true, message = newmodel }, JsonRequestBehavior.AllowGet);
                }

                //浏览数
                new SubSonic.Query.Update<Article>(provider).Set("Views").EqualTo(model.Views + 1).Where<Article>(x => x.ID == model.ID).Execute();

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

                model.CreateDateText = DateTime.Now.ToString("yyyy-MM-dd");

                //模板配置
                model.BackgroundJson = db.Single<Background>(x => x.ArticleNumber == model.Number && x.IsUsed == Enum_Used.Approved);
                if (model.Template > 0)
                {
                    model.TemplateJson = GetArticleTemp().FirstOrDefault(x => x.ID == model.Template);
                    if (model.TemplateJson == null)
                    {
                        model.TemplateJson = new Template();
                    }
                }
                else
                {
                    model.TemplateJson = new Template();
                }

                return Json(new { result = true, message = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("HomeController_Info:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
