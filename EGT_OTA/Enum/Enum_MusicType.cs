using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 支付方式枚举
    /// </summary>
    public class Enum_MusicType : EnumBase
    {
        /// <summary>
        /// 热门歌曲榜
        /// </summary>
        [EnumAttribute("热门歌曲榜")]
        public const int hot0 = 0;

        /// <summary>
        /// 原创歌曲榜
        /// </summary>
        [EnumAttribute("原创歌曲榜")]
        public const int hot1 = 1;

        /// <summary>
        /// ACG音乐榜
        /// </summary>
        [EnumAttribute("ACG音乐榜")]
        public const int hot2 = 2;

        /// <summary>
        /// 古典音乐榜
        /// </summary>
        [EnumAttribute("古典音乐榜")]
        public const int hot3 = 3;

        /// <summary>
        /// KTV唛榜
        /// </summary>
        [EnumAttribute("KTV唛榜")]
        public const int hot4 = 4;
    }
}
