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
        /// 未删除
        /// </summary>
        [EnumAttribute("未删除")]
        public const int Delete = 2;
    }
}
