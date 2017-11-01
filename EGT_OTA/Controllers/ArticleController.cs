using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Controllers.Filter;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 文章
    /// </summary>
    public class ArticleController : BaseController
    {
        /// <summary>
        /// 复制
        /// </summary>
        public ActionResult Copy()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "当前文章不存在" }, JsonRequestBehavior.AllowGet);
                }
                var ip = Tools.GetClientIP;
                var userNumber = user.Number;
                var number = article.Number;
                var result = false;
                var model = article;
                model.Title = article.Title + "(副本)";
                model.Province = ZNRequest.GetString("Province");
                model.City = ZNRequest.GetString("City");
                model.District = ZNRequest.GetString("District");
                model.Street = ZNRequest.GetString("Street");
                model.DetailName = ZNRequest.GetString("DetailName");
                model.CityCode = ZNRequest.GetString("CityCode");
                model.Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                model.Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                model.CreateUserNumber = userNumber;
                model.CreateDate = DateTime.Now;
                model.CreateIP = ip;
                model.UpdateUserNumber = userNumber;
                model.UpdateDate = DateTime.Now;
                model.UpdateIP = ip;
                model.Status = Enum_Status.Approved;
                model.Submission = Enum_Submission.TemporaryApproved;
                model.Views = 0;
                model.Goods = 0;
                model.Keeps = 0;
                model.Comments = 0;
                model.Pays = 0;
                model.Recommend = Enum_ArticleRecommend.None;
                model.ArticlePower = Enum_ArticlePower.Myself;
                model.Number = BuildNumber();
                model.ID = Tools.SafeInt(db.Add<Article>(model));
                result = model.ID > 0;

                if (result)
                {
                    //同步文章内容
                    List<ArticlePart> list = new List<ArticlePart>();
                    var parts = db.Find<ArticlePart>(x => x.ArticleNumber == number).ToList();
                    parts.ForEach(x =>
                    {
                        x.ArticleNumber = model.Number;
                        x.Status = Enum_Status.Delete;
                        x.CreateDate = DateTime.Now;
                        x.CreateUserNumber = userNumber;
                        x.CreateIP = ip;
                        list.Add(x);
                    });
                    db.AddMany<ArticlePart>(list);

                    //同步文章个性化设置
                    var custom = db.Single<ArticleCustom>(x => x.ArticleNumber == number);
                    if (custom != null)
                    {
                        custom.ArticleNumber = model.Number;
                        custom.ID = 0;
                        db.Add<ArticleCustom>(custom);
                    }

                    //同步文章自定义背景
                    var background = db.Find<Background>(x => x.ArticleNumber == number).ToList();
                    if (background.Count > 0)
                    {
                        background.ForEach(x =>
                        {
                            x.ArticleNumber = model.Number;
                            x.ID = 0;
                        });
                        db.AddMany<Background>(background);
                    }
                }
                if (result)
                {
                    return Json(new { result = true, message = model.ID }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Copy:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除
        /// </summary>
        public ActionResult Delete()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                if (article.CreateUserNumber != user.Number)
                {
                    return Json(new { result = false, message = "没有权限" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(provider).Set("Status").EqualTo(Enum_Status.Delete).Set("Submission").EqualTo(Enum_Submission.TemporaryApproved).Where<Article>(x => x.ID == article.ID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Delete:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpPost]
        public ActionResult Edit()
        {
            var parts = "";
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                Article model = new Article();
                model.ID = ZNRequest.GetInt("ArticleID");
                if (model.ID > 0)
                {
                    model = db.Single<Article>(x => x.ID == model.ID);
                    if (model == null)
                    {
                        model = new Article();
                    }
                }
                model.Title = SqlFilter(ZNRequest.GetString("Title"));
                model.Title = CutString(model.Title, 100);
                var dirtyword = HasDirtyWord(model.Title);
                if (!string.IsNullOrWhiteSpace(dirtyword))
                {
                    return Json(new { result = false, message = "您输入的标题含有敏感内容[" + dirtyword + "]，请检查后重试哦" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    model.Title = "我的小微篇";
                }
                model.Province = ZNRequest.GetString("Province");
                model.City = ZNRequest.GetString("City");
                model.District = ZNRequest.GetString("District");
                model.Street = ZNRequest.GetString("Street");
                model.DetailName = ZNRequest.GetString("DetailName");
                model.CityCode = ZNRequest.GetString("CityCode");
                model.Latitude = Tools.SafeDouble(ZNRequest.GetString("Latitude"));
                model.Longitude = Tools.SafeDouble(ZNRequest.GetString("Longitude"));
                model.Template = ZNRequest.GetInt("Template");
                model.ColorTemplate = ZNRequest.GetInt("ColorTemplate");
                model.UpdateUserNumber = user.Number;
                model.UpdateDate = DateTime.Now;
                model.UpdateIP = Tools.GetClientIP;
                model.Status = Enum_Status.Approved;
                model.Submission = Enum_Submission.TemporaryApproved;//默认临时投稿审核

                var result = false;
                if (model.ID == 0)
                {
                    var cover = ZNRequest.GetString("Cover");
                    if (string.IsNullOrWhiteSpace(cover))
                    {
                        return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                    }
                    var covers = cover.Split(',');
                    model.Cover = covers[0];
                    model.Recommend = Enum_ArticleRecommend.None;
                    model.TypeID = 0;
                    model.TypeIDList = "-0-0-";
                    model.ArticlePower = Enum_ArticlePower.Myself;
                    model.ArticlePowerPwd = string.Empty;
                    model.CreateUserNumber = user.Number;
                    model.CreateDate = DateTime.Now;
                    model.CreateIP = Tools.GetClientIP;
                    model.Number = BuildNumber();
                    model.ID = Tools.SafeInt(db.Add<Article>(model));
                    result = model.ID > 0;

                    //初始化文章段落
                    if (result)
                    {
                        for (var i = 0; i < covers.Length; i++)
                        {
                            ArticlePart part = new ArticlePart();
                            part.ArticleNumber = model.Number;
                            part.Types = Enum_ArticlePart.Pic;
                            part.Introduction = covers[i];
                            part.IntroExpand = string.Empty;
                            part.SortID = Tools.SafeInt(i);
                            part.Status = Enum_Status.Audit;
                            part.CreateDate = DateTime.Now;
                            part.CreateUserNumber = user.Number;
                            part.CreateIP = Tools.GetClientIP;
                            part.ID = Tools.SafeInt(db.Add<ArticlePart>(part));
                            result = part.ID > 0;
                        }
                    }
                    if (result)
                    {
                        ArticleType articleType = GetArticleType().FirstOrDefault<ArticleType>(x => x.ID == model.TypeID);
                        model.TypeName = articleType == null ? string.Empty : articleType.Name;

                        return Json(new
                        {
                            result = true,
                            message = new
                            {
                                ID = model.ID,
                                Number = model.Number,
                                ArticlePower = model.ArticlePower,
                                ArticlePowerPwd = model.ArticlePowerPwd,
                                ArticleType = model.TypeID,
                                ArticleTypeName = model.TypeName
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (model.CreateUserNumber != user.Number)
                    {
                        return Json(new { result = false, message = "没有权限" }, JsonRequestBehavior.AllowGet);
                    }

                    if (model.ArticlePower == Enum_ArticlePower.Public)
                    {
                        model.ArticlePower = Enum_ArticlePower.Myself;
                    }

                    if (string.IsNullOrWhiteSpace(model.Title))
                    {
                        model.Title = "我的小微篇";
                    }

                    result = db.Update<Article>(model) > 0;


                    //取消自定义模板启用
                    if (model.Template != 1)
                    {
                        var list = db.Find<Background>(x => x.ArticleNumber == model.Number && x.IsUsed == Enum_Used.Approved).ToList();
                        if (list.Count > 0)
                        {
                            list.ForEach(x =>
                            {
                                x.IsUsed = Enum_Used.Audit;
                            });
                            db.UpdateMany<Background>(list);
                        }
                    }


                    ArticleType articleType = GetArticleType().FirstOrDefault<ArticleType>(x => x.ID == model.TypeID);
                    model.TypeName = articleType == null ? string.Empty : articleType.Name;

                    parts = SqlFilter(ZNRequest.GetString("Part").Trim(), false, false);

                    if (!string.IsNullOrWhiteSpace(parts))
                    {
                        List<PartJson> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PartJson>>(parts);
                        list.ForEach(x =>
                        {
                            if (x.Status == 0)
                            {
                                //排序
                                var partid = Tools.SafeInt(x.ID);
                                var part = db.Single<ArticlePart>(y => y.ID == partid);
                                if (part != null)
                                {
                                    part.SortID = Tools.SafeInt(x.SortID);
                                    db.Update<ArticlePart>(part);
                                }
                            }
                            else if (x.Status == 1)
                            {
                                //新增
                                ArticlePart part = new ArticlePart();
                                part.ArticleNumber = model.Number;
                                part.Types = x.PartType;
                                part.Introduction = x.Introduction;
                                part.IntroExpand = x.IntroExpand;
                                part.SortID = Tools.SafeInt(x.SortID);
                                part.Status = Enum_Status.Audit;
                                part.CreateDate = DateTime.Now;
                                part.CreateUserNumber = user.Number;
                                part.CreateIP = Tools.GetClientIP;
                                part.ID = Tools.SafeInt(db.Add<ArticlePart>(part));
                            }
                            else if (x.Status == 2)
                            {
                                //编辑
                                var partid = Tools.SafeInt(x.ID);
                                var part = db.Single<ArticlePart>(y => y.ID == partid);
                                if (part != null)
                                {
                                    part.Introduction = x.Introduction;
                                    part.IntroExpand = x.IntroExpand;
                                    part.SortID = Tools.SafeInt(x.SortID);
                                    db.Update<ArticlePart>(part);
                                }
                            }
                            else if (x.Status == 3)
                            {
                                //是否临时删除删除
                                if (x.Temporary == 0)
                                {
                                    db.Delete<ArticlePart>(x.ID);
                                }
                            }
                        });
                    }
                }
                if (result)
                {
                    //更新自定义漂浮
                    var ShowyUrl = ZNRequest.GetString("ShowyUrl");
                    var MusicID = ZNRequest.GetInt("MusicID");
                    var MusicName = ZNRequest.GetString("MusicName");
                    var MusicUrl = ZNRequest.GetString("MusicUrl");
                    ArticleCustom custom = db.Single<ArticleCustom>(x => x.ArticleNumber == model.Number);
                    if (custom == null)
                    {
                        custom = new ArticleCustom();
                        custom.ArticleNumber = model.Number;
                    }
                    custom.ShowyUrl = ShowyUrl;
                    custom.MusicID = MusicID;
                    custom.MusicName = MusicName;
                    custom.MusicUrl = MusicUrl;
                    if (custom.ID == 0)
                    {
                        db.Add<ArticleCustom>(custom);
                    }
                    else
                    {
                        db.Update<ArticleCustom>(custom);
                    }

                    return Json(new
                    {
                        result = true,
                        message = new
                        {
                            ID = model.ID,
                            Number = model.Number,
                            ArticlePower = model.ArticlePower,
                            ArticlePowerPwd = model.ArticlePowerPwd,
                            TypeID = model.TypeID,
                            TypeName = model.TypeName
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Edit:" + ex.Message + "," + parts);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑模板
        /// </summary>
        [ArticlePower]
        public ActionResult EditArticleTemp()
        {
            try
            {
                var ArticleID = ZNRequest.GetInt("ArticleID");
                var Template = ZNRequest.GetInt("Template");
                var result = new SubSonic.Query.Update<Article>(provider).Set("Template").EqualTo(Template).Where<Article>(x => x.ID == ArticleID).Execute() > 0;
                if (result)
                {
                    //取消自定义模板启用
                    string ArticleNumber = ZNRequest.GetString("ArticleNumber");
                    if (!string.IsNullOrWhiteSpace(ArticleNumber) && Template != 1)
                    {
                        var list = db.Find<Background>(x => x.ArticleNumber == ArticleNumber && x.IsUsed == Enum_Used.Approved).ToList();
                        if (list.Count > 0)
                        {
                            list.ForEach(x =>
                            {
                                x.IsUsed = Enum_Used.Audit;
                            });
                            db.UpdateMany<Background>(list);
                        }
                    }
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticleTemp:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑封面
        /// </summary>
        [ArticlePower]
        public ActionResult EditArticleCover()
        {
            try
            {
                var ArticleID = ZNRequest.GetInt("ArticleID");
                var Cover = ZNRequest.GetString("Cover");
                if (string.IsNullOrWhiteSpace(Cover))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(provider).Set("Cover").EqualTo(Cover).Where<Article>(x => x.ID == ArticleID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticleCover:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑权限
        /// </summary>
        [ArticlePower]
        public ActionResult EditArticlePower()
        {
            try
            {
                var ArticleID = ZNRequest.GetInt("ArticleID");
                Article article = db.Single<Article>(x => x.ID == ArticleID);
                var ArticlePower = ZNRequest.GetInt("ArticlePower", Enum_ArticlePower.Myself);
                var ArticlePowerPwd = ZNRequest.GetString("ArticlePowerPwd");
                var result = new SubSonic.Query.Update<Article>(provider).Set("ArticlePower").EqualTo(ArticlePower).Set("ArticlePowerPwd").EqualTo(ArticlePowerPwd).Where<Article>(x => x.ID == ArticleID).Execute() > 0;
                if (result)
                {
                    //用户相册是否展示
                    var status = ArticlePower == Enum_ArticlePower.Public ? Enum_Status.Approved : Enum_Status.Audit;
                    new SubSonic.Query.Update<ArticlePart>(provider).Set("Status").EqualTo(status).Where<ArticlePart>(x => x.ArticleNumber == article.Number).Execute();

                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticlePower:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑分类
        /// </summary>
        [ArticlePower]
        public ActionResult EditArticleType()
        {
            try
            {
                var ArticleID = ZNRequest.GetInt("ArticleID");
                var TypeID = ZNRequest.GetInt("ArticleType");
                if (TypeID <= 0)
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var articleType = GetArticleType().FirstOrDefault<ArticleType>(x => x.ID == TypeID);
                if (articleType == null)
                {
                    return Json(new { result = false, message = "不存在当前类型" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(provider).Set("TypeID").EqualTo(TypeID).Set("TypeIDList").EqualTo(articleType.ParentIDList).Where<Article>(x => x.ID == ArticleID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticleType:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑背景
        /// </summary>
        [ArticlePower]
        public ActionResult EditBackground()
        {
            try
            {
                var ArticleID = ZNRequest.GetInt("ArticleID");
                var background = ZNRequest.GetString("Background");
                var result = new SubSonic.Query.Update<Article>(provider).Set("Background").EqualTo(background).Where<Article>(x => x.ID == ArticleID).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditBackground:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验权限
        /// </summary>
        public ActionResult CheckPowerPwd()
        {
            try
            {
                var ArticleID = ZNRequest.GetInt("ArticleID");
                if (ArticleID <= 0)
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var pwd = ZNRequest.GetString("ArticlePowerPwd");
                var result = db.Exists<Article>(x => x.ID == ArticleID && x.ArticlePower == Enum_ArticlePower.Password && x.ArticlePowerPwd == pwd && x.Status == Enum_Status.Approved);
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_CheckPowerPwd:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 模板列表
        /// </summary>
        public ActionResult Template()
        {
            try
            {
                var list = GetArticleTemplate().ToList();
                var result = new
                {
                    currpage = 1,
                    records = list.Count(),
                    totalpage = 1,
                    list = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Template:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 主题色模板列表
        /// </summary>
        public ActionResult ColorTemplate()
        {
            try
            {
                var list = GetColorTemplate().ToList();
                var result = new
                {
                    currpage = 1,
                    records = list.Count(),
                    totalpage = 1,
                    list = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_ColorTemplate:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 投稿
        /// </summary>
        [ArticlePower]
        public ActionResult Recommend()
        {
            try
            {
                var UserNumber = ZNRequest.GetString("UserNumber");
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                var ArticlePower = ZNRequest.GetInt("ArticlePower");
                if (string.IsNullOrWhiteSpace(UserNumber) || string.IsNullOrWhiteSpace(ArticleNumber))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }

                //判断文章权限,公开的才可以投稿
                var article = db.Single<Article>(x => x.Number == ArticleNumber);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                if (article.TypeID == 0)
                {
                    return Json(new { result = false, message = "请选择文章分类" }, JsonRequestBehavior.AllowGet);
                }
                if (article.ArticlePower != Enum_ArticlePower.Public)
                {
                    if (ArticlePower != Enum_ArticlePower.Public)
                    {
                        return Json(new { result = false, message = "公开文章才可以投稿哦" }, JsonRequestBehavior.AllowGet);
                    }
                }
                var time = DateTime.Now.AddDays(-1);
                var log = db.Single<ArticleRecommend>(x => x.CreateUserNumber == UserNumber && x.ArticleNumber == ArticleNumber && x.CreateDate > time);
                if (log != null)
                {
                    return Json(new { result = false, message = "每天只有1次投稿机会，上次投稿时间为：" + log.CreateDate.ToString("yyyy-MM-dd hh:mm:ss") }, JsonRequestBehavior.AllowGet);
                }

                //修改文章投稿记录
                var result = new SubSonic.Query.Update<Article>(provider).Set("Submission").EqualTo(Enum_Submission.Audit).Set("ArticlePower").EqualTo(Enum_ArticlePower.Public).Where<Article>(x => x.ID == article.ID).Execute() > 0;

                ArticleRecommend model = new ArticleRecommend();
                model.ArticleNumber = ArticleNumber;
                model.CreateUserNumber = UserNumber;
                model.CreateDate = DateTime.Now;
                model.CreateIP = Tools.GetClientIP;
                model.Status = Enum_Status.Audit;
                result = Tools.SafeInt(db.Add<ArticleRecommend>(model)) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Recommend:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 文章彻底删除
        /// </summary>
        public ActionResult DeleteCompletely()
        {
            try
            {
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                var result = new SubSonic.Query.Update<Article>(provider).Set("Status").EqualTo(Enum_Status.DeleteCompletely).Where<Article>(x => x.Number == ArticleNumber).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Article_DeleteCompletely:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 文章恢复
        /// </summary>
        public ActionResult Revoke()
        {
            try
            {
                var ArticleNumber = ZNRequest.GetString("ArticleNumber");
                var result = new SubSonic.Query.Update<Article>(provider).Set("Status").EqualTo(Enum_Status.Approved).Where<Article>(x => x.Number == ArticleNumber).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Article_Revoke:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
