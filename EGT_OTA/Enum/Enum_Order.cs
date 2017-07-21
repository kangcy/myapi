using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 订单状态枚举
    /// </summary>
    public class Enum_Order : EnumBase
    {
        /// <summary>
        /// 待支付
        /// </summary>
        [EnumAttribute("待支付")]
        public const int Audit = 0;

        /// <summary>
        /// 已支付
        /// </summary>
        [EnumAttribute("已支付")]
        public const int Approved = 1;
    }
}
