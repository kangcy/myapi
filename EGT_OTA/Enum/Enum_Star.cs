using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 星座枚举
    /// </summary>
    public class Enum_Star : EnumBase
    {
        /// <summary>
        /// 保密
        /// </summary>
        [EnumAttribute("保密")]
        public const int None = 0;

        /// <summary>
        /// 白羊座
        /// </summary>
        [EnumAttribute("白羊座")]
        public const int BaiYang = 1;

        /// <summary>
        /// 金牛座
        /// </summary>
        [EnumAttribute("金牛座")]
        public const int JinNiu = 2;

        /// <summary>
        /// 双子座
        /// </summary>
        [EnumAttribute("双子座")]
        public const int ShuangZi = 3;

        /// <summary>
        /// 巨蟹座
        /// </summary>
        [EnumAttribute("巨蟹座")]
        public const int JuXie = 4;

        /// <summary>
        /// 狮子座
        /// </summary>
        [EnumAttribute("狮子座")]
        public const int ShiZi = 5;

        /// <summary>
        /// 处女座
        /// </summary>
        [EnumAttribute("处女座")]
        public const int ChuNv = 6;

        /// <summary>
        /// 天秤座
        /// </summary>
        [EnumAttribute("天秤座")]
        public const int TianChen = 7;

        /// <summary>
        /// 天蝎座
        /// </summary>
        [EnumAttribute("天蝎座")]
        public const int TianXie = 8;

        /// <summary>
        /// 射手座
        /// </summary>
        [EnumAttribute("射手座")]
        public const int SheShou = 9;

        /// <summary>
        /// 摩羯座
        /// </summary>
        [EnumAttribute("摩羯座")]
        public const int MoJie = 10;

        /// <summary>
        /// 水瓶座
        /// </summary>
        [EnumAttribute("水瓶座")]
        public const int ShuiPing = 11;

        /// <summary>
        /// 双鱼座
        /// </summary>
        [EnumAttribute("双鱼座")]
        public const int ShungYu = 12;
    }
}
