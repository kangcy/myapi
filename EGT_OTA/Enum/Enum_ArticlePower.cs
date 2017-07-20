using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 文章权限枚举
    /// </summary>
    public class Enum_ArticlePower : EnumBase
    {
        /// <summary>
        /// 所有人都可以看到
        /// </summary>
        [EnumAttribute("公开")]
        public const int Public = 3;

        /// <summary>
        /// 仅通过自己分享后才可见
        /// </summary>
        [EnumAttribute("限制可见")]
        public const int Share = 2;

        /// <summary>
        /// 设置密码,输入密码才可见
        /// </summary>
        [EnumAttribute("密码可见")]
        public const int Password = 1;

        /// <summary>
        /// 仅自己可见
        /// </summary>
        [EnumAttribute("私密")]
        public const int Myself = 0;
    }
}
