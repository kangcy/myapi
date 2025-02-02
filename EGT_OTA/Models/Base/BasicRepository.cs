﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;
using SubSonic.Repository;
using SubSonic.DataProviders;
using System.Web;
using EGT_OTA.Models;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 在Global文件中注册时使用
    /// </summary>
    public class BasicRepository
    {
        public static SimpleRepository GetRepo()
        {
            var item = HttpContext.Current.Items["DefaultConnection"] as SimpleRepository;
            if (item == null)
            {
                var newItem = BasicRepository.GetRepo("DefaultConnection");
                HttpContext.Current.Items["DefaultConnection"] = newItem;
                return newItem;
            }
            return item;
        }

        public static SimpleRepository GetRepo(string db)
        {
            return new SimpleRepository(db, SimpleRepositoryOptions.Default);
        }

        public static SimpleRepository GetRepoByConn(string conn)
        {
            var idp = ProviderFactory.GetProvider(conn, "MySql.Data.MySqlClient");
            //var idp = ProviderFactory.GetProvider(conn, "System.Data.SqlClient");

            return new SimpleRepository(idp);
        }

        public static IDataProvider GetProvider(string connection = "DefaultConnection")
        {
            string database = System.Configuration.ConfigurationManager.ConnectionStrings[connection].ToString();
            return ProviderFactory.GetProvider(database, "MySql.Data.MySqlClient");
        }

        public static void UpdateDB(string connection = "DefaultConnection")
        {
            var repo = new SimpleRepository(GetProvider(connection), SimpleRepositoryOptions.RunMigrations);

            repo.Single<User>(x => x.ID == 0);//用户
            repo.Single<Article>(x => x.ID == 0);//文章
            repo.Single<ArticlePart>(x => x.ID == 0);//文章内容
            repo.Single<Comment>(x => x.ID == 0);//评论
            repo.Single<Fan>(x => x.ID == 0);//关注
            repo.Single<Keep>(x => x.ID == 0);//收藏
            repo.Single<ArticleZan>(x => x.ID == 0);//文章点赞
            repo.Single<CommentZan>(x => x.ID == 0);//评论点赞
            repo.Single<FeedBack>(x => x.ID == 0);//意见反馈
            repo.Single<ShareLog>(x => x.ID == 0);//分享记录
            repo.Single<SendSMS>(x => x.ID == 0);//短信发送记录
            repo.Single<ApplyMoney>(x => x.ID == 0);//提现申请记录
            repo.Single<Report>(x => x.ID == 0);//举报记录
            repo.Single<ArticleRecommend>(x => x.ID == 0);//投稿记录
            repo.Single<Black>(x => x.ID == 0);//黑名单
            repo.Single<Tag>(x => x.ID == 0);//标签
            repo.Single<Order>(x => x.ID == 0);//订单
            repo.Single<UserLog>(x => x.ID == 0);//登录日志
            repo.Single<Background>(x => x.ID == 0);//背景
            repo.Single<UserAction>(x => x.ID == 0);//用户操作
            repo.Single<Red>(x => x.ID == 0);//红包
            repo.Single<ArticleCustom>(x => x.ID == 0);//文章自定义
        }
    }
}
