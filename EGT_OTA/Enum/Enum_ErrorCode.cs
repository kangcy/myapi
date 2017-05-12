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
