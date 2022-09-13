using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace IThink.Sqlsugar;
/// <summary>
/// 
/// </summary>
public class RedisConfig
{
    /// <summary>
    /// Connection string
    /// </summary>
    public string[] Connection { get; set; }

    /// <summary>
    /// Connection string for readonly
    /// </summary>
    public string[] ConnectionReadOnly { get; set; }

    /// <summary>
    /// DefaultDatabase
    /// </summary>
    public int? DefaultDatabase { get; set; }

    /// <summary>
    /// 前缀
    /// </summary>
    public string Prefix { get; set; }
}
