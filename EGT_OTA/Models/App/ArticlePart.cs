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
using CommonTools;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 文章部分
    /// </summary>
    [Serializable]
    public class ArticlePart : BaseModelShort
    {
        public ArticlePart() { }

        public ArticlePart(string number, int type, int sortId, string introduction, string introExpand)
        {
            this.ArticleNumber = number;
            this.Introduction = introduction;
            this.IntroExpand = introExpand;
            this.Types = type;
            this.SortID = sortId;
            this.CreateDate = DateTime.Now;
            this.CreateIP = Tools.GetClientIP;
            this.Status = Enum_Status.Approved;
        }

        /// <summary>
        /// 文章编号
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ArticleNumber { get; set; }

        /// <summary>
        /// 类型（1：图片,2：文字,3：视频）
        /// </summary>
        public int Types { get; set; }

        /// <summary>
        /// 详细
        /// </summary>
        [SubSonicLongString, SubSonicNullString]
        public string Introduction { get; set; }

        /// <summary>
        /// 详细扩展
        /// </summary>
        [SubSonicNullString]
        public string IntroExpand { get; set; }

        public int SortID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }


    }

    public class ArticlePartJson
    {
        public ArticlePartJson() { }
        public ArticlePartJson(int id, string url)
        {
            this.ID = id;
            this.Url = url;
        }

        public int ID { get; set; }

        public string Url { get; set; }
    }

    public class PartJson
    {
        public string ID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 内容拓展
        /// </summary>
        public string IntroExpand { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int PartType { get; set; }

        /// <summary>
        /// 状态（0-未操作，1-新增，2-编辑，3-删除）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortID { get; set; }

        /// <summary>
        /// 是否临时
        /// </summary>
        public int Temporary { get; set; }
    }

    public class PicJson
    {
        public string CreateDate { get; set; }

        public List<ArticlePart> List { get; set; }
    }
}