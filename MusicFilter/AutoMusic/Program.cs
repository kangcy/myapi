using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonTools;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json.Linq;
using SubSonic.Repository;

namespace MusicFilter
{


    class Program
    {
        static Object locker = new Object();

        static void Main(string[] args)
        {
            EGT_OTA.Models.Repository.UpdateDB();
            SimpleRepository db = Repository.GetRepo();
            try
            {
                Console.WriteLine("正在运行");

                Console.WriteLine("读取Music01");

                var music01 = db.All<Music01>().ToList();

                Console.WriteLine("读取Music02");

                var music02 = db.All<Music02>().ToList();

                Console.WriteLine("读取Music03");

                var music03 = db.All<Music03>().ToList();

                Console.WriteLine("读取Music04");

                var music04 = db.All<Music04>().ToList();

                Console.WriteLine("读取Music05");

                var music05 = db.All<Music05>().ToList();

                Console.WriteLine("读取Music06");

                var music06 = db.All<Music06>().ToList();

                Console.WriteLine("读取Music07");

                var music07 = db.All<Music07>().ToList();

                Console.WriteLine("读取Music08");

                var music08 = db.All<Music08>().ToList();

                Console.WriteLine("读取Music09");

                var music09 = db.All<Music09>().ToList();

                Console.WriteLine("读取Music10");

                var music10 = db.All<Music10>().ToList();

                Console.WriteLine("读取Music11");

                var music11 = db.All<Music11>().ToList();

                Console.WriteLine("读取Music12");

                var music12 = db.All<Music12>().ToList();

                Console.WriteLine("读取Music13");

                var music13 = db.All<Music13>().ToList();

                var list = new List<Music>();
                list.AddRange(music01);
                list.AddRange(music02);
                list.AddRange(music03);
                list.AddRange(music04);
                list.AddRange(music05);
                list.AddRange(music06);
                list.AddRange(music07);
                list.AddRange(music08);
                list.AddRange(music09);
                list.AddRange(music10);
                list.AddRange(music11);
                list.AddRange(music12);
                list.AddRange(music13);

                var LineCount = Tools.SafeInt(System.Web.Configuration.WebConfigurationManager.AppSettings["LineCount"]);
                var loss = 0;

                for (var i = 0; i < LineCount; i++)
                {
                    Thread thread = new Thread(delegate()
                    {
                        while (list.Count > 0)
                        {
                            lock (locker)
                            {
                                var result = true;
                                var model = list[0];
                                try
                                {
                                    System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.CreateDefault(new Uri(model.FileUrl));
                                    httpWebRequest.Method = "HEAD";
                                    httpWebRequest.Timeout = 2000;
                                    result = (((System.Net.HttpWebResponse)httpWebRequest.GetResponse()).StatusCode == System.Net.HttpStatusCode.OK);
                                }
                                catch (Exception ex)
                                {
                                    result = false;

                                    Console.WriteLine("Request:" + ex.Message + "," + model.FileUrl);
                                }
                                if (!result)
                                {
                                    //switch (model.DataBaseNumber)
                                    //{
                                    //    case 1:
                                    //        db.Delete<Music01>(model.ID);
                                    //        break;
                                    //    case 2:
                                    //        db.Delete<Music02>(model.ID);
                                    //        break;
                                    //    case 3:
                                    //        db.Delete<Music03>(model.ID);
                                    //        break;
                                    //    case 4:
                                    //        db.Delete<Music04>(model.ID);
                                    //        break;
                                    //    case 5:
                                    //        db.Delete<Music05>(model.ID);
                                    //        break;
                                    //    case 6:
                                    //        db.Delete<Music06>(model.ID);
                                    //        break;
                                    //    case 7:
                                    //        db.Delete<Music07>(model.ID);
                                    //        break;
                                    //    case 8:
                                    //        db.Delete<Music08>(model.ID);
                                    //        break;
                                    //    case 9:
                                    //        db.Delete<Music09>(model.ID);
                                    //        break;
                                    //    case 10:
                                    //        db.Delete<Music10>(model.ID);
                                    //        break;
                                    //    case 11:
                                    //        db.Delete<Music11>(model.ID);
                                    //        break;
                                    //    case 12:
                                    //        db.Delete<Music12>(model.ID);
                                    //        break;
                                    //    case 13:
                                    //        db.Delete<Music13>(model.ID);
                                    //        break;
                                    //    default:
                                    //        break;
                                    //}
                                    list.RemoveAt(0);
                                    loss++;
                                    Console.WriteLine("总数:" + loss + ",ID:" + model.ID + ",Name:" + model.Name);
                                }
                            }
                            Thread.Sleep(1000);
                        }
                    });
                    thread.IsBackground = true;
                    thread.Name = "过滤音乐接口线程" + i;
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
