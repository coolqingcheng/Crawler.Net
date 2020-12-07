基于.NET Core开发的迷你爬虫库。

使用方式。

安装nuget包

```
dotnet add package Crawler.Net
```

简单使用

1. 添加crawler.json  配置好数据库，以及爬虫的基本配置。

框架内置了FreeSql。会自动创建数据库，只需要保证数据库连接正确即可.

```json
{
  "mysql": "Data Source=127.0.0.1;Port=3306;User ID=root;Password=root;Initial Catalog=SpiderDb;Charset=utf8;SslMode=none;Max pool size=10",
  "spider": {
    "reqnum": 5,
    "respnum": 5,
    "puase": 2000
  }
}


```

2. new一个CrawlerNet示例。

可在AddServices使用和asp.net一样的依赖注入。在SetCallBack处理http返回。

ICrawlerSelector封装了类似于ORM的数据简单网页匹配。在实体类上添加 Selector 特性即可。目前支持和jQuery一样的选择器。



```c#
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
```

``` c#
 public class CnBlogEntity
    {

        [Selector(Query = ".post-item-title", selectType = SelectType.TEXT)]
        public string Title { get; set; }


        [Selector(Query = ".post-item-summary", selectType = SelectType.HTML)]
        public string Content { get; set; }

        [Selector(Query = ".post-item-title", selectType = SelectType.Attribute,Att = "href")]
        public string href { get; set; }
    }
```

