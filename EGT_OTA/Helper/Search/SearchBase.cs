using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Lucene.Net.Analysis.PanGu;
using PanGu;

namespace EGT_OTA.Helper.Search
{
    public class SearchBase
    {
        public const Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;

        /// <summary>
        /// 对关键字进行分词
        /// </summary>
        /// <param name="keywords">需要分词的关键字</param>
        /// <returns>分词后的关键字</returns>
        public static string AnalysisKeyword(string keywords)
        {
            StringBuilder result = new StringBuilder();
            ICollection<WordInfo> words = new PanGuTokenizer().SegmentToWordInfos(keywords);
            foreach (WordInfo word in words)
            {
                if (word == null)
                    continue;
                result.AppendFormat("{0}^{1}.0 ", word.Word, (int)Math.Pow(3, word.Rank));
            }
            return result.ToString().Trim();
        }

        /// <summary>
        /// 清除关键字中的特殊字符
        /// </summary>
        /// <param name="keyword">查询的关键字</param>
        /// <returns>清楚特殊字符后的查询关键字</returns>
        public static string ClearKeyword(string keyword)
        {
            if (String.IsNullOrEmpty(keyword))
            {
                return "";
            }
            Regex reg = new Regex(@"([^A-Za-z0-9\u4e00-\u9fa5])", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            keyword = reg.Replace(keyword, "");
            return keyword;
        }
    }
}