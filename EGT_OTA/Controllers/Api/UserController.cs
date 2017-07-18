using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using CommonTools;
using EGT_OTA.Controllers.Filter;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json;

namespace EGT_OTA.Controllers.Api
{
    public class UserController : BaseApiController
    {
        /// <summary>
        /// 我的相册
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/User/Pic")]
        public string Pic()
        {
            ApiResult result = new ApiResult();
            try
            {
                var Number = ZNRequest.GetString("Number");
                var UserNumber = ZNRequest.GetString("UserNumber");
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic && x.Status != Enum_Status.DELETE);
                if (Number != UserNumber)
                {
                    query = query.And("Status").IsEqualTo(Enum_Status.Approved);
                }
                query = query.And("CreateUserNumber").IsEqualTo(UserNumber);
                var recordCount = query.GetRecordCount();
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<ArticlePart>();

                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = list
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_User_Pic:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 我的相册
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/User/Pic_1_3")]
        public string Pic_1_3()
        {
            ApiResult result = new ApiResult();
            try
            {
                var Number = ZNRequest.GetString("Number");
                var UserNumber = ZNRequest.GetString("UserNumber");

                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }

                var pager = new Pager();

                //日期分组
                var datequery = new SubSonic.Query.Select(provider, "ID", "CreateDate", "Status").From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic && x.Status != Enum_Status.DELETE && x.CreateUserNumber == UserNumber);
                if (Number != UserNumber)
                {
                    datequery = datequery.And("Status").IsEqualTo(Enum_Status.Approved);
                }
                var datelist = datequery.ExecuteTypedList<ArticlePart>();
                var newdatelist = new List<DateTime>();
                datelist.OrderByDescending(x => x.ID).GroupBy(x => x.CreateDate.ToString("yyyy-MM-dd")).ToList().ForEach(x =>
                {
                    newdatelist.Add(x.FirstOrDefault().CreateDate);
                });
                var recordCount = newdatelist.Count();
                if (recordCount == 0)
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                newdatelist = newdatelist.Skip((pager.Index - 1) * pager.Size).Take(pager.Size).ToList();

                //获取数据
                var query = new SubSonic.Query.Select(provider).From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic && x.Status != Enum_Status.DELETE && x.CreateUserNumber == UserNumber);
                if (Number != UserNumber)
                {
                    query = query.And("Status").IsEqualTo(Enum_Status.Approved);
                }
                query.And("CreateDate").IsGreaterThanOrEqualTo(newdatelist[newdatelist.Count - 1].ToString("yyyy-MM-dd 00:00:00"));
                query.And("CreateDate").IsLessThanOrEqualTo(newdatelist[0].ToString("yyyy-MM-dd 23:59:59"));
                var list = query.ExecuteTypedList<ArticlePart>();

                var newlist = new List<PicJson>();

