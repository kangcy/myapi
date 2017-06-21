/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  EGT_OTA.Models.App.User 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/7/29 11:03:10 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/7/29 11:03:10 
 * 修改人  ：  
 * 版本号  ：V1.0.0.0 
 * 描述    ：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonTools;
using EGT_OTA.Helper;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Serializable]
    public class User : AddressBaseModel
    {
        public User()
        {
            this.Province = string.Empty;
            this.City = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.WeiXin = string.Empty;
            this.CreateDate = DateTime.Now;
            this.LastLoginDate = DateTime.Now;
            this.LastLoginIP = Tools.GetClientIP;
            this.LoginTimes = 1;
            this.IsEmail = 0;
            this.Keeps = 0;
            this.Follows = 0;
            this.Fans = 0;
            this.Status = Enum_Status.Approved;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        ///<summary>
        ///登陆密码
        ///</summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string Password { get; set; }

        ///<summary>
        ///用户头像
        ///</summary>
        [SubSonicStringLength(500), SubSonicNullString]
        public string Avatar { get; set; }

        ///<summary>
        ///用户昵称
        ///</summary>
        [SubSonicStringLength(1000), SubSonicNullString]
        public string NickName { get; set; }

        ///<summary>
        ///个性签名
        ///</summary>
        [SubSonicStringLength(3000), SubSonicNullString]
        public string Signature { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        ///<summary>
        ///电子邮件
        ///</summary>
        [SubSonicNullString]
        public string Email { get; set; }

        ///<summary>
        ///绑定手机
        ///</summary>
        [SubSonicStringLength(11), SubSonicNullString]
        public string Phone { get; set; }

        ///<summary>
        ///绑定微信
        ///</summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string WeiXin { get; set; }

        ///<summary>
        ///绑定QQ
        ///</summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string QQ { get; set; }

        ///<summary>
        ///绑定微博
        ///</summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string Weibo { get; set; }

        /// <summary>
        /// 音乐自动播放
        /// </summary>
        public int AutoMusic { get; set; }

        /// <summary>
        /// 分享带昵称
        /// </summary>
        public int ShareNick { get; set; }

        ///<summary>
        ///上次登录IP
        ///</summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        public int LoginTimes { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [SubSonicNullString]
        public string Cover { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否邮箱认证
        /// </summary>
        public int IsEmail { get; set; }

        /// <summary>
        /// 是否启用打赏
        /// </summary>
        public int IsPay { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 启用水印
        /// </summary>
        public int UseDraw { get; set; }

        /// <summary>
        /// 是否推荐
        /// </summary>
        public int IsRecommend { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public int UserRole { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        [SubSonicNullString]
        public string ClientID { get; set; }

        #region  隐私管理

        /// <summary>
        /// 显示我喜欢的文章
        /// </summary>
        public int ShowArticle { get; set; }

        /// <summary>
        /// 显示我的关注
        /// </summary>
        public int ShowFollow { get; set; }

        /// <summary>
        /// 显示我的粉丝
        /// </summary>
        public int ShowFan { get; set; }

        /// <summary>
        /// 显示消息推送
        /// </summary>
        public int ShowPush { get; set; }

        /// <summary>
        /// 启用定位
        /// </summary>
        public int ShowPosition { get; set; }

        #endregion

        #region  扩展字段

        /// <summary>
        /// 地址
        /// </summary>
        [SubSonicIgnore]
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [SubSonicIgnore]
        public string BirthdayText { get; set; }

        /// <summary>
        /// 关注
        /// </summary>
        [SubSonicIgnore]
        public int Follows { get; set; }

        /// <summary>
        /// 粉丝
        /// </summary>
        [SubSonicIgnore]
        public int Fans { get; set; }

        /// <summary>
        /// 文章数
        /// </summary>
        [SubSonicIgnore]
        public int Articles { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        [SubSonicIgnore]
        public int Keeps { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        [SubSonicIgnore]
        public int Comments { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        [SubSonicIgnore]
        public int Zans { get; set; }

        /// <summary>
        /// 打赏数
        /// </summary>
        [SubSonicIgnore]
        public int Pays { get; set; }

        /// <summary>
        /// 分享链接
        /// </summary>
        [SubSonicIgnore]
        public string ShareUrl { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        [SubSonicIgnore]
        public double Distance { get; set; }

        /// <summary>
        /// 是否关注
        /// </summary>
        [SubSonicIgnore]
        public int IsFan { get; set; }

        /// <summary>
        /// 是否拉黑
        /// </summary>
        [SubSonicIgnore]
        public int IsBlack { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        [SubSonicIgnore]
        public int Money { get; set; }

        /// <summary>
        /// 微信分享
        /// </summary>
        [SubSonicIgnore]
        public Share Share { get; set; }

        #endregion
    }
}