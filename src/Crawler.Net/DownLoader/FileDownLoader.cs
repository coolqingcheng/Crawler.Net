using Crawler.Net.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Net.DownLoader
{
    /// <summary>
    /// 图片下载器
    /// </summary>
    public class FileDownLoader : IFileDownLoader
    {
        private readonly IHttpClientFactory _clientFactory;
        private ILogger _logger;

        public FileDownLoader(IHttpClientFactory clientFactory, ILogger<FileDownLoader> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<Tuple<string, string>> DownAsync(string url, string folderpath)
        {
            var expandName = url.TryGetUrlFileExpandName(".jpg");

            string fileName = Guid.NewGuid().ToString("N") + expandName;
            if (Directory.Exists(folderpath) == false)
            {
                Directory.CreateDirectory(folderpath);
            }
            var localfile = Path.Combine(folderpath, fileName);
            var bytes = await _clientFactory.CreateClient("gzip").GetByteArrayAsync(url);
            using FileStream fs = new FileStream(localfile, FileMode.OpenOrCreate);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            return new Tuple<string, string>(localfile, url);
        }

        public async Task<Tuple<Stream, string>> DownAsync(string url, string referer, bool isReferer)
        {
            try
            {
                string fileName = "";
                int count = 0;
                while (count <= 3)
                {
                    var client = _clientFactory.CreateClient("gzip");
                    if (referer.IsNotEmpty())
                    {
                        client.DefaultRequestHeaders.Add("referer", referer);
                    }
                    var responseMessage = await client.GetAsync(url);
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        fileName = $"image.{MimeUtility.GetExtensionByMime(responseMessage.Content.Headers.ContentType.MediaType)}";
                        //fileName = Path.GetFileName(url);
                        var stream = await responseMessage.Content.ReadAsStreamAsync();
                        return new Tuple<Stream, string>(stream, fileName);

                    }
                    count += 1;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "下载文件错误:" + url);
                return null;
            }
        }
    }

    /// <summary>
    /// 文件下载器
    /// </summary>
    public interface IFileDownLoader
    {
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <param name="locationpath">本地存储地址</param>
        /// <returns>item1 本地地址  item2 远程地址</returns>
        Task<Tuple<string, string>> DownAsync(string url, string folderpath);

        /// <summary>
        /// 下载文件到流
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<Tuple<Stream, string>> DownAsync(string url, string referer = null, bool isReferer = false);
    }
}
