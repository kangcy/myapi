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
                string stream = ZNRequest.GetString("str");
                stream = stream.IndexOf("data:image/jpeg;base64,") > -1 ? stream.Replace("data:image/jpeg;base64,", "") : stream;
                System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(stream));
                string random = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000);

                #region  保存缩略图

                string standards = ZNRequest.GetString("standard");///缩略图规格名称
                string number = ZNRequest.GetString("Number");

                int isDraw = 0;//是否生成水印
                int isThumb = 1;//是否生成缩略图
                var user = db.Single<User>(x => x.Number == number);
                if (user != null)
                {
                    isDraw = user.UseDraw;
                }

                if (standards != "Article")
                {
                    isDraw = 0;
                }

                if (isThumb == 1 && !String.IsNullOrEmpty(standards))
                {
                    UploadConfig.ConfigItem config = UploadConfig.Instance.GetConfig(standards);
                    if (config != null)
                    {
                        //缩略图存放根目录
                        string strFile = System.Web.HttpContext.Current.Server.MapPath(config.SavePath) + "/" + DateTime.Now.ToString("yyyyMMdd");
                        if (!Directory.Exists(strFile))
                        {
                            Directory.CreateDirectory(strFile);
                        }
                        //Image image = Image.FromStream(ms, true);
                        ///生成缩略图（多种规格的）
                        int i = 0;
                        foreach (UploadConfig.ThumbMode mode in config.ModeList)
                        {
                            ///保存缩略图地址
                            i++;
                            //MakeThumbnail(image, mode.Mode, mode.Width, mode.Height, isDraw, strFile + "\\" + random + "_" + i.ToString() + ".jpg");

                            using (Bitmap Origninal = new Bitmap(ms))
                            {
                                Bitmap returnBmp = new Bitmap(Origninal.Width, Origninal.Height);
                                Graphics g = Graphics.FromImage(returnBmp);
                                g.DrawImage(Origninal, 0, 0, Origninal.Width, Origninal.Height);
                                g.Dispose();
                                MakeThumbnail((Image)returnBmp, mode.Mode, mode.Width, mode.Height, isDraw, strFile + "\\" + random + "_" + i.ToString() + ".jpg");
                            }

                        }
                        //image.Dispose();
                    }
                }

                #endregion

                //保存原图
                string savePath = System.Web.HttpContext.Current.Server.MapPath("~/Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                savePath = savePath + "\\" + random + "_0" + ".jpg";
                Image image2 = Image.FromStream(ms, true);
                //添加水印
                if (isDraw == 1)
                {
                    image2 = WaterMark(image2);
                }


                EncoderParameter p;
                EncoderParameters ps;
                ps = new EncoderParameters(1);
                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                ps.Param[0] = p;
                ImageCodecInfo ii = GetCodecInfo("image/jpeg");

                //image2.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg, ps);
                image2.Save(savePath, ii, ps);
                image2.Dispose();
                ms.Close();

                return Json(new
                {
                    result = true,
                    message = ("Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + random + "_0" + ".jpg")
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

            int isDraw = 0;//是否生成水印
            int isThumb = 1;//是否生成缩略图
            var user = db.Single<User>(x => x.Number == number);
            if (user != null)
            {
                isDraw = user.UseDraw;
            }

            if (standards != "Article")
            {
                isDraw = 0;
            }

            UploadConfig.ConfigItem config = null;
            if (isThumb == 1 && !String.IsNullOrEmpty(standards))
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
                    System.Drawing.Bitmap image = new System.Drawing.Bitmap(ms);

                    image.Save(savePath + "\\" + filename.Replace(".", "_0."));

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
                                g.DrawImage(Origninal, 0, 0, Origninal.Width, Origninal.Height);
                                g.Dispose();
                                MakeThumbnail((Image)returnBmp, mode.Mode, mode.Width, mode.Height, isDraw, savePath + "\\" + filename.Replace(".", "_" + index + "."));
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
    }
}
