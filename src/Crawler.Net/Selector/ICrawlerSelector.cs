using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Net.Selector
{
    public interface ICrawlerSelector
    {
        /// <summary>
        /// 解析一个单个的html文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <returns></returns>
        T Parse<T>(string html) where T : class, new();

        /// <summary>
        /// 解析html文件到一个列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <returns></returns>
        IEnumerable<T> ParseToList<T>(string html) where T : class, new();

        /// <summary>
        /// 使用正则表达式匹配出当前里面的网址
        /// </summary>
        /// <param name="html"></param>
        /// <param name="rexstr"></param>
        /// <returns></returns>
        IEnumerable<string> ParseUrlByRex(string html, string rexstr);
    }
}
