using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 申请提现记录
    /// </summary>
    [Serializable]
    public class ApplyMoney : BaseModelShort
    {
        ///<summary>
        ///提款账号
        ///</summary>
        [SubSonicNullString]
        public string Account { get; set; }

        ///<summary>
        ///提款账户昵称
        ///</summary>
        [SubSonicNullString]
        public string AccountName { get; set; }

        ///<summary>
        ///提现状态
        ///</summary>
        public int Status { get; set; }
    }

    /// <summary>
    /// 申请文章恢复记录
    /// </summary>
    [Serializable]
    public class ApplyRevoke : BaseModelShort
    {
        ///<summary>
        ///申请状态
        ///</summary>
        public int Status { get; set; }

        /// <summary>
        /// 文章编号
        /// </summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string ArticleNumber { get; set; }
    }
}
