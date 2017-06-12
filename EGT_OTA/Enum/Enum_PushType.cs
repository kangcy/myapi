using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 推送类型枚举
    /// </summary>
    public class Enum_PushType : EnumBase
    {
        /// <summary>
        /// 系统文章推荐
        /// </summary>
        [EnumAttribute("系统文章推荐")]
        public const int Article = 0;

        /// <summary>
        /// 用户评论
        /// </summary>
        [EnumAttribute("用户评论")]
        public const int Comment = 1;

        /// <summary>
        /// 用户打赏
        /// </summary>
        [EnumAttribute("用户打赏")]
        public const int Money = 2;

        /// <summary>
        /// 关注用户
        /// </summary>
        [EnumAttribute("关注用户")]
        public const int Fan = 3;

        /// <summary>
        /// 关注用户发布文章
        /// </summary>
        [EnumAttribute("关注用户发布文章")]
        public const int FanArticle = 4;

        /// <summary>
        /// APP升级
        /// </summary>
        [EnumAttribute("APP升级")]
        public const int Update = 10;
    }
}
