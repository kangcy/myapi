//---------------------------------------------------------------- 
//Copyright (C) 2011-2012 Nanjing Axon Technology Co.,Ltd
//Http://www.axon.com.cn 
// All rights reserved.
//<author>万刚</author>
//<createDate>2014/12/8 10:26:05</createDate>
//<description>AlipayConfig.cs
//</description>
//----------------------------------------------------------------

using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace IPay.Alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config
    {
        #region 字段
        private static string partner = "";
        private static string private_key = "";
        private static string public_key = "";
        private static string input_charset = "";
        private static string sign_type = "";
        #endregion

        static Config()
        {
            //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            partner = "2088001956299540";

            //商户的私钥
            private_key = @"MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAOj86A4R2FmIVHkwW/fV/EDU1oeGifuGUd5asUGitYQ3wyURHvA57z6SjtjPB9Ryi1f9KwZziy5oXgsaOSbZ+1ntk9/50IlSexFPlblLdWertxHV5mbN6nJuCZg5sOER1SRjZsvTXCN9gO2cUXGHbTujIpksqrYcNN/N0Uy6GpFjAgMBAAECgYEA0Yu9LgJQwl3CE+kxnhqgMLL8a5HOgiERetm4uN6dQNhiM/FNESQaD/4Cae7yDNokhzOUwc2jrU6C3ptsMYw16bj0EHJqBLqU7EVmPDr7em6oFGaSVc42TOZNWqpIg7Bm5PZ6yJgZvNd/t2AkXo4JvH6Kt/WiAUpj05FcUTU5HbkCQQD+8I5rG4h5PRTDBY+l2dW2g05Cy9NTVEw2IgokN03P7WyCrsoGKDxMqQga7wYwMgbizxOu9aueuwLgy+PqC8hdAkEA6fT6MB+lJgFHOE6/UPXc8BjEOkUC+7aOv1UZTCDxr+yM9y/oqwiUg1S3NkrAEwvjn7MJ70yBXZaAiXVmcX8kvwJAYRk8FR1SeGLEQpcepBt4o2AVcalyHp3PvRpv5GVP9K7IEmoCNiAi/0ut85wwLjEPoFkgdRXKvNUbfoUJlH3SXQJAZX2eYu33aIs5aBXRLL/bflRgG58Ack15k0rJVJsd/WEyrbCc0EVCl85SRD0dIaYQsqCqeKLJo928GOVS8X1kZQJBAItwebw7QPS9BQP3v04eAE46sEFELH4WVh0xAqYVrFiMxXIv9BvR21kwveOIwwwlfI/x97UwxpDFt3NBkBJEGwU=";

            //支付宝的公钥，无需修改该值
            public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";

            //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑



            //字符编码格式 目前支持 gbk 或 utf-8
            input_charset = "utf-8";

            //签名方式，选择项：RSA、DSA、MD5
            sign_type = "RSA";
        }

        #region 属性
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public static string Private_key
        {
            get { return private_key; }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public static string Public_key
        {
            get { return public_key; }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return sign_type; }
        }
        #endregion
    }
}
