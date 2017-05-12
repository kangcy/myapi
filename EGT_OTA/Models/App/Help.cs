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
    /// 帮助中心
    /// </summary>
    [Serializable]
    public class Help
    {
        public Help() { }
        public Help(int helpType, int id, string name, string summary)
        {
            this.ID = id;
            this.HelpType = helpType;
            this.Name = name;
            this.Summary = summary;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 帮助类型
        /// </summary>
        public int HelpType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }
    }

    /// <summary>
    /// 帮助分类
    /// </summary>
    [Serializable]
    public class HelpType
    {
        public HelpType() { }
        public HelpType(int id, string name, string cover)
        {
            this.ID = id;
            this.Name = name;
            this.Cover = cover;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
    }
}