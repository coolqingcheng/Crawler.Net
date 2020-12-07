using Crawler.Entity;
using Crawler.Net.Storage.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Net.Storage
{
    public class DbStorage : ITask
    {
        readonly IFreeSql freeSql;

        public DbStorage(IFreeSql fsql)
        {
            freeSql = fsql;
        }

        public async Task<bool> CheckExistAsync(string url)
        {
            var any = await freeSql.Select<CrawlerHistorys>().Where(a => a.Url == url).AnyAsync();
            return any;
        }


        public async Task InsertTaskAsync(TaskEntity task)
        {
            var any = await freeSql.Select<CrawlerTasks>().Where(a => a.Url == task.Url && a.ReqName == task.TagName).AnyAsync();
            if (!any)
            {
                await freeSql.Insert(new CrawlerTasks() { CreateTime = DateTime.Now, Url = task.Url, ReqName = task.TagName }).ExecuteAffrowsAsync();
            }
        }

        public async Task SaveErrorAsync(string url, string reqName)
        {
            await freeSql.Insert(new CrawlerErroros()
            {
                Url = url,
                ReqNqme = reqName
            }).ExecuteAffrowsAsync();
        }

        public async Task SaveUrlToDbAsync(string url)
        {
            await freeSql.Insert(new CrawlerHistorys()
            {
                Url = url,
                CreateTime = DateTime.Now

            }).ExecuteAffrowsAsync();
        }

        public async Task<IEnumerable<TaskEntity>> TakeTaskAsync(int count)
        {
            var tasks = await freeSql.Select<CrawlerTasks>().Take(count).ToListAsync();
            var ids = tasks.Select(a => a.Id).ToArray();
            if (ids.Length > 0)
            {
                await freeSql.Delete<CrawlerTasks>().Where(a => ids.Contains(a.Id)).ExecuteDeletedAsync();
            }
            return tasks.Select(a => new TaskEntity()
            {
                TagName = a.ReqName,
                Url = a.Url
            });
        }
    }
}