                //组织输出
                newdatelist.ForEach(x =>
                {
                    PicJson pic = new PicJson();
                    pic.CreateDate = x.ToString("yyyy年MM月dd日");
                    pic.List = list.FindAll(y => y.CreateDate.Year == x.Year && y.CreateDate.Month == x.Month && y.CreateDate.Day == x.Day);
                    newlist.Add(pic);
                });

                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_User_Pic_1_3:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/User/Search")]
        public string Search()
        {
            ApiResult result = new ApiResult();
            try
            {
                var UserNumber = ZNRequest.GetString("Number");
                var nickname = ZNRequest.GetString("NickName");
                var pager = new Pager();
                var query = new SubSonic.Query.Select(provider).From<User>().Where<User>(x => x.Status == Enum_Status.Approved);

                if (!string.IsNullOrWhiteSpace(nickname))
                {
                    query.And("NickName").IsNotNull().And("NickName").Like("%" + nickname + "%");
                }

                if (!string.IsNullOrWhiteSpace(UserNumber))
                {
                    var black = db.Find<Black>(x => x.CreateUserNumber == UserNumber);
                    if (black.Count > 0)
                    {
                        var userids = black.Select(x => x.ToUserNumber).ToArray();
                        query = query.And("Number").NotIn(userids);
                    }
                }

                var recordCount = query.GetRecordCount();

                if (recordCount == 0)
                {
                    result.result = true;
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("LoginTimes").ExecuteTypedList<User>();

                var follows = db.Find<Fan>(x => x.CreateUserNumber == UserNumber).ToList();

                var newlist = (from l in list
                               select new
                               {
                                   ID = l.ID,
                                   NickName = l.NickName,
                                   Signature = l.Signature,
                                   Avatar = l.Avatar,
                                   Cover = l.Cover,
                                   Number = l.Number,
                                   IsFollow = follows.Exists(x => x.ToUserNumber == l.Number) ? 1 : 0
                               }).ToList();

                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api/User/Search:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 我的文章
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/User/Article_1_3")]
        public string Article_1_3()
        {
            ApiResult result = new ApiResult();
            try
            {
                var UserNumber = ZNRequest.GetString("UserNumber");
                var CurrUserNumber = ZNRequest.GetString("CurrUserNumber");
                if (string.IsNullOrWhiteSpace(UserNumber))
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var pager = new Pager();
                //日期分组
                var datequery = new SubSonic.Query.Select(provider, "ID", "CreateDate", "ArticlePower").From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved && x.CreateUserNumber == UserNumber);
                if (UserNumber != CurrUserNumber)
                {
                    datequery = datequery.And("ArticlePower").IsEqualTo(Enum_ArticlePower.Public);
                    datequery = datequery.And("Submission").IsGreaterThan(Enum_Submission.Audit);
                }
                var datelist = datequery.ExecuteTypedList<Article>();
                var newdatelist = new List<DateTime>();
                datelist.OrderByDescending(x => x.ID).GroupBy(x => x.CreateDate.ToString("yyyy-MM")).ToList().ForEach(x =>
                {
                    newdatelist.Add(x.FirstOrDefault().CreateDate);
                });

                var recordCount = newdatelist.Count();
                if (recordCount == 0)
                {
                    result.message = new { records = 0, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                newdatelist = newdatelist.Skip((pager.Index - 1) * pager.Size).Take(pager.Size).ToList();

                //获取数据
                var query = new SubSonic.Query.Select(provider, "ID", "Number", "CreateUserNumber", "Cover", "ArticlePower", "Status", "CreateDate").From<Article>().Where<Article>(x => x.Status == Enum_Status.Approved && x.CreateUserNumber == UserNumber);
                if (UserNumber != CurrUserNumber)
                {
                    query = query.And("ArticlePower").IsEqualTo(Enum_ArticlePower.Public);
                    query = query.And("Submission").IsGreaterThan(Enum_Submission.Audit);
                }

                query.And("CreateDate").IsGreaterThanOrEqualTo(newdatelist[newdatelist.Count - 1].ToString("yyyy-MM-01 00:00:00"));
                var day = DateTime.DaysInMonth(newdatelist[0].Year, newdatelist[0].Month); ;

                query.And("CreateDate").IsLessThanOrEqualTo(newdatelist[0].ToString("yyyy-MM-" + day + " 23:59:59"));
                var list = query.ExecuteTypedList<Article>();

                LogHelper.ErrorLoger.Error(Newtonsoft.Json.JsonConvert.SerializeObject(list));

                var newlist = new List<UserArticleJson>();

                //组织输出
                newdatelist.ForEach(x =>
                {
                    UserArticleJson model = new UserArticleJson();
                    model.List = new List<UserArticleSubJson>();
                    var items = list.FindAll(y => y.CreateDate.Year == x.Year && y.CreateDate.Month == x.Month);
                    items.ForEach(y =>
                    {
                        UserArticleSubJson item = new UserArticleSubJson();
                        item.ID = y.ID;
                        item.Number = y.Number;
                        item.CreateUserNumber = y.CreateUserNumber;
                        item.Cover = y.Cover;
                        item.ArticlePower = y.ArticlePower;
                        model.List.Add(item);
                    });
                    model.CreateDate = x.Year + "年" + x.Month + "月";
                    model.Count = model.List.Count;
                    newlist.Add(model);
                });

                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_User_Article_1_3:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}