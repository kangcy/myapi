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
    /// 文章评论
    /// </summary>
    [Serializable]
    public class Comment : BaseModelShort
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ArticleNumber { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ArticleUserNumber { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [SubSonicStringLength(5000), SubSonicNullString]
        public string Summary { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string Number { get; set; }

        /// <summary>
        /// 回复评论ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ParentCommentNumber { get; set; }

        /// <summary>
        /// 回复用戶ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ParentUserNumber { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int Goods { get; set; }

        #region  定位

        /// <summary>
        /// 省份名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string Province { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string City { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string District { get; set; }

        /// <summary>
        /// 街道名称
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string Street { get; set; }

        /// <summary>
        /// 具体定位
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string DetailName { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [SubSonicStringLength(20), SubSonicNullString]
        public string CityCode { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 显示定位
        /// </summary>
        public int ShowPosition { get; set; }

        #endregion

        #region 扩展

        /// <summary>
        /// 创建月
        /// </summary>
        [SubSonicIgnore]
        public string Month { get; set; }

        /// <summary>
        /// 创建日
        /// </summary>
        [SubSonicIgnore]
        public string Day { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SubSonicIgnore]
        public int UserID { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        [SubSonicIgnore]
        public string UserNumber { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [SubSonicIgnore]
        public string NickName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [SubSonicIgnore]
        public string Avatar { get; set; }

        /// <summary>
        /// 父评论用户昵称
        /// </summary>
        [SubSonicIgnore]
        public string ParentNickName { get; set; }

        /// <summary>
        /// 父评论内容
        /// </summary>
        [SubSonicIgnore]
        public string ParentSummary { get; set; }

        /// <summary>
        /// 父评论ID
        /// </summary>
        [SubSonicIgnore]
        public int ParentCommentID { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [SubSonicIgnore]
        public string Title { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SubSonicIgnore]
        public string CreateDateText { get; set; }

        [SubSonicIgnore]
        public int IsZan { get; set; }

        [SubSonicIgnore]
        public int SubCommentCount { get; set; }

        [SubSonicIgnore]
        public int ArticleID { get; set; }

        #endregion
    }

    public class CommentJson
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string Summary { get; set; }
        public int Goods { get; set; }
        public string CreateDateText { get; set; }
        public int UserID { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string UserNumber { get; set; }
        public string Title { get; set; }
        public string ArticleNumber { get; set; }
        public string ArticleUserNumber { get; set; }
        public int ArticlePower { get; set; }
        /// <summary>
        /// 显示定位
        /// </summary>
        public int ShowPosition { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 子评论数量
        /// </summary>
        public int SubCommentCount { get; set; }
        public int IsZan { get; set; }
        public string SubUserName { get; set; }
        public string SubSummary { get; set; }
    }

    public class CommentJson2
    {
        public int ID { get; set; }
        public string Summary { get; set; }
        public int UserID { get; set; }
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
    }
}