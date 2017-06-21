using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGT_OTA.Models
{
    public class Share
    {
        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 生成签名的时间戳
        /// </summary>
        public string TimeStr { get; set; }

        /// <summary>
        /// 生成签名的随机串
        /// </summary>
        public string NonceStr { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
    }

    public class AccessToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class Ticket
    {
        public string errcode { get; set; }
        public int expires_in { get; set; }
    }
}