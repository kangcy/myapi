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
        /// 搜搜音乐、阿里云音乐
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/Search")]
        public string Search()
        {
            ApiResult result = new ApiResult();
            var name = SqlFilter(ZNRequest.GetString("name"));
            if (string.IsNullOrWhiteSpace(name))
            {
                result.message = "参数异常";
                return JsonConvert.SerializeObject(result);
            }
            var list = new List<Music>();
            var pager = new Pager();
            var recordCount = 0;
            var totalPage = 0;

            //是否调用阿里云音乐
            var isaly = true;
            try
            {
                if (isaly)
                {
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
                        JArray arr = JArray.Parse(js["result"]["songs"].ToString());
                        foreach (JObject model in arr)
                        {
                            var music = new Music();
                            music.ID = Tools.SafeInt(model["id"]);
                            music.Name = model["name"].ToString();
                            var artists = JArray.Parse(model["artists"].ToString());
                            music.Author = ((JObject)artists[0])["name"].ToString();
                            var album = JObject.Parse(model["album"].ToString());
                            music.Remark = album["name"].ToString();
                            music.Cover = album["picUrl"].ToString() + "?param=100y100";
                            var dic2 = new Dictionary<string, object>();
                            dic2.Add("url", "http://music.163.com/#/song?id=" + music.ID);
                            var filrurl = HttpUtil.Post("http://i.oppsu.cn/link/geturl.php", dic2);//阿里云解析 http://i.oppsu.cn/link/
                            try
                            {
                                var file = JObject.Parse(filrurl);
                                if (Tools.SafeInt(file["status"]) == 1)
                                {
                                    music.FileUrl = file["data"].ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                music.FileUrl = "";
                            }
                            list.Add(music);
                        }
                    }
                    totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                }
                else
                {
                    totalPage = 100;
                    recordCount = pager.Size * totalPage;
                    var url = string.Format("http://mp3.sogou.com/music.so?st=1&query={0}&comp=1&page={1}&len={2}", name, pager.Index, pager.Size);
                    WebClient wc = new WebClient();
                    byte[] pageSourceBytes = wc.DownloadData(new Uri(url));
                    string source = Encoding.GetEncoding("GBK").GetString(pageSourceBytes);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(source);
                    HtmlNodeCollection listNodes = doc.DocumentNode.SelectNodes("//li");
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
                    music.Link = UrlDecode_Encoding(node.ChildNodes[0].Attributes["href"].Value.Trim());
                    music.Cover = subDoc.DocumentNode.SelectNodes("//img")[0].Attributes["src"].Value.Trim();
                    music.Name = subDoc.DocumentNode.SelectSingleNode("//span[@class='play_name']").SelectSingleNode("a").Attributes["title"].Value.Trim();
                    music.Child = new List<MusicMenuChild>();

                    var childNodes = subDoc.DocumentNode.SelectNodes("//span[@class='song_name']");
                    foreach (HtmlNode child in childNodes)
                    {
                        music.Child.Add(new MusicMenuChild(UrlDecode_Encoding(child.ChildNodes[1].Attributes["href"].Value.Trim().Split('&')[1].Replace("query=", "")), UrlDecode_Encoding(child.ChildNodes[3].Attributes["href"].Value.Trim().Split('&')[1].Replace("query=", ""))));
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

                //http://mp3.sogou.comtiny/diss?diss_id=1138337852&query=卖场音乐：每次进店都有种要上T台的感觉！&diss_name=卖场音乐：每次进店都有种要上T台的感觉！

                url = "http://mp3.sogou.com" + url;
                WebClient wc = new WebClient();
                byte[] pageSourceBytes = wc.DownloadData(new Uri(url));
                string source = Encoding.GetEncoding("GBK").GetString(pageSourceBytes);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(source);
                HtmlNodeCollection listNodes = doc.DocumentNode.SelectSingleNode("//ul[@id='music_list']").SelectNodes("li");
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

        /// <summary>
        /// 榜单排行榜
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/SearchTop")]
        public string SearchTop()
        {
            ApiResult result = new ApiResult();
            try
            {
                var list = new List<MusicTop>();
                if (CacheHelper.Exists("MusicTop"))
                {
                    list = (List<MusicTop>)CacheHelper.GetCache("MusicTop");
                }
                else
                {
                    var url = "http://mp3.sogou.com/bang_list.html";
                    WebClient wc = new WebClient();
                    byte[] pageSourceBytes = wc.DownloadData(new Uri(url));
                    string source = Encoding.GetEncoding("UTF-8").GetString(pageSourceBytes);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(source);
                    HtmlNodeCollection listNodes = doc.DocumentNode.SelectNodes("//div[@class='m_tit']");
                    var index = 0;
                    foreach (HtmlNode node in listNodes)
                    {
                        HtmlDocument subDoc = new HtmlDocument();
                        subDoc.LoadHtml(node.InnerHtml);

                        MusicTop top = new MusicTop();
                        top.Name = subDoc.DocumentNode.ChildNodes[1].InnerText;
                        top.Child = new List<Music>();

                        HtmlNodeCollection childListNodes = doc.DocumentNode.SelectNodes("//ul[@class='bang_list']")[index].SelectNodes("./li");
                        foreach (HtmlNode childNode in childListNodes)
                        {
                            var param = childNode.SelectSingleNode("./a[last()]").Attributes["onclick"].Value.Trim();
                            param = param.Substring(param.IndexOf("#"), param.Length - param.IndexOf("#"));
                            param = param.Substring(0, param.LastIndexOf("#"));
                            var parts = param.Replace("#", "").Split(',');
                            var music = new Music();
                            music.ID = Tools.SafeInt(parts[6]);
                            music.Name = parts[3];
                            music.Author = parts[5];
                            music.Remark = parts[7];
                            music.Cover = "http://imgcache.qq.com/music/photo/album_300/" + parts[6].Substring(parts[6].Length - 2) + "/300_albumpic_" + music.ID + "_0.jpg";
                            music.FileUrl = parts[2];
                            top.Child.Add(music);
                        }
                        list.Add(top);
                        index += 1;
                    }
                    if (list.Count > 0)
                    {
                        CacheHelper.Insert("MusicTop", list, TimeSpan.FromDays(1));
                    }
                }
                result.result = true;
                result.message = new
                {
                    currpage = 1,
                    records = list.Count,
                    totalpage = 1,
                    list = list
                };
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Music_SearchTop:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 音乐排行榜
        /// 接口请求地址：http://i.oppsu.cn/index.php?c=index&a=gettop100
        /// 数据源地址：https://rss.itunes.apple.com/api/v1/CN/apple-music/top-songs/all/10/explicit.json
        /// CN：中国
        /// 10：取10条数据
        /// </summary>
        [DeflateCompression]
        [HttpGet]
        [Route("Api/Music/MusicTop")]
        public string MusicTop()
        {
            ApiResult result = new ApiResult();
            try
            {
                var list = new List<Music>();
                if (CacheHelper.Exists("MusicTop"))
                {
                    list = (List<Music>)CacheHelper.GetCache("MusicTop");
                }
                else
                {
                    var recordCount = 0;
                    var totalPage = 0;
                    var json = HttpUtil.Get("https://rss.itunes.apple.com/api/v1/CN/apple-music/top-songs/all/10/explicit.json");
                    var js = JObject.Parse(json);

                    JArray arr = JArray.Parse(js["feed"]["results"].ToString());
                    recordCount = arr.Count;
                    foreach (JObject model in arr)
                    {
                        var music = new Music();
                        music.ID = Tools.SafeInt(model["id"]);
                        music.Name = model["name"].ToString();
                        music.Author = model["artistName"].ToString(); ;
                        music.Remark = model["collectionName"].ToString();
                        music.Cover = model["artworkUrl100"].ToString();
                        var dic = new Dictionary<string, object>();
                        dic.Add("country", "CN");
                        dic.Add("albumid", music.ID);
                        var filrurl = HttpUtil.Post("http://i.oppsu.cn/index.php?c=index&a=getsongpreview", dic);
                        try
                        {
                            var file = JObject.Parse(filrurl);
                            if (Tools.SafeInt(file["status"]) == 1)
                            {
                                music.FileUrl = file["data"].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            music.FileUrl = "";
                        }
                        list.Add(music);
                    }
                    result.result = true;
                    result.message = new
                    {
                        currpage = 1,
                        records = recordCount,
                        totalpage = totalPage,
                        list = list
                    };
                    if (list.Count > 0)
                    {
                        CacheHelper.Insert("MusicTop", list, TimeSpan.FromDays(7));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("Api_Music_MusicTop:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

        public string UrlDecode_Encoding(string text)
        {
            return System.Web.HttpUtility.UrlDecode(text, Encoding.GetEncoding("GB2312"));
        }
    }
}