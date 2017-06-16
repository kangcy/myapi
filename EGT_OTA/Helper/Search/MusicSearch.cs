using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using PanGu;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using PanGu.HighLight;
using SubSonic.Repository;
using EGT_OTA.Models;
using CommonTools;

namespace EGT_OTA.Helper.Search
{
    public class MusicSearch : SearchBase
    {
        /// <summary>
        /// 资讯全文检索索引保存的路径
        /// </summary>
        private static DirectoryInfo indexPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("/Index/Music"));

        #region  资讯全文检索

        /// <summary>
        /// 检测索引是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IndexExists()
        {
            return IndexReader.IndexExists(FSDirectory.Open(indexPath));
        }

        #region  重置索引

        /// <summary>
        /// 重新创建所有资讯的索引(可能会花费较长时间)
        /// </summary>
        public static void IndexReset()
        {
            Index(new List<Music>(), false);
        }

        public static void Init(List<Music> all)
        {
            if (all == null)
            {
                return;
            }
            if (all.Count == 0)
            {
                return;
            }

            int recordCount = all.Count;
            int currentPage = 0;
            int pageSize = 50;
            double pageCount = Math.Ceiling(recordCount / (double)pageSize);
            var index = 0;
            try
            {
                for (int i = 0; i < pageCount; i++)
                {
                    index++;
                    var list = all.Skip(currentPage * pageSize).Take(pageSize).ToList();
                    Index(list, true);
                    currentPage++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("索引数2：" + ex.Message);
            }

            LogHelper.ErrorLoger.Error("索引数3：" + index);
        }


        /// <summary>
        /// 将资讯添加到索引中
        /// </summary>
        /// <param name="pList"></param>
        public static void Index(List<Music> pList, bool rewrite)
        {
            Analyzer analyzer = new PanGuAnalyzer();
            IndexWriter writer = new IndexWriter(FSDirectory.Open(indexPath), analyzer, rewrite, IndexWriter.MaxFieldLength.LIMITED);
            //添加到索引中
            foreach (Music p in pList)
            {
                AddDocument(writer, p);
            }
            writer.Commit();
            writer.Optimize();
            writer.Dispose();
        }

        /// <summary>
        /// 将商品添加到索引中
        /// </summary>
        /// <param name="pList"></param>
        public static void Index(List<Music> pList)
        {
            Index(pList, false);
        }

        /// <summary>
        /// 将资讯添加到索引中
        /// </summary>
        /// <param name="product"></param>
        public static void Index(Music Music)
        {
            Index(new List<Music>() { Music }, false);
        }

        /// <summary>
        /// 将资讯添加到索引中
        /// </summary>
        /// <param name="product"></param>
        public static void Index(Music Music, bool rewrite)
        {
            Index(new List<Music>() { Music }, rewrite);
        }

        /// <summary>
        /// 将资讯添加到索引器中
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int AddDocument(IndexWriter writer, Music p)
        {
            Document doc = new Document();
            doc.Add(new Field("Author", p.Author, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Name", p.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("FileUrl", p.FileUrl, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Cover", p.Cover, Field.Store.YES, Field.Index.NOT_ANALYZED));
            writer.AddDocument(doc);
            int num = writer.MaxDoc();
            return num;
        }

        #endregion

        #region 从索引中删除项目

        /// <summary>
        /// 从资讯索引中将索引删除
        /// </summary>
        /// <param name="IDList">需要删除索引的资讯的ID列表（以 , 隔开）</param>
        public static void MusicDelete(string IDList)
        {
            string[] arrMusicID = IDList.Split(',');
            IndexReader reader = IndexReader.Open(FSDirectory.Open(indexPath), false);
            for (int i = 0, len = arrMusicID.Length; i < len; i++)
            {
                Term term = new Term("Number", arrMusicID[i]);
                reader.DeleteDocuments(term);
            }
            reader.Commit();
            reader.Dispose();
        }

        /// <summary>
        /// 从资讯索引中将索引删除
        /// </summary>
        public static void MusicDelete(Music model)
        {
            MusicDelete(model.ID.ToString());
        }

        /// <summary>
        /// 从资讯索引中将索引删除
        /// </summary>
        public static void MusicDelete(List<Music> pList)
        {
            string IDList = "";
            foreach (Music p in pList)
            {
                IDList += "," + p.ID;
            }
            if (IDList.Length > 0)
            {
                MusicDelete(IDList.Substring(1));
            }
        }

        #endregion

        /// <summary>
        /// 资讯查询
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>符合查询条件的资讯列表</returns>
        public static List<Music> Search(string keyword, Sort sort, int currentPage, int pageSize, out int recordCount)
        {
            if (!IndexExists())
            {
                IndexReset();
            }
            if (sort == null)
            {
                sort = new Sort(new SortField("Number", 0));
            }
            Analyzer analyzer = new PanGuAnalyzer();
            BooleanQuery totalQuery = new BooleanQuery();
            MultiFieldQueryParser parser = new MultiFieldQueryParser(VERSION, new string[] { "Author", "Name" }, analyzer);



            if (!String.IsNullOrEmpty(keyword))
            {
                keyword = ClearKeyword(keyword);
                //Lucene.Net.Search.Query query = parser.Parse(SearchBase.AnalysisKeyword(keyword));

                LogHelper.ErrorLoger.Error("keyword:" + keyword);

                Query query = new WildcardQuery(new Term("Name", "*" + keyword + "*"));

                //Query query = parser.Parse(keyword);


                LogHelper.ErrorLoger.Error("keyword:" + query.ToString());

                totalQuery.Add(query, Occur.MUST);
            }
            if (currentPage < 0)
            {
                currentPage = 0;
            }
            int beginIndex = currentPage * pageSize;
            int lastIndex = (currentPage + 1) * pageSize;

            IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(indexPath), true);
            TopDocs result = searcher.Search(totalQuery, null, lastIndex, sort);
            ScoreDoc[] hits = result.ScoreDocs;
            recordCount = result.TotalHits;
            if (recordCount < lastIndex)
            {
                lastIndex = recordCount;
            }
            List<Music> pList = new List<Music>();
            Music p = null;

            for (int i = beginIndex; i < lastIndex; i++)
            {
                var doc = searcher.Doc(hits[i].Doc);
                p = new Music();
                p.Author = doc.Get("Author");
                p.Name = doc.Get("Name");
                p.FileUrl = doc.Get("FileUrl");
                p.Cover = doc.Get("Cover");
                pList.Add(p);
            }
            searcher.Dispose();
            return pList;
        }

        #endregion
    }
}