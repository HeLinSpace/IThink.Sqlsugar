/* ---------------------------------------------------------------------    
 * 
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2022-9-13   mailhelin@qq.com
 *
 * ------------------------------------------------------------------------------*/

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IThink.Sqlsugar
{
    /// <summary>
    /// SqlSugarDbRepository
    /// </summary>
    public class SqlSugarRepository : ISqlSugarRepository
    {
        private readonly IEnumerable<BaseSqlSugarClient> _clients;

        /// <summary>
        /// 
        /// </summary>
        public bool UseCache { get; set; } = false;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="clients"></param>
        public SqlSugarRepository(IEnumerable<BaseSqlSugarClient> clients)
        {
            _clients = clients;
            DbContext = _clients.FirstOrDefault(it => it.Default);

            UseCache = this.DbContext.UseCache;

            DbContext.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" +
                    DbContext.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
        }

        /// <summary>
        /// 上下文对象
        /// </summary>
        public BaseSqlSugarClient DbContext { get; private set; }

        /// <summary>
        /// 返回指定Db实例
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public ISqlSugarRepository UseDb(string dbName)
        {
            DbContext = _clients.FirstOrDefault(it => it.DbName == dbName);
            return this;
        }

        /// <summary>
        /// 去除@
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IDictionary<string, object> ReSetParameters(IDictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count() == 0)
            {
                return parameters;
            }

            var result = parameters.ToDictionary(k => k.Key.Replace("@", ""), v => v.Value);
            return result;
        }

        /// <summary>
        /// 执行sql查询返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> SqlQueryList<T>(string sql, IDictionary<string, object> parameters = null)
        {
            return DbContext.Ado.SqlQuery<T>(sql, ReSetParameters(parameters));
        }

        /// <summary>
        /// 执行sql查询返回对象（支持dynamic）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T SqlQueryObject<T>(string sql, IDictionary<string, object> parameters = null)
        {
            return DbContext.Ado.SqlQuery<T>(sql, ReSetParameters(parameters)).FirstOrDefault();
        }

        /// <summary>
        /// 执行sql文,查询第一列第一条
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public dynamic GetScalarT<T>(string sql, IDictionary<string, object> parameters = null)
        {
            var typeName = typeof(T).Name;
            var setParams = ReSetParameters(parameters);
            if (typeName == typeof(string).Name)
            {
                return DbContext.Ado.GetString(sql, setParams);
            }
            if (typeName == typeof(int).Name)
            {
                return DbContext.Ado.GetInt(sql, setParams);
            }
            if (typeName == typeof(DateTime).Name)
            {
                return DbContext.Ado.GetDateTime(sql, setParams);
            }
            if (typeName == typeof(long).Name)
            {
                return DbContext.Ado.GetLong(sql, setParams);
            }
            if (typeName == typeof(decimal).Name)
            {
                return DbContext.Ado.GetDecimal(sql, setParams);
            }
            if (typeName == typeof(double).Name)
            {
                return DbContext.Ado.GetDouble(sql, setParams);
            }
            return DbContext.Ado.GetScalar(sql, setParams);
        }

        /// <summary>
        /// 执行sql文
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sql, IDictionary<string, object> parameters = null)
        {
            return DbContext.Ado.ExecuteCommand(sql, ReSetParameters(parameters));
        }

        /// <summary>
        /// 查询sql并返回字典
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDictionary<string, object> QuerySingleDictionary(string sql, IDictionary<string, object> parameters = null)
        {
            return QueryListDictionary(sql, ReSetParameters(parameters)).FirstOrDefault();
        }

        /// <summary>
        /// 查询sql并返回字典列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<IDictionary<string, object>> QueryListDictionary(string sql, IDictionary<string, object> parameters = null)
        {
            using (var reader = DbContext.Ado.GetDataReader(sql, ReSetParameters(parameters)))
            {
                List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();
                if (reader != null && !reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        result.Add(DataReaderToExpandoObject(reader));
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public List<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).WithCacheIF(UseCache).ToList();
        }

        /// <summary>
        /// 查询实体列表异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public Task<List<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).WithCacheIF(UseCache).ToListAsync();
        }

        public List<TResult> GetList<TEntity, TResult>(Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc, Expression<Func<TEntity, TResult>> selectExpression = null) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).WithCacheIF(UseCache).Select<TResult>(selectExpression).ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="page"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public (List<TEntity> list, int totalCount) GetPageList<TEntity>(PageQueryRequest page, Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new()
        {
            int count = 0;
            var result = DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).WithCacheIF(UseCache).ToPageList(page.PageNumber, page.PageSize, ref count);
            return (result, count);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="page"></param>
        /// <param name="selectExpression"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public (int, List<TResult>) GetPageList<TEntity, TResult>(PageQueryRequest page, Expression<Func<TEntity, TResult>> selectExpression, Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new() where TResult : class, new()
        {
            int count = 0;
            var result = DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).Select(selectExpression).WithCacheIF(UseCache).ToPageList(page.PageNumber, page.PageSize, ref count);
            return (count, result);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="page"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <param name="selectExpression"></param>
        /// <returns></returns>
        public (List<TResult> list, int totalCount) GetPageList<TEntity, TResult>(PageQueryRequest page, Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc, Expression<Func<TEntity, TResult>> selectExpression = null)
            where TEntity : BaseEntity, new()
            where TResult : class, new()
        {
            int count = 0;
            var result = DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).Select(selectExpression).ToPageList(page.PageNumber, page.PageSize, ref count);

            return (result, count);
        }

        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="page"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public async Task<(List<TEntity> list, int totalCount)> GetPageListAsync<TEntity>(PageQueryRequest page, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType) where TEntity : BaseEntity, new()
        {
            int count = 0;
            var result = await DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).WithCacheIF(UseCache).ToPageListAsync(page.PageNumber, page.PageSize, count);
            return (result, count);
        }

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetById<TEntity>(string id) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().WithCacheIF(UseCache).InSingle(id);
        }

        /// <summary>
        /// 查询第一条记录
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public TEntity GetFirst<TEntity>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).WithCacheIF(UseCache).First(whereExpression);
        }

        /// <summary>
        /// 查询第一条记录异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public async Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new()
        {
            return await DbContext.Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).FirstAsync(whereExpression);
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public int Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).Count();
        }

        /// <summary>
        /// CountAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).CountAsync();
        }

        /// <summary>
        /// Any
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public bool Any<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null) where TEntity : BaseEntity, new()
        {
            return DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).Any();
        }

        /// <summary>
        /// AnyAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null) where TEntity : BaseEntity, new()
        {
            return await DbContext.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).AnyAsync();
        }

        #region 删除

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public bool Delete<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Deleteable<TEntity>().Where(whereExpression).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Deleteable<TEntity>().Where(whereExpression).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 条件删除异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Deleteable<TEntity>().Where(whereExpression).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandAsync() > 0;
            }
        }

        /// <summary>
        /// 对象删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="deleteObj"></param>
        /// <returns></returns>
        public bool Delete<TEntity>(TEntity deleteObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Deleteable(deleteObj).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Deleteable(deleteObj).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 对象删除异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="deleteObj"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<TEntity>(TEntity deleteObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Deleteable(deleteObj).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Deleteable(deleteObj).ExecuteCommandAsync() > 0;
            }
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById<TEntity>(dynamic id) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Deleteable<TEntity>().In(id).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Deleteable<TEntity>().In(id).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdAsync<TEntity>(dynamic id) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Deleteable<TEntity>().In(id).RemoveDataCache().ExecuteCommandAsync<TEntity>() > 0;
            }
            else
            {
                return await DbContext.Deleteable<TEntity>().In(id).ExecuteCommandAsync<TEntity>() > 0;
            }
        }

        /// <summary>
        /// 根据主键列表删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteByIds<TEntity>(dynamic[] ids) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Deleteable<TEntity>().In(ids).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Deleteable<TEntity>().In(ids).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 根据主键列表删除异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteByIdsAsync<TEntity>(dynamic[] ids) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Deleteable<TEntity>().In(ids).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Deleteable<TEntity>().In(ids).ExecuteCommandAsync() > 0;
            }
        }

        #endregion

        #region 新增

        /// <summary>
        /// 对象新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        public bool Insert<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Insertable(insertObj).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Insertable(insertObj).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 对象新增异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Insertable(insertObj).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Insertable(insertObj).ExecuteCommandAsync() > 0;
            }
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        public bool InsertRange<TEntity>(List<TEntity> insertObjs) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Insertable(insertObjs).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Insertable(insertObjs).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        public bool InsertRange<TEntity>(TEntity[] insertObjs) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Insertable(insertObjs).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Insertable(insertObjs).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 批量新增异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        public async Task<bool> InsertRangeAsync<TEntity>(List<TEntity> insertObjs) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Insertable(insertObjs).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Insertable(insertObjs).ExecuteCommandAsync() > 0;
            }
        }

        /// <summary>
        /// 批量新增异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        public async Task<bool> InsertRangeAsync<TEntity>(TEntity[] insertObjs) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Insertable(insertObjs).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Insertable(insertObjs).ExecuteCommandAsync() > 0;
            }
        }

        /// <summary>
        /// 新增返回自增主键
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        public int InsertReturnIdentity<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Insertable(insertObj).RemoveDataCache().ExecuteReturnIdentity();
            }
            else
            {
                return DbContext.Insertable(insertObj).ExecuteReturnIdentity();
            }
        }

        /// <summary>
        /// 新增返回自增主键
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        public async Task<long> InsertReturnIdentityAsync<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Insertable(insertObj).RemoveDataCache().ExecuteReturnBigIdentityAsync();
            }
            else
            {
                return await DbContext.Insertable(insertObj).ExecuteReturnBigIdentityAsync();
            }
        }

        #endregion

        #region 更新

        /// <summary>
        /// 表达式更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="columns"></param>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public bool Update<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Updateable<TEntity>().SetColumns(columns).Where(whereExpression).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Updateable<TEntity>().SetColumns(columns).Where(whereExpression).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 表达式更新异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="columns"></param>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Updateable<TEntity>().SetColumns(columns).Where(whereExpression).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Updateable<TEntity>().SetColumns(columns).Where(whereExpression).ExecuteCommandAsync() > 0;
            }
        }

        /// <summary>
        /// 对象更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        public bool Update<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Updateable(updateObj).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Updateable(updateObj).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 对象更新异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Updateable(updateObj).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Updateable(updateObj).ExecuteCommandAsync() > 0;
            }
        }

        /// <summary>
        /// 对象更新忽略空值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        public bool UpdateIgnoreNull<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Updateable(updateObj).IgnoreColumns(ignoreAllNullColumns: true).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Updateable(updateObj).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 列表更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObjs"></param>
        /// <returns></returns>
        public bool UpdateRange<TEntity>(TEntity[] updateObjs) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Updateable(updateObjs).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Updateable(updateObjs).ExecuteCommand() > 0;
            }
        }

        /// <summary>
        /// 列表更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObjs"></param>
        /// <returns></returns>
        public bool UpdateRange<TEntity>(List<TEntity> updateObjs) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return DbContext.Updateable(updateObjs).RemoveDataCache().ExecuteCommand() > 0;
            }
            else
            {
                return DbContext.Updateable(updateObjs).ExecuteCommand() > 0;
            }
        }


        /// <summary>
        /// 列表更新异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObjs"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRangeAsync<TEntity>(TEntity[] updateObjs) where TEntity : BaseEntity, new()
        {
            if (UseCache)
            {
                return await DbContext.Updateable(updateObjs).RemoveDataCache().ExecuteCommandAsync() > 0;
            }
            else
            {
                return await DbContext.Updateable(updateObjs).ExecuteCommandAsync() > 0;
            }
        }

        #endregion

        /// <summary>
        /// 包装事务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Tran(Action action)
        {
            try
            {
                BeginTran();
                action();
                CommitTran();
            }
            catch (Exception ex)
            {
                RollbackTran();
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// 事务开始
        /// </summary>
        public void BeginTran()
        {
            DbContext.BeginTran();
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void CommitTran()
        {
            DbContext.CommitTran();
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollbackTran()
        {
            DbContext.RollbackTran();
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool DropTable(string tableName)
        {
            try
            {
                return DbContext.DbMaintenance.DropTable(tableName);
            }
            catch
            {
                return true;
            }
        }

        #region 重写基础方法

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, JoinQueryInfos>> joinExpression)
            where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
            where T11 : BaseEntity, new()
            where T12 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
            where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
            where T11 : BaseEntity, new()
            where T12 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public ISugarQueryable<ExpandoObject> Queryable(string tableName, string shortName)
        {
            return DbContext.Queryable(tableName, shortName).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, JoinQueryInfos>> joinExpression)
                    where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
            where T11 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
            where T11 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object[]>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
            where T11 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object[]>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
            where T11 : BaseEntity, new()
            where T12 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Queryable<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Queryable<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Queryable<T>(string shortName) where T : BaseEntity, new()
        {
            return DbContext.Queryable<T>(shortName).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, JoinQueryInfos>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable) where T : BaseEntity, new()
        {
            return DbContext.Queryable(queryable).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ISugarQueryable<T> Queryable<T>() where T : BaseEntity, new()
        {
            return DbContext.Queryable<T>().WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinQueryable1"></param>
        /// <param name="joinQueryable2"></param>
        /// <param name="joinQueryable3"></param>
        /// <param name="joinType1"></param>
        /// <param name="joinExpression1"></param>
        /// <param name="joinType2"></param>
        /// <param name="joinExpression2"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3, JoinType joinType1, Expression<Func<T, T2, T3, bool>> joinExpression1, JoinType joinType2, Expression<Func<T, T2, T3, bool>> joinExpression2)
            where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
        {
            return DbContext.Queryable(joinQueryable1, joinQueryable2, joinQueryable3, joinType1, joinExpression1, joinType2, joinExpression2).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinQueryable1"></param>
        /// <param name="joinQueryable2"></param>
        /// <param name="joinType"></param>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, JoinType joinType, Expression<Func<T, T2, bool>> joinExpression)
            where T : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            return DbContext.Queryable(joinQueryable1, joinQueryable2, joinType, joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinQueryable1"></param>
        /// <param name="joinQueryable2"></param>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, Expression<Func<T, T2, bool>> joinExpression)
                where T : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            return DbContext.Queryable(joinQueryable1, joinQueryable2, joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, JoinQueryInfos>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, JoinQueryInfos>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, JoinQueryInfos>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, JoinQueryInfos>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
        {
            return DbContext.Queryable(joinExpression).WithCacheIF(UseCache);
        }

        #endregion

        #region private

        /// <summary>
        /// DataReader to Dynamic
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private IDictionary<string, object> DataReaderToExpandoObject(IDataReader reader)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    var addItem = reader.GetValue(i);
                    if (addItem == DBNull.Value)
                        addItem = null;
                    dic.Add(reader.GetName(i), addItem);
                }
                catch
                {
                    dic.Add(reader.GetName(i), null);
                }
            }
            return dic;
        }

        #endregion
    }
}
