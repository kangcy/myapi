﻿/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  EGT_OTA.Models.App.Article 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/7/29 15:08:56 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/7/29 15:08:56 
 * 修改人  ：  
 * 版本号  ：V1.0.0.0 
 * 描述    ：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 模板
    /// </summary>
    [Serializable]
    public class Template
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 排版类型
        /// </summary>
        public int TemplateType { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// 背景缩略图
        /// </summary>
        public string ThumbUrl { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string Cover { get; set; }
    }

    /// <summary>
    /// 文章模板组合
    /// </summary>
    [Serializable]
    public class ArticleTemplate
    {
        public int ID { get; set; }

        /// <summary>
        /// 标题颜色
        /// </summary>
        public string TitleColor { get; set; }

        /// <summary>
        /// 昵称颜色
        /// </summary>
        public string UserColor { get; set; }

        /// <summary>
        /// 浏览次数颜色
        /// </summary>
        public string ViewColor { get; set; }

        /// <summary>
        /// 创建时间颜色
        /// </summary>
        public string TimeColor { get; set; }

        /// <summary>
        /// 顶部边距
        /// </summary>
        public int MarginTop { get; set; }

        /// <summary>
        /// 自定义半透明蒙层透明度
        /// </summary>
        public string Transparency { get; set; }

        /// <summary>
        /// 自定义半透明蒙层位置(0：不包含头部信息,1：包含用户头部信息)
        /// </summary>
        public int TransparencyFixed { get; set; }

        /// <summary>
        /// 页面背景颜色
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// 背景图片是否固定(0：随内容滚动、1：固定头部背景、2：背景滚动视差)
        /// </summary>
        public int CoverFixed { get; set; }

        /// <summary>
        /// 背景基础图片
        /// </summary>
        public string BackgroundImage { get; set; }

        /// <summary>
        /// 背景滚动模糊图片
        /// </summary>
        public string BackgroundBlurImage { get; set; }

        /// <summary>
        /// 头部图片
        /// </summary>
        public List<TopImage> TopImage { get; set; }

        /// <summary>
        /// 底部图片
        /// </summary>
        public string BottomImage { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>
        public string ThumbImage { get; set; }

        /// <summary>
        /// 背景图片Repeat
        /// </summary>
        public string BackgroundRepeat { get; set; }

        /// <summary>
        /// 背景图片Size
        /// </summary>
        public string BackgroundSize { get; set; }

        /// <summary>
        /// 背景定位
        /// </summary>
        public string BackgroundPosition { get; set; }

        /// <summary>
        /// 样式名称
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 背景描述
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否显示头像(-1不显示)
        /// </summary>
        public int ShowAvatar { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public AvatarImage Avatar { get; set; }

        /// <summary>
        /// 漂浮
        /// </summary>
        public string Showy { get; set; }

        /// <summary>
        /// 全局文字颜色（默认：#333）
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// 容器内间距（0：内外间距,1：外间距,2：内间距,3：无间距）
        /// </summary>
        public int PaddingFixed { get; set; }

        /// <summary>
        /// 显示标题（-1：不显示,其他显示）
        /// </summary>
        public int ShowTitle { get; set; }
    }

    /// <summary>
    /// 头像配置
    /// </summary>
    public class AvatarImage
    {
        public AvatarImage() { }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string BackgroundImage { get; set; }

        /// <summary>
        /// 背景宽
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 背景高
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// 头像宽
        /// </summary>
        public string SubWidth { get; set; }

        /// <summary>
        /// 头像位置
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// 头像位置
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// 头像间距
        /// </summary>
        public string MarginTop { get; set; }

        /// <summary>
        /// 头像间距
        /// </summary>
        public string MarginBottom { get; set; }

        /// <summary>
        /// 圆角
        /// </summary>
        public string BorderRadius { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// 边框
        /// </summary>
        public string Border { get; set; }
    }

    public class TopImage
    {
        public TopImage() { }

        public string Url { get; set; }

        public string Width { get; set; }

        public string Align { get; set; }
    }

    /// <summary>
    /// 文章主题色组合
    /// </summary>
    [Serializable]
    public class ColorTemplate
    {
        public int ID { get; set; }

        /// <summary>
        /// 标题颜色
        /// </summary>
        public string TitleColor { get; set; }

        /// <summary>
        /// 昵称颜色
        /// </summary>
        public string UserColor { get; set; }

        /// <summary>
        /// 浏览次数颜色
        /// </summary>
        public string ViewColor { get; set; }

        /// <summary>
        /// 创建时间颜色
        /// </summary>
        public string TimeColor { get; set; }
    }
}