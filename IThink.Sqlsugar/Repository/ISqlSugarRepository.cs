using SqlSugar;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IThink.Sqlsugar
{


    /// <summary>
    /// 
    /// </summary>
    public interface ISqlSugarRepository
    {
        /// <summary>
        /// 上下文对象
        /// </summary>
        BaseSqlSugarClient DbContext { get; }

        /// <summary>
        /// 返回指定Db实例
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        ISqlSugarRepository UseDb(string dbName);

        /// <summary>
        /// 执行sql查询返回列表（支持dynamic）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<TEntity> SqlQueryList<TEntity>(string sql, IDictionary<string, object> parameters = null);

        /// <summary>
        /// 执行sql查询返回对象（支持dynamic）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        T SqlQueryObject<T>(string sql, IDictionary<string, object> parameters = null);

        /// <summary>
        /// 执行sql文,查询第一列第一条
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        dynamic GetScalarT<T>(string sql, IDictionary<string, object> parameters = null);

        /// <summary>
        /// 执行sql文
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteCommand(string sql, IDictionary<string, object> parameters = null);

        /// <summary>
        /// 查询sql并返回字典
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IDictionary<string, object> QuerySingleDictionary(string sql, IDictionary<string, object> parameters = null);

        /// <summary>
        /// 查询sql并返回字典列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<IDictionary<string, object>> QueryListDictionary(string sql, IDictionary<string, object> parameters = null);

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        List<TEntity> GetList<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new();
        List<TResult> GetList<TEntity, TResult>(Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc, Expression<Func<TEntity, TResult>> selectExpression = null) where TEntity : BaseEntity, new();

        /// <summary>
        /// 查询实体列表异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="page"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        (List<TEntity> list, int totalCount) GetPageList<TEntity>(PageQueryRequest page, Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="page"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        (List<TResult> list, int totalCount) GetPageList<TEntity, TResult>(PageQueryRequest page, Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc, Expression<Func<TEntity, TResult>> selectExpression = null) where TEntity : BaseEntity, new() where TResult : class, new();

        /// <summary>
        /// 分页查询异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="page"></param>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        Task<(List<TEntity> list, int totalCount)> GetPageListAsync<TEntity>(PageQueryRequest page, Expression<Func<TEntity, bool>> whereExpression = null, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new();

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById<TEntity>(string id) where TEntity : BaseEntity, new();

        /// <summary>
        /// 查询第一条记录
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        TEntity GetFirst<TEntity>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new();

        /// <summary>
        /// 查询第一条记录异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where TEntity : BaseEntity, new();

        /// <summary>
        /// count
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        int Count<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null) where TEntity : BaseEntity, new();

        /// <summary>
        /// CountAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new();

        /// <summary>
        /// Any
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        bool Any<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null) where TEntity : BaseEntity, new();

        /// <summary>
        /// AnyAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new();

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        bool Delete<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new();

        /// <summary>
        /// 条件删除异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="deleteObj"></param>
        /// <returns></returns>
        bool Delete<TEntity>(TEntity deleteObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象删除异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="deleteObj"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync<TEntity>(TEntity deleteObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteById<TEntity>(dynamic id) where TEntity : BaseEntity, new();

        /// <summary>
        /// 根据主键删除异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteByIdAsync<TEntity>(dynamic id) where TEntity : BaseEntity, new();

        /// <summary>
        /// 根据主键列表删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool DeleteByIds<TEntity>(dynamic[] ids) where TEntity : BaseEntity, new();

        /// <summary>
        /// 根据主键列表删除异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteByIdsAsync<TEntity>(dynamic[] ids) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        bool Insert<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象新增异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        Task<bool> InsertAsync<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        bool InsertRange<TEntity>(List<TEntity> insertObjs) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量新增异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        Task<bool> InsertRangeAsync<TEntity>(List<TEntity> insertObjs) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        bool InsertRange<TEntity>(TEntity[] insertObjs) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量新增异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObjs"></param>
        /// <returns></returns>
        Task<bool> InsertRangeAsync<TEntity>(TEntity[] insertObjs) where TEntity : BaseEntity, new();

        /// <summary>
        /// 新增返回自增主键
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        int InsertReturnIdentity<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 新增返回自增主键
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="insertObj"></param>
        /// <returns></returns>
        Task<long> InsertReturnIdentityAsync<TEntity>(TEntity insertObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 表达式更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="columns"></param>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        bool Update<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new();

        /// <summary>
        /// 表达式更新异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="columns"></param>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TEntity>(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        bool Update<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        bool UpdateLock<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象更新异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象更新异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        Task<bool> UpdateLockAsync<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象更新忽略空值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        bool UpdateIgnoreNull<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 对象更新忽略空值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObj"></param>
        /// <returns></returns>
        bool UpdateLockIgnoreNull<TEntity>(TEntity updateObj) where TEntity : BaseEntity, new();

        /// <summary>
        /// 列表更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObjs"></param>
        /// <returns></returns>
        bool UpdateRange<TEntity>(TEntity[] updateObjs) where TEntity : BaseEntity, new();

        /// <summary>
        /// 列表更新异步
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObjs"></param>
        /// <returns></returns>
        Task<bool> UpdateRangeAsync<TEntity>(TEntity[] updateObjs) where TEntity : BaseEntity, new();

        /// <summary>
        /// 列表更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="updateObjs"></param>
        /// <returns></returns>
        bool UpdateRange<TEntity>(List<TEntity> updateObjs) where TEntity : BaseEntity, new();

        /// <summary>
        /// 包装事务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        bool Tran(Action action);

        /// <summary>
        /// 事务开始
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 事务提交
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 事务回滚
        /// </summary>
        void RollbackTran();

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool DropTable(string tableName);

        #region 查询

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
            where T12 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression) where T : BaseEntity, new()
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
            where T12 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public ISugarQueryable<ExpandoObject> Queryable(string tableName, string shortName);

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
            where T11 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new()
            where T11 : BaseEntity, new();

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
            where T11 : BaseEntity, new();

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
            where T12 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new();

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
            where T5 : BaseEntity, new();

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
            where T9 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, JoinQueryInfos>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression)
                        where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new();

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
            where T8 : BaseEntity, new();

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
            where T8 : BaseEntity, new();

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
            where T8 : BaseEntity, new();

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
            where T7 : BaseEntity, new();

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
            where T7 : BaseEntity, new();

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
            where T7 : BaseEntity, new();

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
            where T6 : BaseEntity, new();

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
            where T6 : BaseEntity, new();

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
            where T6 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Queryable<T>(string shortName) where T : BaseEntity, new();

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
            where T10 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new();

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
            where T4 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Queryable<T>(ISugarQueryable<T> queryable) where T : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ISugarQueryable<T> Queryable<T>() where T : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, ISugarQueryable<T3> joinQueryable3, JoinType joinType1, Expression<Func<T, T2, T3, bool>> joinExpression1, JoinType joinType2, Expression<Func<T, T2, T3, bool>> joinExpression2) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new();

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
        public ISugarQueryable<T, T2> Queryable<T, T2>(ISugarQueryable<T> joinQueryable1, ISugarQueryable<T2> joinQueryable2, JoinType joinType, Expression<Func<T, T2, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new();

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
            where T2 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new();

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
            where T3 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new();

        /// <summary>
        /// Queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="joinExpression"></param>
        /// <returns></returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, bool>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new();

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
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, JoinQueryInfos>> joinExpression) where T : BaseEntity, new()
            where T2 : BaseEntity, new()
            where T3 : BaseEntity, new()
            where T4 : BaseEntity, new()
            where T5 : BaseEntity, new()
            where T6 : BaseEntity, new()
            where T7 : BaseEntity, new()
            where T8 : BaseEntity, new()
            where T9 : BaseEntity, new()
            where T10 : BaseEntity, new();

        #endregion
    }
}
