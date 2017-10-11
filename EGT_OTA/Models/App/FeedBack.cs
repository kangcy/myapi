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
    /// 意见反馈
    /// </summary>
    [Serializable]
    public class FeedBack : BaseModelShort
    {
        /// <summary>
        /// 内容
        /// </summary>
        [SubSonicStringLength(2000), SubSonicNullString]
        public string Summary { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [SubSonicStringLength(500), SubSonicNullString]
        public string QQ { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [SubSonicStringLength(2500), SubSonicNullString]
        public string Cover { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public int Status { get; set; }

        [SubSonicIgnore]
        public string NickName { get; set; }

        [SubSonicIgnore]
        public string Avatar { get; set; }

        [SubSonicIgnore]
        public string CreateDateText { get; set; }
    }
}