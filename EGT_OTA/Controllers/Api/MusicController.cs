using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using CommonTools;
using EGT_OTA.Controllers.Filter;
using EGT_OTA.Helper;
using EGT_OTA.Helper.Search;
using EGT_OTA.Models;
using Newtonsoft.Json;
using SubSonic.Repository;
using SubSonic.DataProviders;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace EGT_OTA.Controllers.Api
{
    public class MusicController : BaseApiController
    {
        /// <summary>
        /// 音乐列表
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/All")]
        public string All()
        {
            ApiResult result = new ApiResult();
            try
            {
                var musicType = GetMusic().OrderBy(x => x.SortID).ToList();
                musicType.ForEach(x =>
                {
                    x.Music.ForEach(l =>
                    {
                        l.Cover = GetFullUrl(l.Cover);
                        l.FileUrl = GetFullUrl(l.FileUrl);
                    });
                });
                result.result = true;
                result.message = new
                {
                    records = musicType.Count,
                    list = musicType
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Music_All:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 音乐搜索  
        /// http://s.music.163.com/search/get/?type=1&offset=0&limit=5&s=爱
        /// s: 搜索词
        /// offset: 偏移量
        /// limit: 返回数量
        /// sub: 意义不明(非必须参数)；取值：false
        /// type: 搜索类型；取值意义
        /// 1 单曲
        /// 10 专辑
        /// 100 歌手
        /// 1000 歌单
        /// 1002 用户
        /// 1004 MV
        /// 1006 歌词 
        /// 1009 主播电台 
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/Search2")]
        public string Search2()
        {
            ApiResult result = new ApiResult();
            try
            {
                var name = SqlFilter(ZNRequest.GetString("name"));
                if (string.IsNullOrWhiteSpace(name))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var list = new List<Music>();
                var recordCount = 0;
                var pager = new Pager();
                var url = "http://music.163.com/api/search/pc";
                IDictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("offset", (pager.Index - 1) * pager.Size);
                dic.Add("limit", pager.Size);
                dic.Add("type", 1);
                dic.Add("s", name);
                var json = HttpUtil.Post(url, dic);

                var js = JObject.Parse(json);
                if (Tools.SafeInt(js["code"]) == 200)
                {
                    recordCount = Tools.SafeInt(js["result"]["songCount"]);
                    var songs = js["result"]["songs"].ToString();
                    JArray arr = JArray.Parse(songs);
                    foreach (JObject model in arr)
                    {
                        var fileUrl = model["mp3Url"].ToString(); ;
                        if (!string.IsNullOrWhiteSpace(fileUrl))
                        {
                            var success = true;
                            try
                            {
                                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(fileUrl);
                                wbRequest.Method = "GET";
                                wbRequest.Timeout = 2000;
                                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                                success = (wbResponse.StatusCode == System.Net.HttpStatusCode.OK);

                                if (wbResponse != null)
                                {
                                    wbResponse.Close();
                                }
                                if (wbRequest != null)
                                {
                                    wbRequest.Abort();
                                }
                                var music = new Music();
                                music.ID = Tools.SafeInt(model["id"]);
                                music.Name = model["name"].ToString();
                                var artists = JArray.Parse(model["artists"].ToString());
                                music.Author = ((JObject)artists[0])["name"].ToString();
                                var album = JObject.Parse(model["album"].ToString());
                                music.Cover = album["picUrl"].ToString();
                                music.FileUrl = fileUrl;
                                list.Add(music);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                            }
                        }
                    }
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = list
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Music_Search2:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 搜搜音乐
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/Search")]
        public string Search()
        {
            ApiResult result = new ApiResult();
            try
            {
                var name = SqlFilter(ZNRequest.GetString("name"));
                if (string.IsNullOrWhiteSpace(name))
                {
                    result.message = "参数异常";
                    return JsonConvert.SerializeObject(result);
                }
                var list = new List<Music>();

                var pager = new Pager();

                var totalPage = 100;
                var recordCount = pager.Size * totalPage;

                var url = string.Format("http://mp3.sogou.com/music.so?st=1&query={0}&comp=1&page={1}&len={2}", name, pager.Index, pager.Size);

                WebClient wc = new WebClient();
                byte[] pageSourceBytes = wc.DownloadData(new Uri(url));
                string source = Encoding.GetEncoding("GBK").GetString(pageSourceBytes);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(source);
                HtmlNodeCollection listNodes = doc.DocumentNode.SelectNodes("//li");

                //最后一页
                if (listNodes.Count < pager.Size)
                {
                    totalPage = pager.Index;
                    if (totalPage > 1)
                    {
                        recordCount = (pager.Index - 1) * pager.Size + listNodes.Count;
                    }
                    else
                    {
                        recordCount = listNodes.Count;
                    }
                }

                foreach (HtmlNode node in listNodes)
                {
                    //[#4801424#,#2#,#http://cc.stream.qqmusic.qq.com/C100002Zobl64HwPBi.m4a?fromtag=52#,#爱#,#5017#,#小虎队#,#426766#,#华纳国语情浓13首#,#100#,#56a453131fa27a35#]
                    //4801424
                    //2
                    //http://cc.stream.qqmusic.qq.com/C100002Zobl64HwPBi.m4a?fromtag=52
                    //爱
                    //5017
                    //小虎队
                    //426766
                    //华纳国语情浓13首
                    //100
                    //56a453131fa27a35

                    var param = node.Attributes["param"].Value.Trim();
                    var parts = param.Replace("[", "").Replace("]", "").Replace("#", "").Split(',');
                    var music = new Music();
                    music.ID = Tools.SafeInt(parts[6]);
                    music.Name = parts[3];
                    music.Author = parts[5];
                    music.Remark = parts[7];
                    music.Cover = "http://imgcache.qq.com/music/photo/album_300/" + parts[6].Substring(parts[6].Length - 2) + "/300_albumpic_" + music.ID + "_0.jpg";
                    music.FileUrl = parts[2];
                    list.Add(music);
                }

                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = list
                };

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Music_Search:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 搜搜音乐模块
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/SearchMenu")]
        public string SearchMenu()
        {
            ApiResult result = new ApiResult();
            try
            {
                var name = SqlFilter(ZNRequest.GetString("name"));
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = "全部";
                }

                var pager = new Pager();
                pager.Size = 20;

                var totalPage = 100;
                var recordCount = pager.Size * totalPage;

                var url = string.Format("http://mp3.sogou.com/tiny/dissList?query={0}&diss_type_name={0}&page={1}", name, pager.Index);

                WebClient wc = new WebClient();
                byte[] pageSourceBytes = wc.DownloadData(new Uri(url));
                string source = Encoding.GetEncoding("GBK").GetString(pageSourceBytes);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(source);
                HtmlNodeCollection listNodes = doc.DocumentNode.SelectSingleNode("//*[@class='playlist_list']").SelectNodes("li");

                //最后一页
                if (listNodes.Count < pager.Size)
                {
                    totalPage = pager.Index;
                    if (totalPage > 1)
                    {
                        recordCount = (pager.Index - 1) * pager.Size + listNodes.Count;
                    }
                    else
                    {
                        recordCount = listNodes.Count;
                    }
                }
                var list = new List<MusicMenu>();
                foreach (HtmlNode node in listNodes)
                {
                    HtmlDocument subDoc = new HtmlDocument();
                    subDoc.LoadHtml(node.InnerHtml);

                    var music = new MusicMenu();
                    music.Link = node.ChildNodes[0].Attributes["href"].Value.Trim();
                    music.Cover = subDoc.DocumentNode.SelectNodes("//img")[0].Attributes["src"].Value.Trim();
                    music.Name = subDoc.DocumentNode.SelectSingleNode("//span[@class='play_name']").SelectSingleNode("a").Attributes["title"].Value.Trim();
                    music.Child = new List<MusicMenuChild>();

                    var childNodes = subDoc.DocumentNode.SelectNodes("//span[@class='song_name']");
                    foreach (HtmlNode child in childNodes)
                    {
                        music.Child.Add(new MusicMenuChild(ChangeLan(child.ChildNodes[1].Attributes["href"].Value.Trim().Split('&')[1].Replace("query=", "")), ChangeLan(child.ChildNodes[3].Attributes["href"].Value.Trim().Split('&')[1].Replace("query=", ""))));
                    }
                    list.Add(music);
                }

                result.result = true;
                result.message = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = list
                };

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Music_SearchMenu:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 搜搜音乐模块
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/SearchMenuList")]
        public string SearchMenuList()
        {
            ApiResult result = new ApiResult();
            try
            {

                var list = new List<Music>();
                var url = ZNRequest.GetString("url");
                //url = "http://mp3.sogou.com/tiny/diss?diss_id=1739846437&query=YouTube上最受欢迎的50首音乐&diss_name=YouTube上最受欢迎的50首音乐";
                if (string.IsNullOrWhiteSpace(url))
                {
                    result.result = true;
                    result.message = new
                    {
                        currpage = 1,
                        records = 0,
                        totalpage = 1,
                        list = list
                    };
                    return JsonConvert.SerializeObject(result);
                }
                url = "http://mp3.sogou.com" + url;

                WebClient wc = new WebClient();
                byte[] pageSourceBytes = wc.DownloadData(new Uri(url));
                string source = Encoding.GetEncoding("GBK").GetString(pageSourceBytes);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(source);
                HtmlNodeCollection listNodes = doc.DocumentNode.SelectSingleNode("//*[@id='music_list']").SelectNodes("li");
                foreach (HtmlNode node in listNodes)
                {
                    var param = node.Attributes["param"].Value.Trim();
                    var parts = param.Replace("[", "").Replace("]", "").Replace("#", "").Split(',');
                    var music = new Music();
                    music.ID = Tools.SafeInt(parts[6]);
                    music.Name = parts[3];
                    music.Author = parts[5];
                    music.Remark = parts[7];
                    music.Cover = "http://imgcache.qq.com/music/photo/album_300/" + parts[6].Substring(parts[6].Length - 2) + "/300_albumpic_" + music.ID + "_0.jpg";
                    music.FileUrl = parts[2];
                    list.Add(music);
                }

                result.result = true;
                result.message = new
                {
                    currpage = 1,
                    records = listNodes.Count,
                    totalpage = 1,
                    list = list
                };

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Music_SearchMenuList:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        public string ChangeLan(string text)
        {
            return System.Web.HttpUtility.UrlDecode(text, Encoding.GetEncoding("GB2312"));
        }
    }
}