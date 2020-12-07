using Crawler.Net.Storage.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Net.Storage
{
    /// <summary>
    /// 任务接口
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// 检查当前的URL是否已经采集过
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<bool> CheckExistAsync(string url);

        /// <summary>
        /// 保存当前的URL到本地数据库
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task SaveUrlToDbAsync(string url);

        /// <summary>
        /// 保存错误链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task SaveErrorAsync(string url, string reqName);

        /// <summary>
        /// 暂时保存任务链接到数据库
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        Task InsertTaskAsync(TaskEntity task);

        /// <summary>
        /// 从任务库里面抓取多少条任务，并删除
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<TaskEntity>> TakeTaskAsync(int count);
    }
}
