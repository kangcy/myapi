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
using EGT_OTA.Helper;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 背景
    /// </summary>
    [Serializable]
    public class Background
    {
        public Background()
        {
            this.ID = 0;
            this.ArticleNumber = "";
            this.Full = 0;
            this.High = 0;
            this.Transparency = 0;
            this.Number = "";
            this.CreateUserNumber = "";
        }

        /// <summary>
        /// ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 文章编号
        /// </summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string ArticleNumber { get; set; }

        /// <summary>
        /// 是否全屏（0:不全屏,1:全屏）
        /// </summary>
        public int Full { get; set; }

        /// <summary>
        /// 是否高清（0:不高清,1:高清）
        /// </summary>
        public int High { get; set; }

        /// <summary>
        /// 透明度（0-100）
        /// </summary>
        public int Transparency { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string Number { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string CreateUserNumber { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [SubSonicStringLength(255), SubSonicNullString]
        public string Url { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsUsed { get; set; }
    }
}