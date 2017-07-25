using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 推荐状态枚举
    /// </summary>
    public class Enum_Recommend : EnumBase
    {
        /// <summary>
        /// 推荐
        /// </summary>
        [EnumAttribute("推荐")]
        public const int Approved = 1;

        /// <summary>
        /// 不推荐
        /// </summary>
        [EnumAttribute("不推荐")]
        public const int Audit = 0;
    }
}
