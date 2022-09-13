using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace IThink.Sqlsugar
{
    public static class SqlSugarExtension
    {
        /// <summary>
        /// 添加sqlSugar支持多数据库
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugar(this IServiceCollection services, SqlSugarConnectOption option, RedisConfig redisConfig = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            if (option != null)
            {
                if (option.DbType == DbType.PostgreSQL)
                {
                    option.MoreSettings = new ConnMoreSettings { PgSqlIsAutoToLower = option.SqlIsAutoToLower };
                }

                option.InitKeyType = InitKeyType.Attribute;

                if (option.CacheModel == CacheModel.Redis)
                {
                    if (redisConfig == null)
                    {
                        throw new ArgumentNullException(typeof(RedisConfig).Name);
                    }

                    var instance = new RedisCache(redisConfig);
                    option.ConfigureExternalServices = new ConfigureExternalServices
                    {
                        DataInfoCacheService = instance
                    };
                }

                if (contextLifetime == ServiceLifetime.Scoped)
                {
                    services.AddScoped(s => new BaseSqlSugarClient(option));
                }
                if (contextLifetime == ServiceLifetime.Singleton)
                {
                    services.AddSingleton(s => new BaseSqlSugarClient(option));
                }
                if (contextLifetime == ServiceLifetime.Transient)
                {
                    services.AddTransient(s => new BaseSqlSugarClient(option));
                }
            }

            services.AddRepository(contextLifetime);

            return services;
        }

        /// <summary>
        /// 添加sqlSugar支持多数据库
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugar(this IServiceCollection services, RedisConfig configuration, List<SqlSugarConnectOption> connectOptions, ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            if (connectOptions != null)
            {
                foreach (var option in connectOptions)
                {
                    if (option != null)
                    {
                        if (option.DbType == DbType.PostgreSQL)
                        {
                            option.MoreSettings = new ConnMoreSettings { PgSqlIsAutoToLower = option.SqlIsAutoToLower };
                        }

                        option.InitKeyType = InitKeyType.Attribute;

                        if (option.CacheModel == CacheModel.Redis)
                        {
                            var instance = new RedisCache(configuration);
                            option.ConfigureExternalServices = new ConfigureExternalServices
                            {
                                DataInfoCacheService = instance
                            };
                        }

                        if (contextLifetime == ServiceLifetime.Scoped)
                        {
                            services.AddScoped(s => new BaseSqlSugarClient(option));
                        }
                        if (contextLifetime == ServiceLifetime.Singleton)
                        {
                            services.AddSingleton(s => new BaseSqlSugarClient(option));
                        }
                        if (contextLifetime == ServiceLifetime.Transient)
                        {
                            services.AddTransient(s => new BaseSqlSugarClient(option));
                        }
                    }
                }

                services.AddRepository(contextLifetime);
            }

            return services;
        }

        /// <summary>
        /// 添加sqlSugar支持多数据库
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, string section = "DbConfig")
        {
            var connectOptions = configuration.GetSection(section).Get<List<SqlSugarConnectOption>>();
            if (connectOptions != null)
            {
                foreach (var option in connectOptions)
                {
                    if (option != null)
                    {
                        if (option.DbType == DbType.PostgreSQL)
                        {
                            option.MoreSettings = new ConnMoreSettings { PgSqlIsAutoToLower = option.SqlIsAutoToLower };
                        }

                        option.InitKeyType = InitKeyType.Attribute;

                        if (option.CacheModel == CacheModel.Redis)
                        {
                            var instance = new RedisCache(configuration);
                            option.ConfigureExternalServices = new ConfigureExternalServices
                            {
                                DataInfoCacheService = instance
                            };
                        }

                        if (contextLifetime == ServiceLifetime.Scoped)
                        {
                            services.AddScoped(s => new BaseSqlSugarClient(option));
                        }
                        if (contextLifetime == ServiceLifetime.Singleton)
                        {
                            services.AddSingleton(s => new BaseSqlSugarClient(option));
                        }
                        if (contextLifetime == ServiceLifetime.Transient)
                        {
                            services.AddTransient(s => new BaseSqlSugarClient(option));
                        }
                    }
                }

                services.AddRepository(contextLifetime);
            }

            return services;
        }


        /// <summary>
        /// 添加sqlSugar支持多数据库
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private static IServiceCollection AddRepository(this IServiceCollection services, ServiceLifetime contextLifetime)
        {
            if (contextLifetime == ServiceLifetime.Scoped)
            {
                services.AddScoped<ISqlSugarRepository, SqlSugarRepository>();
            }
            if (contextLifetime == ServiceLifetime.Singleton)
            {
                services.AddSingleton<ISqlSugarRepository, SqlSugarRepository>();
            }
            if (contextLifetime == ServiceLifetime.Transient)
            {
                services.AddTransient<ISqlSugarRepository, SqlSugarRepository>();
            }

            return services;
        }
    }
}
