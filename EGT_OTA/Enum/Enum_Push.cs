using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 推送类型枚举
    /// </summary>
    public class Enum_Push : EnumBase
    {
        /// <summary>
        /// 单个用户
        /// </summary>
        [EnumAttribute("单个用户")]
        public const int Single = 0;

        /// <summary>
        /// 多个用户
        /// </summary>
        [EnumAttribute("多个用户")]
        public const int Multiple = 1;

        /// <summary>
        /// 所有用户
        /// </summary>
        [EnumAttribute("所有用户")]
        public const int All = 2;
    }
}
