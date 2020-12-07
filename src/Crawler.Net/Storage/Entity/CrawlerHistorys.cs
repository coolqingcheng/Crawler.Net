using FreeSql.DataAnnotations;
using System;

namespace Crawler.Entity
{
    [Index("url唯一","Url",true)]
    public class CrawlerHistorys
    {

        [Column(IsIdentity = true,IsPrimary = true)]
        public int Id { get; set; }

        [Column(StringLength = 100)]
        public string Url { get; set; }


        public DateTime CreateTime { get; set; }
    }
}
