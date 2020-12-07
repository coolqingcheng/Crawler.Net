using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Crawler.Net.Extensions
{
    public static class UrlUtils
    { /// <summary>
      /// 获取URL地址中的文件扩展名
      /// </summary>
      /// <param name="url"></param>
      /// <param name="expandName"></param>
      /// <returns></returns>
        public static string TryGetUrlFileExpandName(this string url, string defaultName)
        {
            var name = Regex.Match(url, @"\.[a-zA-Z]*$")?.Value;
            if (name == null)
            {
                name = defaultName;
            }
            return name;
        }

        /// <summary>
        /// 尝试通过父级的URL补全当前的文件地址
        /// </summary>
        /// <param name="fileUri">文件URL地址</param>
        /// <param name="baseUrl">父级页面的URL地址</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool TryUrlCompletion(this string fileUri, string baseUrl, out string url)
        {
            //验证父级URL是否正确
            string host = Regex.Match(baseUrl, @"(https|http)://(\w|\.)*/")?.Value;
            url = string.Empty;
            if (string.IsNullOrWhiteSpace(host))
            {
                return false;
            }
            //验证是否是全的地址URL

            var filehost = Regex.Match(fileUri, @"^(https|http)://(\w|\.)*/")?.Value;
            if (!string.IsNullOrWhiteSpace(filehost))
            {
                url = fileUri;
                return true;
            }

            //匹配出文件地址中是否需要补全
            if (string.IsNullOrWhiteSpace(Regex.Match(fileUri, @"^((https|http)://|(//))(\w|\.)*/")?.Value))
            {
                var baseurl = new Uri(host);
                url = new Uri(baseurl, fileUri).ToString();
                return true;
            }
            //如果是双斜杠域名，根据baseUrl进行替换补全
            if (string.IsNullOrWhiteSpace(Regex.Match(fileUri, @"^//(\w|\.)*/")?.Value) == false)
            {
                url = Regex.Replace(fileUri, @"^//", "http://");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算哈希值字符串
        /// </summary>
        public static string ComputeHash(this string url)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(url);
            if (buffer == null || buffer.Length < 1)
                return "";

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();

            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
