using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Net.Request
{
    public class SpiderRequest
    {
        /// <summary>
        /// 所有的请求头数据
        /// </summary>
        public Dictionary<string, string> header { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 请求标识，表示这是干啥的，比如 【主页列表，详情页】
        /// </summary>
        public string ReqName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 从哪儿来的
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// UA
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// cookie
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public RequestType Reqtype { get; set; } = RequestType.HTML;

        /// <summary>
        /// 重试次数
        /// </summary>
        public int ReTryTimes { get; set; }

        /// <summary>
        /// 请求开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 请求结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 抓取当前Xpath下的所有图片
        /// </summary>
        public string GetImageXpath { get; set; }

        /// <summary>
        /// 抓取的图片，k 网页链接 v本地图片存储路径
        /// </summary>
        public Dictionary<string, string> Images { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 更多表达式，根据当前表达式解析符合规则的标签
        /// </summary>
        public string MoreExpression { get; set; }

        /// <summary>
        /// 解析出更多表达式的规则，放到对应的ReqName字段上去
        /// </summary>
        public string MoreExpressionName { get; set; }

        /// <summary>
        /// 在管道中传递的数据
        /// </summary>
        public Dictionary<string, object> ItemData { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// [成功或者失败]是否保存
        /// </summary>
        public bool IfSave { get; set; } = true;

        /// <summary>
        /// 是否是Ajax
        /// </summary>
        public bool IsAjax { get; set; }

        /// <summary>
        /// Ajax数据
        /// </summary>
        public string AjaxParamData { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public string Method { get; set; } = "GET";
    }

    /// <summary>
    /// 请求类型
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// 图片
        /// </summary>
        IMAGE = 0,

        /// <summary>
        /// Html
        /// </summary>
        HTML = 1,

        /// <summary>
        /// 文件
        /// </summary>
        FILE = 2
    }
}
