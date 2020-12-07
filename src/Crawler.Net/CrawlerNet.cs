using AngleSharp.Css.Dom;
using Crawler.Net.Controller;
using Crawler.Net.DownLoader;
using Crawler.Net.Extensions;
using Crawler.Net.Request;
using Crawler.Net.Response;
using Crawler.Net.Segmenter;
using Crawler.Net.Selector;
using Crawler.Net.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Net
{
    public class CrawlerNet
    {
        /// <summary>
        /// 允许域名
        /// </summary>
        public string AllowHost { get; set; } = "*";

        /// <summary>
        /// 日志组件
        /// </summary>
        protected ILogger _logger;

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public IConfiguration GetConfig()
        {
            return _config;
        }

        private ServiceCollection services;
        private ServiceProvider serviceProvider;
        protected IConfiguration _config;

        ///// <summary>
        ///// 所有的请求集合
        ///// </summary>
        //private ConcurrentQueue<SpiderRequest> _requests = new ConcurrentQueue<SpiderRequest>();
        /// <summary>
        /// 请求队列
        /// </summary>
        private ConcurrentQueue<SpiderRequest> _requestQueue = new ConcurrentQueue<SpiderRequest>();

        /// <summary>
        /// 请求队列速度控制器
        /// </summary>
        private SpeedController _requestQueueSpeedController = new SpeedController();

        /// <summary>
        /// 响应数据队列
        /// </summary>
        private ConcurrentQueue<SpiderResponse> _responseQueue = new ConcurrentQueue<SpiderResponse>();

        /// <summary>
        /// 响应队列速度控制器
        /// </summary>
        private SpeedController _responseQueueSpeedController = new SpeedController(12);

        /// <summary>
        /// 注入服务列表
        /// </summary>
        private List<Action<IServiceCollection>> _serviceList = new List<Action<IServiceCollection>>();

        /// <summary>
        /// 爬虫速度，默认队列最大值为10
        /// </summary>
        private int _speed { get; set; } = 10;

        /// <summary>
        /// 任务频率，每次循环加入队列前暂停的毫秒数
        /// </summary>
        ///
        private int _taskHz { get; set; } = 300;

        /// <summary>
        /// 爬虫状态
        /// </summary>
        private SpiderStatus status = SpiderStatus.Paused;

        /// <summary>
        /// 添加请求到
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task AddRequest(SpiderRequest request)
        {
            //检查是否是允许的域名
            try
            {
                if (AllowHost != "*" && request.Url.Contains(AllowHost) == false)
                {
                    _logger.LogError($"URL:{request.Url} 不在允许的域名内！");
                    return;
                }
                using var scope = serviceProvider.CreateScope();
                var task = scope.ServiceProvider.GetRequiredService<ITask>();
                if (await task.CheckExistAsync(request.Url) == false)
                {
                    //验证是否已经采集过了
                    _logger.LogInformation("添加URL:" + request.Url + $"[{request.Url}]");
                    //todo 验证是否已经存在任务库中，如果存在，那么不添加
                    if (_requestQueue.Where(a => a.Url == request.Url).Any() == false)
                    {
                        //判断队列还有多少数据，如果超过1000，那么把任务存到数据库
                        if (_requestQueue.Count() > 100)
                        {
                            await task.InsertTaskAsync(new Storage.Entity.TaskEntity()
                            {
                                TagName = request.ReqName,
                                Url = request.Url
                            });
                        }
                        else
                        {
                            _requestQueue.Enqueue(request);
                        }
                    }
                }
                else
                {
                    //_logger.LogInformation($"当前URL:{request.Url}已经采集了，不加入队列");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加URL到请求队列失败！");
            }
        }


        private Action<SpiderResponse> callback;

        public void SetCallBack(Action<SpiderResponse> action)
        {
            callback = action;
        }

        public CrawlerNet()
        {
            services = new ServiceCollection();
            services.AddScoped<ILoggerFactory, LoggerFactory>();
            services.AddScoped<IHtmlDownLoader, HtmlDownLoader>();
            services.AddScoped<IFileDownLoader, FileDownLoader>();
            services.AddScoped<IAjaxDownLoader, JsonAjaxDownLoader>();
            services.AddScoped<ISegmenter, JbSegmenter>();
            services.AddHttpClient("gzip").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
            });

            _config = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("crawler.json", optional: true, reloadOnChange: true)
         .Build();
            services.AddSingleton<IConfiguration>(_config);
        }

        /// <summary>
        /// 设置爬虫并发爬取速度
        /// </summary>
        /// <param name="speed"></param>
        private void SetSpeed()
        {
            var reqnum = _config.GetSection("spider:reqnum").Value.ToInt();
            var respnum = _config.GetSection("spider:respnum").Value.ToInt();
            var puase = _config.GetSection("spider:puase").Value.ToInt();
            _logger.LogInformation($"当前的速度.请求:{reqnum} 响应:{respnum}");
            this._requestQueueSpeedController.SetMaxSpeed(reqnum);
            this._responseQueueSpeedController.SetMaxSpeed(respnum);
            this._taskHz = puase;
        }

        public void AddServices(Action<IServiceCollection> action)
        {
            if (services != null)
            {
                _serviceList.Add(action);
            }
        }

        /// <summary>
        /// 运行
        /// </summary>
        public async Task Run(params string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _serviceList.ForEach(action =>
            {
                action(services);
            });

            services.AddSingleton(
                    new FreeSql.FreeSqlBuilder()
                    .UseConnectionString(FreeSql.DataType.MySql, _config.GetSection("mysql").Value)
                    .UseAutoSyncStructure(true).Build()
                    );
            services.AddScoped<ITask, DbStorage>();
            services.AddScoped<ICrawlerSelector, CrawlerSelector>();
            serviceProvider = services.BuildServiceProvider();
            status = SpiderStatus.Running;
            _logger = serviceProvider.GetRequiredService<ILogger<CrawlerNet>>();
            _logger.LogInformation("爬虫开始启动");
            this.SetSpeed();
            await StartSpider();
            await WaitForExisting();
        }

        /// <summary>
        /// 等待爬虫结束
        /// </summary>
        /// <returns></returns>
        public async Task WaitForExisting()
        {
            while (status == SpiderStatus.Running)
            {
                await Task.Delay(10000);
                bool quit = true;
                //todo  检查 请求集合 请求队列，响应队列，是否都无操作，如果都无操作，那么退出当前的程序
                if (_requestQueue.Count != 0 || _responseQueue.Count != 0)
                {
                    quit = false;
                }
                if (_requestQueueSpeedController.CheckEmpty() != true || _responseQueueSpeedController.CheckEmpty() != true
                    )
                {
                    quit = false;
                }

                if (quit)
                {
                    _logger.LogInformation("检查状态完成，爬虫退出");
                    break;
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public virtual async Task Init()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 解析出数据
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual async Task ParseItem(SpiderResponse response)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 开始爬虫
        /// </summary>
        /// <returns></returns>
        public async Task StartSpider()
        {
            //调用初始化
            await Init();

            //启动任务调度
            await RunScheduler();
        }

        //ConcurrentDictionary<string, string> testdic = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 启动任务调度
        /// </summary>
        /// <returns></returns>
        public async Task RunScheduler()
        {
            _logger.LogInformation("任务调度器开始运行...");
            //取出任务列表加到下载队列中

            //从下载队列遍历数据，然后开始请求数据
            _ = Task.Factory.StartNew(async () =>
            {
                using var reqscope = serviceProvider.CreateScope();
                while (status == SpiderStatus.Running)
                {
                    try
                    {
                        if (_requestQueueSpeedController.GetFullLoadStatus())
                        {
                            _logger.LogInformation($"下载任务已经达到了上限，稍后再试！先暂停:{_taskHz} 队列中一共:{_requestQueue.Count()}条数据");
                            await Task.Delay(_taskHz);
                            continue;
                        }
                        if (_requestQueue.TryDequeue(out var req))
                        {
                            _logger.LogInformation($"当前队列还剩下:{_requestQueue.Count()}");
                            SpiderRequest request = req;
                            _ = Task.Factory.StartNew(async () =>
                            {
                                using var scope = serviceProvider.CreateScope();
                                try
                                {
                                    _logger.LogInformation($"下载html:{request.Url}");
                                    //检查是否下载任务已经满了

                                    //检查下载任务已经下载过了
                                    var task = scope.ServiceProvider.GetRequiredService<ITask>();
                                    if (await task.CheckExistAsync(req.Url))
                                    {
                                        _logger.LogInformation("当前URL已经下载过了，不进行再次下载");
                                        return;
                                    }
                                    if (_requestQueueSpeedController.GetFullLoadStatus())
                                    {
                                        return;
                                    }
                                    _requestQueueSpeedController.Add();

                                    //todo  检查是否已经采集过了
                                    var html = string.Empty;
                                    if (request.IsAjax)
                                    {
                                        var ajax = scope.ServiceProvider.GetRequiredService<IAjaxDownLoader>();
                                        html = await ajax.DownAsync(request.Url, request.AjaxParamData);
                                    }
                                    else
                                    {
                                        var htmlloader = scope.ServiceProvider.GetRequiredService<IHtmlDownLoader>();
                                        html = await htmlloader.DownAsync(request.Url);
                                    }

                                    var response = new SpiderResponse
                                    {
                                        Request = request,
                                        Referer = request.Url,
                                        Content = html,
                                        CharSet = "utf-8"
                                    };
                                    _responseQueue.Enqueue(response);

                                    _requestQueueSpeedController.Sub();
                                    //下载完成后，
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "下载文件队列异常！");
                                    //抛出异常后，重新添加到请求列表
                                    _requestQueueSpeedController.Sub();
                                    if (request.ReTryTimes <= 3)
                                    {
                                        request.ReTryTimes += 1;

                                        _requestQueue.Enqueue(request);
                                    }
                                    else
                                    {
                                        ITask db = scope.ServiceProvider.GetRequiredService<ITask>();
                                        if (request.IfSave)
                                        {
                                            await db.SaveErrorAsync(request.Url, request.ReqName);
                                        }
                                    }
                                }
                            });
                        }
                        else
                        {
                            //如果请求队列中没有数据了，那么从本地数据库抓取数据
                            _logger.LogInformation("请求队列中么有数据了，从本地数据库抓取");
                            var task = reqscope.ServiceProvider.GetRequiredService<ITask>();
                            var list = await task.TakeTaskAsync(100);
                            if (list != null && list.Count() > 0)
                            {
                                foreach (var item in list)
                                {
                                    _requestQueue.Enqueue(new SpiderRequest()
                                    {
                                        Url = item.Url,
                                        ReqName = item.TagName
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "没有拦截到的错误【429】");
                    }
                    await Task.Delay(_taskHz);
                }
            });
            //从响应队列中读数据包，进入解析操作
            _ = Task.Factory.StartNew(async () =>
            {
                while (status == SpiderStatus.Running)
                {
                    try
                    {
                        if (_responseQueueSpeedController.GetFullLoadStatus())
                        {
                            _logger.LogInformation($"当前速度已经满载！response队列剩余:{_responseQueue.Count()} 下载队列剩余:{_requestQueue.Count()}");
                            await Task.Delay(_taskHz);
                            continue;
                        }
                        using var scope = serviceProvider.CreateScope();
                        var log = scope.ServiceProvider.GetRequiredService<ILogger<CrawlerNet>>();
                        if (_responseQueue.TryDequeue(out var response))
                        {
                            _ = Task.Factory.StartNew(async () =>
                            {
                                using var scope = serviceProvider.CreateScope();
                                try
                                {
                                    ITask db = scope.ServiceProvider.GetRequiredService<ITask>();
                                    try
                                    {
                                        _responseQueueSpeedController.Add();
                                        response.serviceProvider = scope.ServiceProvider;
                                        //await ParseItem(response);
                                        if (callback != null)
                                        {
                                            callback(response);
                                        }
                                        _responseQueueSpeedController.Sub();
                                        //把当前URL存储本地
                                        //保存数据成功后，把当前URL存入本机数据库
                                        if (response.Request.IfSave)
                                        {
                                            await db.SaveUrlToDbAsync(response.Request.Url);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, $"解析或保存数据错误:{response.Request.Url}");
                                        _responseQueueSpeedController.Sub();
                                        if (response.Request.IfSave)
                                        {
                                            await db.SaveErrorAsync(response.Request.Url, response.Request.ReqName);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    scope.ServiceProvider.GetService<ILogger>().LogError(ex, "写入到ParseItem错误！");
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "没有拦截到的错误【490】");
                    }
                    await Task.Delay(_taskHz);
                }
            });
            await Task.CompletedTask;
        }
    }

    public enum SpiderStatus
    {
        /// <summary>
        /// 正在运行
        /// </summary>
        Running = 1,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused = 2,

        /// <summary>
        /// 退出中
        /// </summary>
        Exiting = 4,

        /// <summary>
        /// 退出完成
        /// </summary>
        Exited = 8
    }
}
