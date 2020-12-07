using Crawler.Net.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Net.Response
{
    public class SpiderResponse
    {
        public SpiderRequest Request { get; set; }

        /// <summary>
        /// 注入对象
        /// </summary>
        public IServiceProvider serviceProvider { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string CharSet { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// 数据包
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 获取当前页所有的图片
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetImages()
        {
            return Request.Images;
        }
    }

}
