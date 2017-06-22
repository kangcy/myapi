using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 操作类型枚举
    /// </summary>
    public class Enum_ActionType : EnumBase
    {
        /// <summary>
        /// 喜欢
        /// </summary>
        [EnumAttribute("喜欢")]
        public const int Like = 0;

        /// <summary>
        /// 关注
        /// </summary>
        [EnumAttribute("关注")]
        public const int Follow = 1;

        /// <summary>
        /// 收藏
        /// </summary>
        [EnumAttribute("收藏")]
        public const int Keep = 2;
    }
}
