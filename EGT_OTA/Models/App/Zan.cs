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
    /// 文章点赞
    /// </summary>
    [Serializable]
    public class ArticleZan : BaseModelShort
    {
        /// <summary>
        /// 文章编号
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ArticleNumber { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ArticleUserNumber { get; set; }
    }

    /// <summary>
    /// 评论点赞
    /// </summary>
    [Serializable]
    public class CommentZan : BaseModelShort
    {
        /// <summary>
        /// 评论编号
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string CommentNumber { get; set; }
    }


    public class ZanJson
    {
        public int ID { get; set; }
        public string CreateDate { get; set; }

        //文章信息
        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string Cover { get; set; }
        public string CreateUserNumber { get; set; }
        public int ArticlePower { get; set; }

        //用户信息
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string UserNumber { get; set; }
    }
}