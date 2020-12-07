﻿using Crawler.Entity;
using Crawler.Net.Storage;
using Crawler.Net.Storage.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler.Storage
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
                await freeSql.Insert(new CrawlerTasks() { CreateTime = DateTime.Now, Url = task.Url, ReqName = task.TagName }).ExecuteInsertedAsync();
            }
        }

        public async Task SaveErrorAsync(string url, string reqName)
        {
            await freeSql.Insert(new CrawlerErroros()
            {
                Url = url,
                ReqNqme = reqName
            }).ExecuteInsertedAsync();
        }

        public async Task SaveUrlToDbAsync(string url)
        {
            await freeSql.Insert<CrawlerHistorys>(new CrawlerHistorys()
            {
                Url = url,
                CreateTime = DateTime.Now

            }).ExecuteInsertedAsync();
        }

        public async Task<IEnumerable<TaskEntity>> TakeTaskAsync(int count)
        {
            var tasks = await freeSql.Select<CrawlerTasks>().Take(count).ToListAsync();
            var ids = tasks.Select(a => a.Id).ToArray();
            await freeSql.Delete<CrawlerTasks>().Where(a => ids.Contains(a.Id)).ExecuteDeletedAsync();
            return tasks.Select(a => new TaskEntity()
            {
                TagName = a.ReqName,
                Url = a.Url
            });
        }
    }
}
