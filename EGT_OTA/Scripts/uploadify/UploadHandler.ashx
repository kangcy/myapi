﻿<%@ WebHandler Language="C#" Class="UploadHandler" %>

using System;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Web.UI;
using System.Web.SessionState;
using System.Drawing;
using MyWeb.Models;
using MyWeb.BLL;
using System.Text;
using CommonTools;

public class UploadHandler : IHttpHandler
{
    private static object lockObject = new object();

    //文件类型
    public static string TxtExtensions = "doc,docx,docm,dotx,dotm,txt,xml,rft,htm,html,mht,mhtml,wps,wtf";
    public static string XlsExtensions = "xls,xlsm,xlsb,xlsm,csv";
    public static string ImageExtensions = "jpg,jpeg,jpe,jfif,png,tif,tiff,gif,bmp,dib";
    public static string UpdateExtensions = "zip,rar";

    public void ProcessRequest(HttpContext context)
    {
        ///上传图片并生成不同规格的缩略图

        lock (lockObject)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Charset = "utf-8";
            HttpPostedFile file = context.Request.Files["Filedata"];///取得文件名(不含路径)
            if (file == null)
            {
                context.Response.Write("请选择您要上传的文件");
                return;
            }
            string Filename = Path.GetFileName(file.FileName);
            ///保存原始图片到服务器上
            try
            {
                string savePath = HttpContext.Current.Server.MapPath("/Accessory/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                savePath = savePath + "\\" + Filename;

                file.SaveAs(savePath);

                context.Response.Write("/Accessory/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/" + Filename);
            }
            catch (Exception ex)
            {
                context.Response.Write("上传附件失败，原因：" + ex.Message);
                return;
            }
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
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
    public void MakeThumbnail(Image originalImage, string mode, int width, int height, int isDraw, string thumbnailPath)
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
    public Image WaterMark(Image image)
    {
        ///读取水印配置
        CommonConfig.ConfigItem watermarkmodel = CommonConfig.Instance.GetConfig("WaterMark");

        if (watermarkmodel != null)
        {
            if (watermarkmodel.Cate == 1) //判断水印类型
            {
                ///水印图片
                Image copyImage = Image.FromFile(HttpContext.Current.Server.MapPath("/Image/WaterMark/" + watermarkmodel.ImageUrl));
                int width = 0;
                int height = 0;
                switch (watermarkmodel.Location)
                {
                    case 1: width = 0; height = 0; break;
                    case 2: width = (image.Width - copyImage.Width) / 2; height = 0; break;
                    case 3: width = image.Width - copyImage.Width; height = 0; break;
                    case 4: width = 0; height = (image.Height - copyImage.Height) / 2; break;
                    case 5: width = (image.Width - copyImage.Width) / 2; height = (image.Height - copyImage.Height) / 2; break;
                    case 6: width = image.Width - copyImage.Width; height = (image.Height - copyImage.Height) / 2; break;
                    case 7: width = 0; height = image.Height - copyImage.Height; break;
                    case 8: width = (image.Width - copyImage.Width) / 2; height = image.Height - copyImage.Height; break;
                    case 9: width = image.Width - copyImage.Width; height = image.Height - copyImage.Height; break;
                }
                Graphics g = Graphics.FromImage(image);
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
}