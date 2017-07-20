using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGT_OTA.Models;
using Newtonsoft.Json;
using CommonTools;
using EGT_OTA.Helper;

namespace EGT_OTA.Controllers
{
    public class CommentController : BaseController
    {
        /// <summary>
        /// 评论编辑
        /// </summary>
        [HttpPost]
        public ActionResult Edit_1_3()
        {
            ApiResult result = new ApiResult();
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    result.message = EnumBase.GetDescription(typeof(Enum_ErrorCode), Enum_ErrorCode.UnLogin);
                    result.code = Enum_ErrorCode.UnLogin;
                    return Json(result);
                }
                var ArticleNumber = Tools.SafeString(ZNRequest.GetString("ArticleNumber"));
                if (string.IsNullOrWhiteSpace(ArticleNumber))
                {
                    result.message = "文章信息异常";
                    return Json(result);
                }
                var summary = SqlFilter(UrlDecode(ZNRequest.GetString("Summary")), false, false);
                if (string.IsNullOrWhiteSpace(summary))
                {
                    result.message = "请填写评论内容";
                    return Json(result);
                }
                summary = CutString(summary, 2000);
                if (HasDirtyWord(summary))
                {
                    result.message = "您的输入内容含有敏感内容，请检查后重试哦";
                    return Json(result);
                }

                Article article = new SubSonic.Query.Select(provider, "ID", "Number", "CreateUserNumber").From<Article>().Where<Article>(x => x.Number == ArticleNumber).ExecuteSingle<Article>();
                if (article == null)
                {
                    result.message = "文章信息异常";
                    return Json(result);
                }

                //判断是否拉黑
                var black = db.Exists<Black>(x => x.CreateUserNumber == article.CreateUserNumber && x.ToUserNumber == user.Number);
                if (black)
                {
                    result.message = "没有权限";
                    return Json(result);
                }

                Comment model = new Comment();
                model.ArticleNumber = article.Number;
                model.ArticleUserNumber = article.CreateUserNumber;
                model.Summary = summary;
                model.Number = BuildNumber();
                model.CreateDate = DateTime.Now;
                model.CreateUserNumber = user.Number;
                model.CreateIP = Tools.GetClientIP;
                model.ParentCommentNumber = ZNRequest.GetString("ParentCommentNumber");
                model.ParentUserNumber = ZNRequest.GetString("ParentUserNumber");

                model.Province = ZNRequest.GetString("Province");
                model.City = ZNRequest.GetString("City");
                model.District = ZNRequest.GetString("District");
                model.Street = ZNRequest.GetString("Street");
                model.DetailName = ZNRequest.GetString("DetailName");
                model.CityCode = ZNRequest.GetString("CityCode");
                model.Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                model.Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                model.ShowPosition = ZNRequest.GetInt("ShowPosition");
                model.ID = Tools.SafeInt(db.Add<Comment>(model));
                if (model.ID > 0)
                {
                    result.result = true;
                    result.message = model.ID;

                    //推送
                    new AppHelper().Push(article.CreateUserNumber, article.ID, article.Number, user.NickName, Enum_PushType.Comment, false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Comment_Edit_1_3:" + ex.Message);
                result.message = ex.Message;
            }
            return Json(result);
        }
    }
}
