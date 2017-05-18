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

namespace EGT_OTA.Controllers.Api
{
    public class MusicController : BaseApiController
    {
        protected readonly SimpleRepository musicdb = MusicRepository.GetRepo();

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
                var pager = new Pager();

                var music01 = musicdb.Find<Music01>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music02 = musicdb.Find<Music02>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music03 = musicdb.Find<Music03>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music04 = musicdb.Find<Music04>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music05 = musicdb.Find<Music05>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music06 = musicdb.Find<Music06>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music07 = musicdb.Find<Music07>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music08 = musicdb.Find<Music08>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music09 = musicdb.Find<Music09>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music10 = musicdb.Find<Music10>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music11 = musicdb.Find<Music11>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music12 = musicdb.Find<Music12>(x => x.Name.Contains(name) || x.Author.Contains(name));
                var music13 = musicdb.Find<Music13>(x => x.Name.Contains(name) || x.Author.Contains(name));

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

                var recordCount = list.Count;

                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
                }
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                list = list.OrderByDescending(x => x.ID).Skip((pager.Index - 1) * pager.Size).Take(pager.Size).ToList();
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
        /// 音乐搜索
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
                //if (string.IsNullOrWhiteSpace(name))
                //{
                //    result.message = "参数异常";
                //    return JsonConvert.SerializeObject(result);
                //}
                var pager = new Pager();

                var recordCount = 0;
                List<Music> list = MusicSearch.Search(name, null, pager.Index - 1, pager.Size, out recordCount);

                if (recordCount == 0)
                {
                    result.message = new { records = recordCount, totalpage = 1 };
                    return JsonConvert.SerializeObject(result);
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
    }
}