using JiebaNet.Segmenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Net.Segmenter
{
    public interface ISegmenter
    {
        string[] Cut(string str);
    }

    public class JbSegmenter : ISegmenter
    {
        public string[] Cut(string str)
        {
            var jieba = new JiebaSegmenter();
            return jieba.Cut(str)?.Where(a => a.Length > 1 && a.Length <= 5)?.Distinct().ToArray();
        }
    }
}
