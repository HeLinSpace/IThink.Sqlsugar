using System;
using SqlSugar;

namespace IThink.Sqlsugar;

public class BaseSqlSugarClient : SqlSugarClient
{
    public BaseSqlSugarClient(SqlSugarConnectOption config) : base(config) { DbName = config.Name; Default = config.Default; }

    public string DbName { set; get; }

    public bool Default { set; get; }

    /// <summary>
    /// 
    /// </summary>
    public bool UseCache { set; get; }
}
