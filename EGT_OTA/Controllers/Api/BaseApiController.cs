using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using EGT_OTA.Models;
using CommonTools;
using SubSonic.Repository;
using System.Text.RegularExpressions;
using EGT_OTA.Helper;
using System.Drawing;
using EGT_OTA.Helper.Config;
using System.Web.Http;
using SubSonic.DataProviders;
using IRedis;
using EGT_OTA.Redis;
using System.Threading;

namespace EGT_OTA.Controllers.Api
{
    public class BaseApiController : ApiController
    {
        protected readonly SimpleRepository db = BasicRepository.GetRepo();
        protected readonly IDataProvider provider = BasicRepository.GetProvider();
        protected static readonly RedisBase redis = RedisHelper.Redis;

        //默认管理员账号
        protected readonly string Admin_Name = System.Web.Configuration.WebConfigurationManager.AppSettings["admin_name"];
        protected readonly string Admin_Password = System.Web.Configuration.WebConfigurationManager.AppSettings["admin_password"];
        protected readonly string Base_Url = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"];

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
                this.Size = ZNRequest.GetInt("rows", 20);
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
                LogHelper.ErrorLoger.Error(ex.Message);
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
        /// 格式化时间显示
        /// </summary>
        protected string FormatTime(DateTime date)
        {
            var totalSeconds = Convert.ToInt32((DateTime.Now - date).TotalSeconds);
            if (totalSeconds < 0)
            {
                return "刚刚";
            }
            var hour = (totalSeconds / 3600);
            var year = 24 * 365;
            if (hour > year)
            {
                return Convert.ToInt32(hour / year) + "年前";
            }
            else if (hour > 24 * 10)
            {
                return date.ToString("MM-dd");
            }
            else if (hour > 24)
            {
                return Convert.ToInt32(hour / 24) + "天前";
            }
            else if (hour > 0)
            {
                return Convert.ToInt32(hour) + "小时前";
            }
            else
            {
                var minute = totalSeconds / 60;
                if (minute > 0)
                {
                    return Convert.ToInt32(minute) + "分钟前";
                }
                else
                {
                    if (totalSeconds > 0)
                        return totalSeconds + "秒前";
                    else
                        return "刚刚";
                }
            }
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
        protected void MakeThumbnail(User user, Image originalImage, string mode, int width, int height, int isDraw, string thumbnailPath)
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
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;//设置高质量插值法  
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
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
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
                    g.Clear(Color.Transparent);//清空画布并以透明背景色填充 
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
            if (list == null)
            {
                return new List<ArticleJson>();
            }
            if (list.Count == 0)
            {
                return new List<ArticleJson>();
            }


            //文章编号集合
            var array = list.Select(x => x.Number).ToArray();
            var articletypes = AppHelper.GetArticleType();
            var parts = new SubSonic.Query.Select(provider).From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic).And("ArticleNumber").In(array).OrderAsc("SortID").ExecuteTypedList<ArticlePart>();

            List<string> userids = new List<string>();
            list.ForEach(x =>
            {
                userids.Add(x.CreateUserNumber);
            });

            List<string> articleids = new List<string>();
            list.ForEach(x =>
            {
                articleids.Add(x.Number);
            });

            var users = new SubSonic.Query.Select(provider, "ID", "NickName", "Avatar", "Cover", "Signature", "Number", "IsPay").From<User>().Where("Number").In(userids.ToArray()).ExecuteTypedList<User>();

            var comments = new SubSonic.Query.Select(provider, "ID", "ArticleNumber", "ParentCommentNumber").From<Comment>().Where("ArticleNumber").In(articleids.ToArray()).And("ParentCommentNumber").IsEqualTo("").ExecuteTypedList<Comment>();

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
                    model.UserCover = user.Cover;
                    model.Signature = user.Signature;
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
                    model.Comments = comments.Count(y => y.ArticleNumber == x.Number);
                    model.IsFollow = fans.Count(y => y.ToUserNumber == x.CreateUserNumber);
                    model.IsZan = zans.Count(y => y.ArticleNumber == x.Number);
                    model.IsKeep = keeps.Count(y => y.ArticleNumber == x.Number);
                    model.UserNumber = x.CreateUserNumber;
                    model.Cover = x.Cover;
                    model.CreateDate = FormatTime(x.CreateDate);
                    model.TypeName = articletype == null ? "" : articletype.Name;
                    model.ArticlePart = parts.Where(y => y.ArticleNumber == x.Number).OrderBy(y => y.ID).Take(3).ToList();
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
