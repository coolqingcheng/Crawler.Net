using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Crawler.Net.Selector
{
    public class CrawlerSelector : ICrawlerSelector
    {
        public T Parse<T>(string html) where T : class, new()
        {
            var t = new T();
            var properties = t.GetType().GetProperties();
            if (properties == null)
            {
                return null;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            //移除注释
            //移除注释
            var comments = doc.DocumentNode.SelectNodes("//comment()");
            if (comments != null)
            {
                foreach (var item in comments)
                {
                    item.Remove();
                }
            }
            //移除script
            var scripts = doc.DocumentNode.Descendants("script").ToArray();
            if (scripts != null)
            {
                foreach (var item in scripts)
                {
                    item.Remove();
                }
            }
            //移除iframe
            var iframes = doc.DocumentNode.Descendants("iframe").ToArray();
            if (iframes != null)
            {
                foreach (var item in iframes)
                {
                    item.Remove();
                }
            }
            foreach (var item in properties)
            {
                var att = item.GetCustomAttribute<SelectorAttribute>();
                if (att != null && att.parseType == ParseType.Xpath)
                {
                    if (att.selectType == SelectType.HTML || att.GetAllImage)
                    {
                        var contentNode = doc.DocumentNode.SelectSingleNode(att.Xpath);

                        var htmlstr = contentNode?.InnerHtml;
                        if (att.Replace != null && att.Replace.Length == 2)
                        {
                            htmlstr = htmlstr.Replace(att.Replace[0], att.Replace[1]);
                        }

                        // todo 替换某些标签为文字
                        if (att.ReplaceTagToTxt != null)
                        {
                            foreach (var temp in att?.ReplaceTagToTxt)
                            {
                                var tags = contentNode.Descendants(temp);
                                foreach (var tag in tags)
                                {
                                    var outer = tag.OuterHtml;
                                    var inner = tag.InnerHtml;
                                    htmlstr = htmlstr.Replace(outer, inner);
                                }
                            }
                        }
                        if (att.GetAllImage)
                        {
                            var nodes = doc.DocumentNode.SelectSingleNode(att.Xpath)?.Descendants("img");
                            if (nodes != null)
                            {
                                var imgs = string.Join(",", nodes.Select(a => a.GetAttributeValue("src", "")));
                                item.SetValue(t, imgs);
                            }
                        }

                        item.SetValue(t, htmlstr?.Trim());
                    }
                    else
                    {
                        var text = doc.DocumentNode.SelectSingleNode(att.Xpath)?.InnerText;
                        if (att.Replace != null && att.Replace.Length == 2)
                        {
                            text = text.Replace(att.Replace[0], att.Replace[1]);
                        }
                        text = text?.Trim();
                        item.SetValue(t, text);
                    }
                }
                else
                {
                }
            }
            return t;
        }

        public IEnumerable<T> ParseToList<T>(string html) where T : class, new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties();
            if (properties == null)
            {
                return null;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            //todo 没有实现

            return list;
        }

        public IEnumerable<string> ParseUrlByRex(string html, string rexstr)
        {
            var list = Regex.Matches(html, rexstr)?.Select(a => a.Value);
            return list;
        }
    }
}
