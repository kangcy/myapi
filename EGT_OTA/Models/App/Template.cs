/************************************************************************************ 
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
    public class TemplateCombination
    {
        /// <summary>
        /// 头部排版类型
        /// </summary>
        public int ArticleHead { get; set; }

        /// <summary>
        /// 音乐类型
        /// </summary>
        public int Music { get; set; }

        /// <summary>
        /// 标题颜色
        /// </summary>
        public string TitleColor { get; set; }

        /// <summary>
        /// 昵称颜色
        /// </summary>
        public string NickNameColor { get; set; }

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
        public int MarginTiop { get; set; }

        /// <summary>
        /// 自定义半透明蒙层颜色
        /// </summary>
        public string BlurColor { get; set; }

        /// <summary>
        /// 图片背景
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 漂浮
        /// </summary>
        public string SnowyUrl { get; set; }
    }
}