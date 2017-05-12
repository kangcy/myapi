﻿using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace EGT_OTA.Helper
{

    /// <summary>
    /// 公历/农历日期转换
    /// </summary>
    public class ChineseLunisolar
    {
        public ChineseLunisolar() { }

        /// <summary>
        /// 根据公历获取农历日期
        /// </summary>
        /// <param name="datetime">公历日期</param>
        public static string GetChineseDateTime(DateTime datetime)
        {
            System.Globalization.ChineseLunisolarCalendar cCalendar = new System.Globalization.ChineseLunisolarCalendar();
            int lyear = cCalendar.GetYear(datetime);
            int lmonth = cCalendar.GetMonth(datetime);
            int lday = cCalendar.GetDayOfMonth(datetime);

            //获取闰月， 0 则表示没有闰月
            int leapMonth = cCalendar.GetLeapMonth(lyear);

            bool isleap = false;

            if (leapMonth > 0)
            {
                if (leapMonth == lmonth)
                {
                    //闰月
                    isleap = true;
                    lmonth--;
                }
                else if (lmonth > leapMonth)
                {
                    lmonth--;
                }
            }
            string ret = string.Concat(GetLunisolarYear(datetime.Year), "年", isleap ? "闰" : string.Empty, GetLunisolarMonth(lmonth), "月", GetLunisolarDay(lday));
            ret = "农历" + ret;
            return ret;
        }

        #region 农历年

        /// <summary>
        /// 十天干
        /// </summary>
        private static string[] tiangan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };

        /// <summary>
        /// 十二地支
        /// </summary>
        private static string[] dizhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };

        /// <summary>
        /// 十二生肖
        /// </summary>
        private static string[] shengxiao = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

        /// <summary>
        /// 返回农历天干地支年 
        /// </summary>
        /// <param name="year">公历年</param>
        public static string GetLunisolarYear(int year)
        {
            if (year > 3)
            {
                int tgIndex = (year - 4) % 10;
                int dzIndex = (year - 4) % 12;
                return string.Concat(tiangan[tgIndex], dizhi[dzIndex], "", shengxiao[dzIndex], "");
            }
            throw new ArgumentOutOfRangeException("无效的年份!");
        }

        #endregion

        #region 农历月

        /// <summary>
        /// 农历月
        /// </summary>
        private static string[] months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二(腊)" };

        /// <summary>
        /// 返回农历月
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns></returns>
        public static string GetLunisolarMonth(int month)
        {
            if (month < 13 && month > 0)
            {
                return months[month - 1];
            }
            throw new ArgumentOutOfRangeException("无效的月份!");
        }

        #endregion

        #region 农历日

        private static string[] days1 = { "初", "十", "廿", "三" };

        /// <summary>
        /// 日
        /// </summary>
        private static string[] days = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

        /// <summary>
        /// 返回农历日
        /// </summary>
        public static string GetLunisolarDay(int day)
        {
            if (day > 0 && day < 32)
            {
                if (day != 20 && day != 30)
                {
                    return string.Concat(days1[(day - 1) / 10], days[(day - 1) % 10]);
                }
                else
                {
                    return string.Concat(days[(day / 10) - 1], days1[1]);
                }
            }
            throw new ArgumentOutOfRangeException("无效的日!");
        }

        #endregion
    }

}