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
    /// 音乐
    /// </summary>
    [Serializable]
    public class Music
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 音乐编号
        /// </summary>
        [SubSonicNullString]
        public string Number { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SubSonicNullString]
        public string Name { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [SubSonicNullString]
        public string Author { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [SubSonicNullString]
        public string Cover { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        [SubSonicNullString]
        public string FileUrl { get; set; }

        /// <summary>
        /// 数据库编号
        /// </summary>
        public int DataBaseNumber { get; set; }
    }

    /// <summary>
    /// 音乐分库
    /// </summary>
    public class Music01 : Music { }
    public class Music02 : Music { }
    public class Music03 : Music { }
    public class Music04 : Music { }
    public class Music05 : Music { }
    public class Music06 : Music { }
    public class Music07 : Music { }
    public class Music08 : Music { }
    public class Music09 : Music { }
    public class Music10 : Music { }
    public class Music11 : Music { }
    public class Music12 : Music { }
    public class Music13 : Music { }
}