using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 红包领取状态枚举
    /// </summary>
    public class Enum_Red : EnumBase
    {
        /// <summary>
        /// 待领取
        /// </summary>
        [EnumAttribute("待领取")]
        public const int Audit = 0;

        /// <summary>
        /// 已领取
        /// </summary>
        [EnumAttribute("已领取")]
        public const int Approved = 1;
    }
}
