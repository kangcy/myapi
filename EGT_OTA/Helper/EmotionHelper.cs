using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EGT_OTA.Helper
{
    public class EmotionHelper
    {
        /// <summary>
        /// 判断是否表情
        /// </summary>
        public static bool IsEmojiCharacter(char codePoint)
        {
            return !((codePoint == 0x0) ||
                    (codePoint == 0x9) ||
                    (codePoint == 0xA) ||
                    (codePoint == 0xD) ||
                    ((codePoint >= 0x20) && (codePoint <= 0xD7FF)) ||
                    ((codePoint >= 0xE000) && (codePoint <= 0xFFFD)) ||
                    ((codePoint >= 0x10000) && (codePoint <= 0x10FFFF)));
        }

        /// <summary>
        /// 过滤表情
        /// </summary>
        /// <param name="str"></param>
        public static string EmotionFilter(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            StringBuilder sbr = new StringBuilder();
            var list = UnicodeHelper.ToUnicode(str).Split('\\').ToList();
            var index = 0;
            foreach (char c in str)
            {
                index++;
                var isEmoji = EmotionHelper.IsEmojiCharacter(c);
                if (isEmoji)
                {
                    sbr.Append(@"\" + list[index]);
                }
                else
                {
                    sbr.Append(c.ToString());
                }
            }
            return sbr.ToString();
        }
    }
}