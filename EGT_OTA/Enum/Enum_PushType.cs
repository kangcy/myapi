using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 推送类型枚举
    /// </summary>
    public class Enum_PushType : EnumBase
    {
        /// <summary>
        /// 文章
        /// </summary>
        [EnumAttribute("文章")]
        public const int Article = 0;

        /// <summary>
        /// 评论
        /// </summary>
        [EnumAttribute("评论")]
        public const int Comment = 1;

        /// <summary>
        /// 打赏
        /// </summary>
        [EnumAttribute("打赏")]
        public const int Money = 2;
    }
}
