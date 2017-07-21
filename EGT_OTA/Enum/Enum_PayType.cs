using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 支付方式枚举
    /// </summary>
    public class Enum_PayType : EnumBase
    {
        /// <summary>
        /// 支付宝
        /// </summary>
        [EnumAttribute("支付宝")]
        public const int ZHIFUBAO = 1;

        /// <summary>
        /// 微信
        /// </summary>
        [EnumAttribute("微信")]
        public const int WEIXIN = 2;
    }
}
