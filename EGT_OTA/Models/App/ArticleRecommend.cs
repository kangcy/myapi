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
    /// 投稿记录
    /// </summary>
    [Serializable]
    public class ArticleRecommend : BaseModelShort
    {
        /// <summary>
        /// 文章
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ArticleNumber { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int Status { get; set; }
    }

    public class ArticleRecommendJson
    {
        public ArticleRecommendJson() { }

        public string Title { get; set; }
        public string TypeName { get; set; }
        public int Status { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string DetailName { get; set; }
        public string ArticleNumber { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public int Sex { get; set; }
        public string CreateDate { get; set; }
        public string UserNumber { get; set; }
    }
}