using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonTools;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json.Linq;
using SubSonic.Repository;

namespace AutoMusic
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
                var MusicStartIndex = Tools.SafeInt(System.Web.Configuration.WebConfigurationManager.AppSettings["MusicStartIndex"]);
                var MusicEndIndex = Tools.SafeInt(System.Web.Configuration.WebConfigurationManager.AppSettings["MusicEndIndex"]);
                var LineCount = Tools.SafeInt(System.Web.Configuration.WebConfigurationManager.AppSettings["LineCount"]);

                for (var i = 0; i < LineCount; i++)
                {
                    Thread thread = new Thread(delegate()
                    {
                        while (MusicStartIndex < MusicEndIndex)
                        {
                            lock (locker)
                            {
                                try
                                {
                                    var json = HttpHelper.Get("http://music.163.com/api/song/detail?ids=%5B" + MusicStartIndex + "%5D");

                                    JArray array = JArray.Parse(JObject.Parse(json)["songs"].ToString());
                                    if (array.Count > 0)
                                    {
                                        JObject model = JObject.Parse(array[0].ToString());
                                        var musicId = model["id"].ToString();
                                        var musicName = model["name"].ToString();
                                        if (!string.IsNullOrWhiteSpace(musicName))
                                        {
                                            var musicUrl = model["mp3Url"].ToString();
                                            var artistsArray = JArray.Parse(model["artists"].ToString());
                                            var artists = JObject.Parse(artistsArray[0].ToString());
                                            var artistsName = artists["name"].ToString();
                                            var album = JObject.Parse(model["album"].ToString());
                                            var albumName = album["name"].ToString();
                                            var musicPicUrl = album["picUrl"].ToString();

                                            if (!string.IsNullOrWhiteSpace(musicUrl))
                                            {
                                                var result = true;
                                                try
                                                {
                                                    HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(musicUrl);
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
                                                    Music music = new Music();
                                                    music.Author = artistsName;
                                                    music.Cover = musicPicUrl;
                                                    music.FileUrl = musicUrl;
                                                    music.Name = musicName;
                                                    music.Number = musicId;
                                                    db.Add<Music>(music);
                                                    Console.WriteLine(MusicStartIndex + ":成功");
                                                }
                                                catch (Exception ex)
                                                {
                                                    result = false;
                                                    Console.WriteLine("失败:" + musicName + "," + ex.Message);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine(MusicStartIndex + ":失败");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(MusicStartIndex);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(MusicStartIndex + ":" + ex.Message);
                                }

                                MusicStartIndex += 1;
                            }
                            Thread.Sleep(1000);
                        }
                    });
                    thread.IsBackground = true;
                    thread.Name = "同步音乐接口线程" + i;
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
