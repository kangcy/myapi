using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 红包枚举
    /// </summary>
    public class Enum_RedType : EnumBase
    {
        /// <summary>
        /// 首次登陆
        /// </summary>
        [EnumAttribute("新用户专享红包")]
        public const int Login = 1;

        /// <summary>
        /// 小微篇打赏红包
        /// </summary>
        [EnumAttribute("小微篇打赏红包")]
        public const int Admin = 2;
    }
}
