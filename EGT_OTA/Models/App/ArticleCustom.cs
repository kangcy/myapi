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
    /// 文章自定义
    /// </summary>
    [Serializable]
    public class ArticleCustom
    {
        public ArticleCustom() { }

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
        /// 漂浮装扮
        /// </summary>
        [SubSonicNullString]
        public string ShowyUrl { get; set; }

        /// <summary>
        /// 音乐
        /// </summary>
        public int MusicID { get; set; }

        /// <summary>
        /// 音乐名称
        /// </summary>
        [SubSonicStringLength(255), SubSonicNullString]
        public string MusicName { get; set; }

        /// <summary>
        /// 音乐外链
        /// </summary>
        [SubSonicStringLength(500), SubSonicNullString]
        public string MusicUrl { get; set; }

        /// <summary>
        /// 头部排版类型
        /// </summary>
        public int ArticleHead { get; set; }

        /// <summary>
        /// 音乐类型
        /// </summary>
        public int Music { get; set; }
    }
}