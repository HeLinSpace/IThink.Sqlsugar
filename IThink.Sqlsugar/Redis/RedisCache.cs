using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IThink.Sqlsugar;

/// <summary>
/// 
/// </summary>
public class RedisCache : ICacheService
{
    private static RedisConfig _config = null;
    private static RedisHandle _handleInstance = null;
    private static readonly object _instanceLock = new object();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public RedisCache(RedisConfig config)
    {
        if (_handleInstance == null)
            lock (_instanceLock)
                if (_handleInstance == null)
                {
                    _handleInstance = new RedisHandle(config);
                }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public RedisCache(IConfiguration configuration, string section = "RedisConfig")
    {
        if (_handleInstance == null)
            lock (_instanceLock)
                if (_handleInstance == null)
                {
                    _config = configuration.GetSection(section).Get<RedisConfig>();
                    _handleInstance = new RedisHandle(_config);
                }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cacheDurationInSeconds"></param>
    public void Add<V>(string key, V value, int cacheDurationInSeconds = int.MaxValue)
    {
        _handleInstance.Set<V>(_config.Prefix + key, value, cacheDurationInSeconds);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add<V>(string key, V value)
    {
        Add(_config.Prefix + key, value, int.MaxValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey<V>(string key)
    {
        return _handleInstance.ContainsKey(_config.Prefix + key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public V Get<V>(string key)
    {
        return _handleInstance.Get<V>(_config.Prefix + key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public IEnumerable<string> GetAllKey<V>()
    {
        var keys = _handleInstance.GetAllKeys($"{_config.Prefix}SqlSugarDataCache.*");

        return keys.Select(s => s.ToString().Remove(0, _config.Prefix?.Length ?? 0)).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="create"></param>
    /// <param name="cacheDurationInSeconds"></param>
    /// <returns></returns>
    public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        if (ContainsKey<V>(cacheKey))
        {
            return Get<V>(cacheKey);
        }
        else
        {
            var result = create();
            Add(cacheKey, result, cacheDurationInSeconds);
            return result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="key"></param>
    public void Remove<V>(string key)
    {
        _handleInstance.Remove(_config.Prefix + key);
    }
}