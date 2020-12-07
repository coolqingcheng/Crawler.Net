using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Net.DownLoader
{
    public class HtmlDownLoader : IHtmlDownLoader
    {
        private readonly IHttpClientFactory _clientFactory;

        public HtmlDownLoader(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> DownAsync(string url)
        {
            var http = _clientFactory.CreateClient("gzip");
            //判断是否有cookie 
            var cookiepath = Path.Combine(Directory.GetCurrentDirectory(), "cookie.txt");
            if (File.Exists(cookiepath))
            {
                var cookie = await File.ReadAllTextAsync(cookiepath);
                http.DefaultRequestHeaders.Add("cookie", cookie);
            }
            http.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (compatible; Baiduspider-render/2.0; +http://www.baidu.com/search/spider.html)");
            var resp = await http.GetAsync(url);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var res = await resp.Content.ReadAsStringAsync();
                return res;
            }
            throw new Exception($"地址:{url} 响应为:{resp.StatusCode}");
        }
    }

    /// <summary>
    /// Html下载器
    /// </summary>
    public interface IHtmlDownLoader
    {
        /// <summary>
        /// 读取到响应
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<string> DownAsync(string url);
    }
}
