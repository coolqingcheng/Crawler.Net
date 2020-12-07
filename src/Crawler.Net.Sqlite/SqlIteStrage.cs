using Crawler.Net.Storage;
using Crawler.Net.Storage.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Net.Sqlite
{
    public class SqlIteStrage : ITask
    {
        IFreeSql fsql;

        public SqlIteStrage(IFreeSql freeSql)
        {
            fsql = freeSql;
        }

        public Task<bool> CheckExistAsync(string url)
        {
            throw new NotImplementedException();
        }

        public async Task InitDb()
        {
            await Task.Delay(100);
        }

        public Task InsertTaskAsync(TaskEntity task)
        {
            throw new NotImplementedException();
        }

        public Task SaveErrorAsync(string url, string reqName)
        {
            throw new NotImplementedException();
        }

        public Task SaveUrlToDbAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskEntity>> TakeTaskAsync(int count)
        {
            throw new NotImplementedException();
        }
    }
}
