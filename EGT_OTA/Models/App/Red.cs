using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 红包
    /// </summary>
    public class Red
    {
        /// <summary>
        /// ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 红包编号
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string Number { get; set; }

        /// <summary>
        /// 红包对象
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string ToUserNumber { get; set; }

        /// <summary>
        /// 红包金额（单位：分）
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 红包状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 红包来源（1：首次登陆红包）
        /// </summary>
        public int RedType { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
    }

    public class RedJson : Red
    {
        public int UserID { get; set; }
        public string UserAvatar { get; set; }
        public string UserName { get; set; }
        public string UserCover { get; set; }
        public string CreateDateText { get; set; }
    }

    public class RedJson2
    {
        public int ID { get; set; }
        public int FromUserID { get; set; }
        public string FromUserNumber { get; set; }
        public string FromUserAvatar { get; set; }
        public string FromUserName { get; set; }
        public string FromUserCover { get; set; }
        public int ToUserID { get; set; }
        public string ToUserNumber { get; set; }
        public string ToUserAvatar { get; set; }
        public string ToUserName { get; set; }
        public string ToUserCover { get; set; }
        public string CreateDate { get; set; }
        public int Price { get; set; }
        public int PayType { get; set; }
        public string Title { get; set; }
        public string ArticleNumber { get; set; }
        public int Anony { get; set; }
        public int Status { get; set; }
    }
}