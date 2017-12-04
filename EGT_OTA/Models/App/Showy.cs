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
    /// 漂浮装扮
    /// </summary>
    [Serializable]
    public class Showy
    {
        public Showy()
        {

        }

        public Showy(string name, List<ShowyCover> cover, int status = 1)
        {
            this.Name = name;
            this.ShowyCover = cover;
            this.Status = status;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 漂浮图标
        /// </summary>
        public List<ShowyCover> ShowyCover { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }

    public class ShowyCover
    {
        public ShowyCover() { }

        public ShowyCover(string cover, int count, int showtype = 0, int status = 1, string name = "")
        {
            this.Cover = cover;
            this.Count = Count;
            this.ShowType = showtype;
            this.Status = status;
            this.Name = name;
        }

        public string Cover { get; set; }

        /// <summary>
        /// 展示图片个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 展示方式
        /// 0：默认
        /// 1：气球
        /// 2：心形
        /// 3：泡泡
        /// 4：黑泡泡
        /// 5：小雪
        /// 6：小雨
        /// 7：暴雨
        /// 8：暴雪
        /// 9：红包
        /// 10：爆竹
        /// 11：元宝
        /// </summary>
        public int ShowType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Name { get; set; }
    }
}