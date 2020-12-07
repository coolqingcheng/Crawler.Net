using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Net.Selector
{
    /// <summary>
    /// 选择器selector
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SelectorAttribute : Attribute
    {
        /// <summary>
        /// xpath解析 1
        /// </summary>
        public string Xpath { get; set; }

        /// <summary>
        /// 正则表达式 2
        /// </summary>
        public string Rex { get; set; }

        /// <summary>
        /// 是否是列表
        /// </summary>
        public bool IsList { get; set; } = false;

        /// <summary>
        /// 替换字符串，第一个是旧的，第二个是新的
        /// </summary>
        public string[] Replace { get; set; }

        /// <summary>
        /// 是否抓取全部图片,按逗号分隔返回
        /// </summary>
        public bool GetAllImage { get; set; }

        /// <summary>
        /// 需要去掉的标签
        /// </summary>
        public string[] TakeOutTag { get; set; }

        /// <summary>
        /// 需要把哪些标签替换成文字
        /// </summary>
        public string[] ReplaceTagToTxt { get; set; }

        /// <summary>
        /// 采集类型 默认是文字
        /// </summary>
        public SelectType selectType { get; set; } = SelectType.TEXT;

        /// <summary>
        /// 解析方式，默认是Xpath
        /// </summary>
        public ParseType parseType { get; set; } = ParseType.Xpath;
    }

    public enum ParseType
    {
        /// <summary>
        /// 用xpath解析
        /// </summary>
        Xpath = 0,

        /// <summary>
        /// 用正则表达式解析
        /// </summary>
        Rex = 1
    }

    public enum SelectType
    {
        TEXT = 0,

        HTML = 1
    }
}
