using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 处理状态枚举
    /// </summary>
    public class Enum_Deal : EnumBase
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [EnumAttribute("待处理")]
        public const int Audit = 0;

        /// <summary>
        /// 已处理
        /// </summary>
        [EnumAttribute("已处理")]
        public const int Approved = 1;
    }
}
