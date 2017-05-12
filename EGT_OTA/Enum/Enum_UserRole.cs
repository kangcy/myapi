using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 用户角色枚举
    /// </summary>
    public class Enum_UserRole : EnumBase
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [EnumAttribute("普通用户")]
        public const int Common = 0;

        /// <summary>
        /// 管理员
        /// </summary>
        [EnumAttribute("管理员")]
        public const int Administrator = 1;

        /// <summary>
        /// 超级管理员
        /// </summary>
        [EnumAttribute("超级管理员")]
        public const int SuperAdministrator = 2;
    }
}
