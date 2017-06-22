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
    /// 用户操作
    /// </summary>
    [Serializable]
    public class UserAction
    {
        /// <summary>
        /// ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string CreateUserNumber { get; set; }

        /// <summary>
        /// （文章、用户）编号
        /// </summary>
        [SubSonicLongString]
        public string ActionInfo { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int ActionType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTimeText { get; set; }

        [SubSonicIgnore]
        public List<ArticleInfoJson> ArticleInfoJson { get; set; }

        [SubSonicIgnore]
        public List<UserInfoJson> UserInfoJson { get; set; }

        [SubSonicIgnore]
        public UserInfoJson UserInfo { get; set; }
    }

    public class ArticleInfoJson
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string Cover { get; set; }
        public int ArticlePower { get; set; }
        public string Title { get; set; }
    }

    public class UserInfoJson
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
    }

    public class UserActionJson
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
        public int ActionType { get; set; }
        public string CreateTime { get; set; }
        public List<ArticleInfoJson> ArticleInfoJson { get; set; }
        public List<UserInfoJson> UserInfoJson { get; set; }
    }
}