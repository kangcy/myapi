using System;
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
    public class MusicRepository
    {
        public static SimpleRepository GetRepo()
        {
            var item = HttpContext.Current.Items["MusicConnection"] as SimpleRepository;
            if (item == null)
            {
                var newItem = MusicRepository.GetRepo("MusicConnection");
                HttpContext.Current.Items["MusicConnection"] = newItem;
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

        public static IDataProvider GetProvider(string connection = "MusicConnection")
        {
            string database = System.Configuration.ConfigurationManager.ConnectionStrings[connection].ToString();
            return ProviderFactory.GetProvider(database, "MySql.Data.MySqlClient");
        }

        public static void UpdateDB(string connection = "MusicConnection")
        {
            var repo = new SimpleRepository(GetProvider(connection), SimpleRepositoryOptions.RunMigrations);

            repo.Single<Music>(x => x.ID == 0);//音乐
            repo.Single<Music01>(x => x.ID == 0);//音乐库01
            repo.Single<Music02>(x => x.ID == 0);//音乐库02
            repo.Single<Music03>(x => x.ID == 0);//音乐库03
            repo.Single<Music04>(x => x.ID == 0);//音乐库04
            repo.Single<Music05>(x => x.ID == 0);//音乐库05
            repo.Single<Music06>(x => x.ID == 0);//音乐库06
            repo.Single<Music07>(x => x.ID == 0);//音乐库07
            repo.Single<Music08>(x => x.ID == 0);//音乐库08
            repo.Single<Music09>(x => x.ID == 0);//音乐库09
            repo.Single<Music10>(x => x.ID == 0);//音乐库10
            repo.Single<Music11>(x => x.ID == 0);//音乐库11
            repo.Single<Music12>(x => x.ID == 0);//音乐库12
            repo.Single<Music13>(x => x.ID == 0);//音乐库13
        }
    }
}
