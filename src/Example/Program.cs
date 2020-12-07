using Crawler.Entity;
using Crawler.Net;
using Crawler.Net.Request;
using Crawler.Net.Storage;
using Crawler.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CrawlerNet crawler = new CrawlerNet();
            crawler.AddServices(service =>
            {
               
            });
            crawler.SetCallBack((resp) =>
            {
                Console.WriteLine(resp.Content);
            });
            await crawler.Run(args);
            await crawler.AddRequest(new SpiderRequest()
            {
                Url = "https://www.cnblogs.com"
            });

            Console.ReadLine();


        }
    }
}
