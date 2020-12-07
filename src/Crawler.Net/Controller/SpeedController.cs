using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Crawler.Net.Controller
{
    /// <summary>
    /// 爬虫速度控制器
    /// </summary>
    public class SpeedController
    {
        /// <summary>
        /// 最大的值
        /// </summary>
        private int _maxValue = 1;

        /// <summary>
        /// 当前值
        /// </summary>
        private int _currentValue = 0;

        public SpeedController()
        {
        }

        /// <summary>
        /// 最大的阈值
        /// </summary>
        /// <param name="value"></param>
        public SpeedController(int value)
        {
            _maxValue = value;
        }

        /// <summary>
        /// 设置速度
        /// </summary>
        /// <param name="value"></param>
        public void SetMaxSpeed(int value)
        {
            _maxValue = value;
        }

        /// <summary>
        /// 加一个值
        /// </summary>
        /// <returns></returns>
        public bool Add()
        {
            if (_maxValue > _currentValue)
            {
                Interlocked.Increment(ref _currentValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查任务状态是否为空
        /// </summary>
        /// <returns></returns>
        public bool CheckEmpty()
        {
            if (_currentValue == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前控制器状态，看看是否达到了满载状态
        /// </summary>
        /// <returns>true是 false 不是</returns>
        public bool GetFullLoadStatus()
        {
            return _currentValue >= _maxValue;
        }

        /// <summary>
        /// 减少一个值
        /// </summary>
        /// <returns></returns>
        public bool Sub()
        {
            if (_currentValue > 0)
            {
                Interlocked.Decrement(ref _currentValue);
                return true;
            }
            return false;
        }
    }
}
