using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
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
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(html);
            //移除注释

            var comments = doc.Descendents<IComment>();
            if (comments != null)
            {
                var c = comments.Select(a => a.TextContent).ToList();
                foreach (var item in comments)
                {
                    item.Remove();
                }
            }
            //移除script
            var scripts = doc.QuerySelectorAll("script");
            if (scripts != null)
            {
                foreach (var item in scripts)
                {
                    item.Remove();
                }
            }
            //移除iframe
            var iframes = doc.QuerySelectorAll("iframe");
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
                if (att != null && att.parseType == ParseType.CssQuery)
                {
                    if (att.selectType == SelectType.HTML || att.GetAllImage)
                    {
                        var contentNode = doc.QuerySelector(att.Query);

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
                                var tags = contentNode.QuerySelectorAll(temp);
                                foreach (var tag in tags)
                                {
                                    var outer = tag.OuterHtml;
                                    var inner = tag.InnerHtml;
                                    htmlstr = htmlstr.Replace(outer, inner);
                                }
                            }
                        }

                        item.SetValue(t, htmlstr?.Trim());
                    }
                    else
                    {
                        var text = doc.QuerySelector(att.Query)?.TextContent;
                        if (att.Replace != null && att.Replace.Length == 2)
                        {
                            text = text.Replace(att.Replace[0], att.Replace[1]);
                        }
                        text = text?.Trim();
                        item.SetValue(t, text);
                    }
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
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(html);
            var querys = properties.Select(a => new
            {
                a.Name,
                att = a.GetCustomAttribute<SelectorAttribute>()
            });

            var table = new Dictionary<string, List<string>>();

            foreach (var item in querys)
            {

                var propertyInfo = typeof(T).GetType().GetProperty(item.Name);
                var tmps = doc.QuerySelectorAll(item.att.Query);
                var att = item.att;
                var tableCol = new List<string>();
                foreach (var tmp in tmps)
                {
                    if (item.att.selectType == SelectType.HTML)
                    {

                        var htmlstr = tmp.InnerHtml;
                        if (att.Replace != null && att.Replace.Length == 2)
                        {
                            htmlstr = htmlstr.Replace(att.Replace[0], att.Replace[1]);
                        }

                        // todo 替换某些标签为文字
                        if (att.ReplaceTagToTxt != null)
                        {
                            foreach (var temp in att?.ReplaceTagToTxt)
                            {
                                var tags = tmp.QuerySelectorAll(temp);
                                foreach (var tag in tags)
                                {
                                    var outer = tag.OuterHtml;
                                    var inner = tag.InnerHtml;
                                    htmlstr = htmlstr.Replace(outer, inner);
                                }
                            }
                        }
                        tableCol.Add(tmp.InnerHtml);
                    }
                    else if (item.att.selectType == SelectType.TEXT)
                    {
                        tableCol.Add(tmp.TextContent);
                    }
                    else if (item.att.selectType == SelectType.Attribute)
                    {
                        tableCol.Add(tmp.GetAttribute(item.att.Att));
                    }
                }

                table.TryAdd(item.Name, tableCol);
            }

            //获取最长的条数
            int size = table.Select(a => a.Value.Count()).Max();
            for (int i = 0; i < size; i++)
            {
                var t = new T();
                foreach (var property in properties)
                {

                    var col = table[property.Name];
                    if (col.Count() >= i)
                    {
                        t.GetType().GetProperty(property.Name).SetValue(t, col[i]);
                    }
                }
                list.Add(t);
            }



            return list;
        }

        public IEnumerable<string> ParseUrlByRex(string html, string rexstr)
        {
            var list = Regex.Matches(html, rexstr)?.Select(a => a.Value);
            return list;
        }
    }

}
