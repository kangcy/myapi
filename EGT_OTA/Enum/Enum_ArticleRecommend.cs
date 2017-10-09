using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 文章推荐枚举
    /// </summary>
    public class Enum_ArticleRecommend : EnumBase
    {
        /// <summary>
        /// 无
        /// </summary>
        [EnumAttribute("无")]
        public const int None = 0;

        /// <summary>
        /// 推荐
        /// </summary>
        [EnumAttribute("推荐")]
        public const int Recommend = 98;

        /// <summary>
        /// 置顶
        /// </summary>
        [EnumAttribute("置顶")]
        public const int Top = 99;

        /// <summary>
        /// 系统
        /// </summary>
        [EnumAttribute("系统")]
        public const int System = 100;
    }
}
