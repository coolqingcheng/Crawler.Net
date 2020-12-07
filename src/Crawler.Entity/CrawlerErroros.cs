using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Entity
{
    [Index("Url唯一", "Url", IsUnique = true)]
    public class CrawlerErroros
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string ErrorMsg { get; set; }

        [Column(StringLength = 50)]
        public string ReqNqme { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
