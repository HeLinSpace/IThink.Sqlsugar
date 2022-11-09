using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace IThink.Sqlsugar
{

    /// <summary>
    /// 数据库配置
    /// </summary>
    public class SqlSugarConnectOption : ConnectionConfig
    {
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 是否默认库
        /// </summary>
        public bool Default { set; get; } = true;

        /// <summary>
        /// SQL 自动转为小写
        /// </summary>
        public bool SqlIsAutoToLower { set; get; } = false;

        /// <summary>
        /// 缓存方式
        /// </summary>
        public CacheModel CacheModel { set; get; } = 0;
    }

    /// <summary>
    /// 持久化方式
    /// </summary>
    public enum CacheModel
    {
        /// <summary>
        /// 不使用缓存
        /// </summary>
        Off = 0,
        /// <summary>
        /// 内存
        /// </summary>
        Memory = 1,
        /// <summary>
        /// Redis
        /// </summary>
        Redis = 2
    }
}

