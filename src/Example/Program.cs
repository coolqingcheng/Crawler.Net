using Crawler.Entity;
using Crawler.Net;
using Crawler.Net.Request;
using Crawler.Net.Selector;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            CrawlerNet crawler = new CrawlerNet();
            crawler.AddServices(service =>
            {

            });
            crawler.SetCallBack(async (resp) =>
            {
                var selector = resp.serviceProvider.GetRequiredService<ICrawlerSelector>();
                var entity = selector.ParseToList<CnBlogEntity>(resp.Content);

                await Task.FromResult(true);

            });
            await crawler.Run(args);
            await crawler.AddRequest(new SpiderRequest()
            {
                Url = "https://www.cnblogs.com/",
                ReqName = "获取主页"
            });

            Console.ReadLine();


        }
    }
}
