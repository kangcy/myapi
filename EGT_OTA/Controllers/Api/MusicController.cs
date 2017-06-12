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
                LogHelper.ErrorLoger.Error("Api_Music_Search:" + ex.Message);
                result.message = ex.Message;
            }
            return JsonConvert.SerializeObject(result);
        }

    }
}