using Crawler.Net.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class CnBlogEntity
    {

        [Selector(Query = ".post-item-title", selectType = SelectType.TEXT)]
        public string Title { get; set; }


        [Selector(Query = ".post-item-summary", selectType = SelectType.HTML)]
        public string Content { get; set; }

        [Selector(Query = ".post-item-title", selectType = SelectType.Attribute,Att = "href")]
        public string href { get; set; }
    }
}
