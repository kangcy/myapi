/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  EGT_OTA.Models.App.Article 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/7/29 15:08:56 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/7/29 15:08:56 
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
    /// 文章
    /// </summary>
    [Serializable]
    public class Article : BaseModel
    {
        public Article()
        {
            this.Title = string.Empty;
            this.MusicID = 0;
            this.MusicName = string.Empty;
            this.MusicUrl = string.Empty;
            this.Province = string.Empty;
            this.City = string.Empty;
            this.Cover = string.Empty;
            this.Views = 0;
            this.Goods = 0;
            this.Keeps = 0;
            this.Comments = 0;
            this.Pays = 0;
            this.TypeID = 0;
            this.TypeIDList = "-0-0-";
            this.Template = 0;
            this.ArticlePower = Enum_ArticlePower.Public;
            this.Status = Enum_Status.Approved;
            this.Recommend = Enum_ArticleRecommend.None;
            this.CreateIP = Tools.GetClientIP;
            this.UpdateIP = Tools.GetClientIP;
            this.Tag = string.Empty;
        }

        /// <summary>
        /// 文章ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [SubSonicNullString]
        public string Title { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [SubSonicStringLength(255), SubSonicNullString]
        public string Cover { get; set; }

        /// <summary>
        /// 音乐
        /// </summary>
        public int MusicID { get; set; }

        /// <summary>
        /// 音乐名称
        /// </summary>
        [SubSonicStringLength(255), SubSonicNullString]
        public string MusicName { get; set; }

        /// <summary>
        /// 音乐外链
        /// </summary>
        [SubSonicStringLength(500), SubSonicNullString]
        public string MusicUrl { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// 类型父集合
        /// </summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string TypeIDList { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        public int Views { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int Goods { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        public int Shares { get; set; }

        /// <summary>
        /// 加精
        /// </summary>
        public int Recommend { get; set; }

        /// <summary>
        /// 文章权限
        /// </summary>
        public int ArticlePower { get; set; }

        /// <summary>
        /// 文章权限密码
        /// </summary>
        [SubSonicStringLength(10), SubSonicNullString]
        public string ArticlePowerPwd { get; set; }

        /// <summary>
        /// 文章编号
        /// </summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string Number { get; set; }

        /// <summary>
        /// 模板（0:纯白、1:自定义、2:模板）
        /// </summary>
        public int Template { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [SubSonicNullString]
        public string Tag { get; set; }

        /// <summary>
        /// 是否投稿启用
        /// </summary>
        public int Submission { get; set; }

        #region  定位

        /// <summary>
        /// 省份名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string Province { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string City { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string District { get; set; }

        /// <summary>
        /// 街道名称
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string Street { get; set; }

        /// <summary>
        /// 具体定位
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string DetailName { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [SubSonicStringLength(20), SubSonicNullString]
        public string CityCode { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        #endregion

        #region 扩展

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
        /// 打赏数
        /// </summary>
        [SubSonicIgnore]
        public int Pays { get; set; }

        /// <summary>
        /// 模板配置
        /// </summary>
        [SubSonicIgnore]
        public Template TemplateJson { get; set; }

        /// <summary>
        /// 创建人头像
        /// </summary>
        [SubSonicIgnore]
        public string Avatar { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SubSonicIgnore]
        public string NickName { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [SubSonicIgnore]
        public string UserCover { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SubSonicIgnore]
        public int UserID { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        [SubSonicIgnore]
        public string TypeName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SubSonicIgnore]
        public string CreateDateText { get; set; }

        /// <summary>
        /// 音乐是否自动播放
        /// </summary>
        [SubSonicIgnore]
        public int AutoMusic { get; set; }

        /// <summary>
        /// 分享带昵称
        /// </summary>
        [SubSonicIgnore]
        public int ShareNick { get; set; }

        /// <summary>
        /// 分享链接
        /// </summary>
        [SubSonicIgnore]
        public string ShareUrl { get; set; }

        /// <summary>
        /// 文章部分
        /// </summary>
        [SubSonicIgnore]
        public List<ArticlePart> ArticlePart { get; set; }

        /// <summary>
        /// 文章评论
        /// </summary>
        [SubSonicIgnore]
        public List<Comment> CommentList { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [SubSonicIgnore]
        public List<Tag> TagList { get; set; }

        /// <summary>
        /// 启用打赏
        /// </summary>
        [SubSonicIgnore]
        public int IsPay { get; set; }

        /// <summary>
        /// 是否收藏
        /// </summary>
        [SubSonicIgnore]
        public int IsKeep { get; set; }

        /// <summary>
        /// 是否关注
        /// </summary>
        [SubSonicIgnore]
        public int IsFollow { get; set; }

        /// <summary>
        /// 是否点赞
        /// </summary>
        [SubSonicIgnore]
        public int IsZan { get; set; }

        /// <summary>
        /// 背景设置
        /// </summary>
        [SubSonicIgnore]
        public Background BackgroundJson { get; set; }

        /// <summary>
        /// 分享
        /// </summary>
        [SubSonicIgnore]
        public Share Share { get; set; }

        #endregion
    }

    public class ArticleJson
    {
        public int UserID { get; set; }
        public string UserNumber { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string UserCover { get; set; }
        public string Signature { get; set; }
        public int ArticleID { get; set; }
        public string ArticleNumber { get; set; }
        public string Title { get; set; }
        public int Views { get; set; }
        public int Goods { get; set; }
        public int Comments { get; set; }
        public string Cover { get; set; }
        public string CreateDate { get; set; }
        public string TypeName { get; set; }
        public List<ArticlePart> ArticlePart { get; set; }
        public int ArticlePower { get; set; }
        public int Recommend { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public int IsFollow { get; set; }
        public int IsZan { get; set; }
        public int IsKeep { get; set; }
        public List<Tag> TagList { get; set; }
        public int IsPay { get; set; }
        public int Submission { get; set; }
    }

    public class UserArticleJson
    {
        public string CreateDate { get; set; }
        public int Count { get; set; }
        public List<UserArticleSubJson> List { get; set; }
    }

    public class UserArticleSubJson
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string CreateUserNumber { get; set; }
        public string Cover { get; set; }
        public int ArticlePower { get; set; }
    }
}