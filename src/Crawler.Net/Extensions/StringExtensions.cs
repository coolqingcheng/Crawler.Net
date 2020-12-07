using Ganss.XSS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Net.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 去除重复
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// 摘要
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string ZhaiyaoTxt(this string txt, int len)
        {
            if (txt.Length < len)
            {
                return txt;
            }
            else
            {
                return txt.Substring(0, len);
            }
        }


        public static int ToInt(this object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 去掉空格换行符等
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string RemoveEmpty(this string txt)
        {
            var res = txt.Replace("\n", "").Replace("\t", "").Replace("\r", "");
            return res;
        }

        /// <summary>
        /// 把Id转成对应的字符串
        /// </summary>
        /// <returns></returns>
        public static string IdToString(this int id)
        {
            var dic = new Dictionary<int, string>();
            dic.Add(0, "w");
            dic.Add(1, "v");
            dic.Add(2, "c");
            dic.Add(3, "d");
            dic.Add(4, "j");
            dic.Add(5, "y");
            dic.Add(6, "f");
            dic.Add(7, "g");
            dic.Add(8, "q");
            dic.Add(9, "z");
            var str = id.ToString();
            foreach (var item in dic)
            {
                str = str.Replace(item.Key.ToString(), item.Value);
            }
            return str;
        }

        /// <summary>
        /// 把字符串转成数字ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int StringToId(this string id)
        {
            var dic = new Dictionary<int, string>();
            dic.Add(0, "w");
            dic.Add(1, "v");
            dic.Add(2, "c");
            dic.Add(3, "d");
            dic.Add(4, "j");
            dic.Add(5, "y");
            dic.Add(6, "f");
            dic.Add(7, "g");
            dic.Add(8, "q");
            dic.Add(9, "z");
            var str = id.ToString();
            foreach (var item in dic)
            {
                str = str.Replace(item.Value, item.Key.ToString());
            }
            return Convert.ToInt32(str);
        }

        /// <summary>
        /// 尝试转换成int
        /// </summary>
        /// <param name="v"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static bool TryToInt(this string v, out int res)
        {
            res = 0;
            try
            {
                res = Convert.ToInt32(v);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 日期转Unix时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToServerLocalUnixTimeNumber(this DateTime time)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
            return unixTime;
        }

        /// <summary>
        /// 对象转换Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// json转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string str)
        {
            if (str.IsNull())
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            if (obj == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果当前对象不为空或者空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str) == false)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果不不等于空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNull(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return true;
            }
            return false;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        /// <summary>
        /// 过滤富文本中的XSS,[使用的是默认过滤配置]
        /// 更多配置:https://github.com/mganss/HtmlSanitizer
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string FilterXss(this string html)
        {
            var sanitizer = new HtmlSanitizer();
            string str = sanitizer.Sanitize(html);
            return str;
        }
    }

}
