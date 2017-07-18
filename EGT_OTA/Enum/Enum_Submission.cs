using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 投稿枚举
    /// </summary>
    public class Enum_Submission : EnumBase
    {
        /// <summary>
        /// 未提交
        /// </summary>
        [EnumAttribute("未提交")]
        public const int None = 0;

        /// <summary>
        /// 待审核
        /// </summary>
        [EnumAttribute("待审核")]
        public const int Audit = 1;

        /// <summary>
        /// 已审核
        /// </summary>
        [EnumAttribute("已审核")]
        public const int Approved = 2;

        /// <summary>
        /// 临时审核
        /// </summary>
        [EnumAttribute("临时审核")]
        public const int TemporaryApproved = 100;
    }
}
