using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Net.DownLoader
{
    public class JsonAjaxDownLoader : IAjaxDownLoader
    {
        private readonly IHttpClientFactory _clientFactory;

        public JsonAjaxDownLoader(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> DownAsync(string url, string data)
        {
            var http = _clientFactory.CreateClient("gzip");
            StringContent content = new StringContent(data, Encoding.Default);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var resp = await http.PostAsync(url, content);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var res = await resp.Content.ReadAsStringAsync();
                return res;
            }
            throw new Exception($"地址:{url} 响应为:{resp.StatusCode}");
        }
    }

    public interface IAjaxDownLoader
    {
        /// <summary>
        /// 默认是Json对象 Form没实现
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> DownAsync(string url, string data);
    }
}
