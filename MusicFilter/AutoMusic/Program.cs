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
using System.Net;

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
                music01.ForEach(x =>
                {
                    x.DataBaseNumber = 1;
                });

                Console.WriteLine("读取Music02");

                var music02 = db.All<Music02>().ToList();
                music02.ForEach(x =>
                {
                    x.DataBaseNumber = 2;
                });

                Console.WriteLine("读取Music03");

                var music03 = db.All<Music03>().ToList();
                music03.ForEach(x =>
                {
                    x.DataBaseNumber = 3;
                });

                Console.WriteLine("读取Music04");

                var music04 = db.All<Music04>().ToList();
                music04.ForEach(x =>
                {
                    x.DataBaseNumber = 4;
                });

                Console.WriteLine("读取Music05");

                var music05 = db.All<Music05>().ToList();
                music05.ForEach(x =>
                {
                    x.DataBaseNumber = 5;
                });

                Console.WriteLine("读取Music06");

                var music06 = db.All<Music06>().ToList();
                music06.ForEach(x =>
                {
                    x.DataBaseNumber = 6;
                });

                Console.WriteLine("读取Music07");

                var music07 = db.All<Music07>().ToList();
                music07.ForEach(x =>
                {
                    x.DataBaseNumber = 7;
                });

                Console.WriteLine("读取Music08");

                var music08 = db.All<Music08>().ToList();
                music08.ForEach(x =>
                {
                    x.DataBaseNumber = 8;
                });

                Console.WriteLine("读取Music09");

                var music09 = db.All<Music09>().ToList();
                music09.ForEach(x =>
                {
                    x.DataBaseNumber = 9;
                });

                Console.WriteLine("读取Music10");

                var music10 = db.All<Music10>().ToList();
                music10.ForEach(x =>
                {
                    x.DataBaseNumber = 10;
                });

                Console.WriteLine("读取Music11");

                var music11 = db.All<Music11>().ToList();
                music11.ForEach(x =>
                {
                    x.DataBaseNumber = 11;
                });

                Console.WriteLine("读取Music12");

                var music12 = db.All<Music12>().ToList();
                music12.ForEach(x =>
                {
                    x.DataBaseNumber = 12;
                });

                Console.WriteLine("读取Music13");

                var music13 = db.All<Music13>().ToList();
                music13.ForEach(x =>
                {
                    x.DataBaseNumber = 13;
                });

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
                                var model = list[0];
                                var result = CheckFile(model);
                                if (!result)
                                {
                                    switch (model.DataBaseNumber)
                                    {
                                        case 1:
                                            db.Delete<Music01>(model.ID);
                                            break;
                                        case 2:
                                            db.Delete<Music02>(model.ID);
                                            break;
                                        case 3:
                                            db.Delete<Music03>(model.ID);
                                            break;
                                        case 4:
                                            db.Delete<Music04>(model.ID);
                                            break;
                                        case 5:
                                            db.Delete<Music05>(model.ID);
                                            break;
                                        case 6:
                                            db.Delete<Music06>(model.ID);
                                            break;
                                        case 7:
                                            db.Delete<Music07>(model.ID);
                                            break;
                                        case 8:
                                            db.Delete<Music08>(model.ID);
                                            break;
                                        case 9:
                                            db.Delete<Music09>(model.ID);
                                            break;
                                        case 10:
                                            db.Delete<Music10>(model.ID);
                                            break;
                                        case 11:
                                            db.Delete<Music11>(model.ID);
                                            break;
                                        case 12:
                                            db.Delete<Music12>(model.ID);
                                            break;
                                        case 13:
                                            db.Delete<Music13>(model.ID);
                                            break;
                                        default:
                                            break;
                                    }

                                    loss++;
                                }
                                list.RemoveAt(0);
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

        private static bool CheckFile(Music model)
        {
            var result = true;
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(model.FileUrl);
                wbRequest.Method = "GET";
                wbRequest.Timeout = 2000;
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                result = (wbResponse.StatusCode == System.Net.HttpStatusCode.OK);

                if (wbResponse != null)
                {
                    wbResponse.Close();
                }
                if (wbRequest != null)
                {
                    wbRequest.Abort();
                }
                Console.WriteLine(model.ID + "," + model.Name);
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine("失败:" + model.ID + "," + model.Name + "," + ex.Message);
            }
            return result;
        }
    }
}
