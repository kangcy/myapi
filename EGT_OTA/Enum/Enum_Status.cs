using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 通用状态枚举
    /// </summary>
    public class Enum_Status : EnumBase
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [EnumAttribute("未审核")]
        public const int Audit = 0;

        /// <summary>
        /// 已审核
        /// </summary>
        [EnumAttribute("已审核")]
        public const int Approved = 1;

        /// <summary>
        /// 临时删除
        /// </summary>
        [EnumAttribute("临时删除")]
        public const int Delete = 2;


        /// <summary>
        /// 彻底审核
        /// </summary>
        [EnumAttribute("彻底审核")]
        public const int DeleteCompletely = 3;
    }
}
