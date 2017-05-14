using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 接口返回错误码枚举
    /// </summary>
    public class Enum_ErrorCode : EnumBase
    {
        /// <summary>
        /// 未登录
        /// </summary>
        [EnumAttribute("未登录")]
        public const int UnLogin = 1;
    }
}
