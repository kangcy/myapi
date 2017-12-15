using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CommonTools;
using EGT_OTA.Helper;
using Newtonsoft.Json;
using EGT_OTA.Helper.Config;
using System.Drawing;
using EGT_OTA.Models;
using System.Drawing.Imaging;
using System.Threading;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 上传文件
    /// </summary>
    public class UploadController : BaseController
    {
        public static string TxtExtensions = ",doc,docx,docm,dotx,txt,xml,htm,html,mhtml,wps,";
        public static string XlsExtensions = ",xls,xlsm,xlsb,xlsm,";
        public static string ImageExtensions = ",jpg,jpeg,jpe,png,gif,bmp,";
        public static string CompressionExtensions = ",zip,rar,";
        public static string AudioExtensions = ",mp3,wav,";
        public static string VideoExtensions = ",mp4,avi,wmv,mkv,3gp,flv,rmvb,";

        //上传文件
        public ActionResult UploadFile()
        {
            var result = false;
            var message = string.Empty;
            var count = Request.Files.Count;
            if (count == 0)
            {
                return Json(new { result = result, message = "未上传任何文件" }, JsonRequestBehavior.AllowGet);
            }

            var folder = ZNRequest.GetString("folder");

            var file = Request.Files[0];
            string extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = "Other";
            }
            else
            {
                if (folder.ToLower() == "pic" && !ImageExtensions.Contains(extension.ToLower().Replace(".", "")))
                {
                    return Json(new { result = false, message = "上传文件格式不正确" }, JsonRequestBehavior.AllowGet);
                }
                if (folder.ToLower() == "music" && !AudioExtensions.Contains(extension.ToLower().Replace(".", "")))
                {
                    return Json(new { result = false, message = "上传文件格式不正确" }, JsonRequestBehavior.AllowGet);
                }
                if (folder.ToLower() == "video" && !VideoExtensions.Contains(extension.ToLower().Replace(".", "")))
                {
                    return Json(new { result = false, message = "上传文件格式不正确" }, JsonRequestBehavior.AllowGet);
                }
            }
            var url = string.Empty;
            try
            {
                string data = DateTime.Now.ToString("yyyy-MM-dd");
                string virtualPath = "~/Upload/" + folder + "/" + data;
                string savePath = this.Server.MapPath(virtualPath);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                string filename = Path.GetFileName(file.FileName);
                string code = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000);
                string fileExtension = Path.GetExtension(filename);//获取文件后缀名(.jpg)
                //filename = code + fileExtension;//重命名文件
                var name = ZNRequest.GetString("name");
                if (string.IsNullOrEmpty(name))
                {
                    filename = code + fileExtension;
                }
                else
                {
                    filename = name + fileExtension;
                }
                filename = filename.Replace("3gp", "mp4");
                savePath = savePath + "\\" + filename;
                file.SaveAs(savePath);
                url = "Upload/" + folder + "/" + data + "/" + filename;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UploadController_UploadFile" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = true, message = url }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 带缩略图上传
        /// </summary>
        public ActionResult Upload()
        {
            var error = string.Empty;
            try
            {
                var mimeType = "";
                string stream = ZNRequest.GetString("str");
                if (string.IsNullOrWhiteSpace(mimeType))
                {
                    if (stream.IndexOf("data:image/gif;base64,") > -1)
                    {
                        stream = stream.Replace("data:image/gif;base64,", "");
                        mimeType = "image/gif";
                    }
                }
                if (string.IsNullOrWhiteSpace(mimeType))
                {
                    if (stream.IndexOf("data:image/jpeg;base64,") > -1)
                    {
                        stream = stream.Replace("data:image/jpeg;base64,", "");
                    }
                    if (stream.IndexOf("data:image/png;base64,") > -1)
                    {
                        stream = stream.Replace("data:image/png;base64,", "");
                    }
                    if (stream.IndexOf("data:image/bmp;base64,") > -1)
                    {
                        stream = stream.Replace("data:image/bmp;base64,", "");
                    }
                }

                if (string.IsNullOrWhiteSpace(mimeType))
                {
                    mimeType = "image/jpeg";
                }

                System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(stream));
                string random = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000);

                string standards = ZNRequest.GetString("standard");///缩略图规格名称
                string number = ZNRequest.GetString("Number");

                if (string.IsNullOrWhiteSpace(standards))
                {
                    standards = "Article";
                }

                var filename = random + ".jpg";

                //保存原图
                var basePath = "Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + number + "/";
                string savePath = Server.MapPath("~/" + basePath);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                #region  保存缩略图

                var user = db.Single<User>(x => x.Number == number);
                if (!String.IsNullOrEmpty(standards))
                {
                    UploadConfig.ConfigItem config = UploadConfig.Instance.GetConfig(standards);
                    if (config != null)
                    {
                        int i = 0;
                        foreach (UploadConfig.ThumbMode mode in config.ModeList)
                        {
                            i++;
                            using (Bitmap Origninal = new Bitmap(ms))
                            {
                                Bitmap returnBmp = new Bitmap(Origninal.Width, Origninal.Height);
                                Graphics g = Graphics.FromImage(returnBmp);
                                g.Clear(Color.Transparent);//清空画布并以透明背景色填充 
                                g.DrawImage(Origninal, 0, 0, Origninal.Width, Origninal.Height);
                                g.Dispose();
                                MakeThumbnail(user, (Image)returnBmp, mode, savePath + "\\" + filename.Replace(".", "_" + i + "."));
                            }
                        }
                    }
                }

                #endregion

                Image image2 = Image.FromStream(ms, true);
                //原图不添加水印
                //if (isDraw == 1)
                //{
                //    image2 = WaterMark(image2, user);
                //}
                EncoderParameter p;
                EncoderParameters ps;
                ps = new EncoderParameters(1);
                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
                ps.Param[0] = p;
                ImageCodecInfo ii = GetCodecInfo(mimeType);
                image2.Save(savePath + "\\" + filename.Replace(".", "_0."), ii, ps);
                image2.Dispose();
                ms.Close();
                return Json(new
                {
                    result = true,
                    message = basePath + filename.Replace(".", "_0.")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                LogHelper.ErrorLoger.Error("UploadController_Upload" + ex.Message, ex);
            }
            return Json(new
            {
                result = true,
                message = ""
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 带缩略图上传
        /// </summary>
        public ActionResult UploadImage()
        {
            var standards = ZNRequest.GetString("standard");//缩略图规格名称
            var number = ZNRequest.GetString("number");//用户编号
            var user = db.Single<User>(x => x.Number == number);
            UploadConfig.ConfigItem config = null;
            if (!String.IsNullOrEmpty(standards))
            {
                config = UploadConfig.Instance.GetConfig(standards);
            }
            var folder = ZNRequest.GetString("folder");
            var basePath = "Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + number + "/";
            string savePath = Server.MapPath("~/" + basePath);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            var url = new List<string> { };
            int count = Request.Files.Count;

            for (int i = 0; i < count; i++)
            {
                try
                {
                    var file = Request.Files[i];
                    string filename = Path.GetFileName(file.FileName);
                    filename = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000) + Path.GetExtension(filename);

                    int l = file.ContentLength;
                    byte[] buffer = new byte[l];
                    Stream ms = file.InputStream;
                    Image image = Image.FromStream(ms, true);
                    //原图不添加水印
                    //if (isDraw == 1)
                    //{
                    //    image = WaterMark(image, user);
                    //}

                    ThumbImage(image, savePath + "\\" + filename.Replace(".", "_0."), 720);
                    //image.Save(savePath + "\\" + filename.Replace(".", "_0."));

                    #region  生成缩略图

                    if (config != null)
                    {
                        //生成缩略图（多种规格的）
                        int index = 0;
                        foreach (UploadConfig.ThumbMode mode in config.ModeList)
                        {
                            ///保存缩略图地址
                            index++;
                            using (Bitmap Origninal = new Bitmap(ms))
                            {
                                Bitmap returnBmp = new Bitmap(Origninal.Width, Origninal.Height);
                                Graphics g = Graphics.FromImage(returnBmp);
                                g.Clear(Color.Transparent);
                                g.DrawImage(Origninal, 0, 0, Origninal.Width, Origninal.Height);
                                g.Dispose();
                                MakeThumbnail(user, (Image)returnBmp, mode, savePath + "\\" + filename.Replace(".", "_" + index + "."));
                            }
                        }
                    }

                    ms.Dispose();
                    image.Dispose();

                    url.Add(basePath + filename.Replace(".", "_0."));

                    #endregion
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLoger.Error("UploadController_UploadImage" + ex.Message, ex);
                }
            }
            return Json(new
            {
                result = true,
                message = url.Count == 0 ? "" : string.Join(",", url)
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重置图片大小
        /// </summary>
        public ActionResult ResizeImage()
        {
            var date = ZNRequest.GetString("date");
            var standard = ZNRequest.GetString("standard");
            if (string.IsNullOrWhiteSpace(standard))
            {
                standard = "Article";
            }
            string folderPath = "";
            if (string.IsNullOrWhiteSpace(date))
            {
                folderPath = Server.MapPath("~/Upload/Images/" + standard);
            }
            else
            {
                folderPath = Server.MapPath("~/Upload/Images/" + standard + "/" + date);
            }
            ThumbDirectory(folderPath);
            return Content("成功");
        }

        public void ThumbDirectory(string folderPath)
        {
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            //子目录
            foreach (DirectoryInfo d in folder.GetDirectories())
            {
                ThumbDirectory(folderPath + "/" + d.Name);
            }
            //子文件
            foreach (FileInfo f in folder.GetFiles())
            {
                Image img = Image.FromFile(f.FullName, true);
                if (f.Name.IndexOf("_0") > 0)
                {
                    ThumbImage(img, f.FullName, 720);
                }
                if (f.Name.IndexOf("_1") > 0)
                {
                    ThumbImage(img, f.FullName, 500);
                }
                if (f.Name.IndexOf("_2") > 0)
                {
                    ThumbImage(img, f.FullName, 350);
                }
                Thread.Sleep(500);
            }
        }

        public void ThumbImage(Image originalImage, string thumbnailPath, int modewidth)
        {
            if (originalImage == null)
            {
                return;
            }
            int towidth = modewidth;
            int toheight = 0;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;//原图宽度
            int oh = originalImage.Height;//原图高度
            if (ow < modewidth)
            {
                towidth = ow;
                toheight = oh;
                originalImage.Dispose();
                return;
            }
            else
            {
                //指定宽，高按比例                      
                toheight = originalImage.Height * modewidth / originalImage.Width;
            }

            //新建一个bmp图片 
            using (Bitmap bitmap = new Bitmap(towidth, toheight))
            {
                //新建一个画板 
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;//设置高质量插值法  
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//设置高质量,低速度呈现平滑程度  
                    g.Clear(Color.Transparent);//清空画布并以透明背景色填充  
                    g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);//在指定位置并且按指定大小绘制原图片的指定部分  
                }
                var mineType = "image/jpeg";
                if (originalImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                {
                    mineType = "image/gif";
                }
                if (originalImage.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                {
                    mineType = "image/png";
                }
                originalImage.Dispose();
                try
                {
                    EncoderParameter p;
                    EncoderParameters ps;
                    ps = new EncoderParameters(1);
                    p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                    ps.Param[0] = p;
                    ImageCodecInfo ii = GetCodecInfo(mineType);
                    bitmap.Save(thumbnailPath, ii, ps);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
