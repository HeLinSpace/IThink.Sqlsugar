using SqlSugar;
using System;

namespace IThink.Sqlsugar
{

    /// <summary>
    /// 模型基类
    /// </summary>
    public abstract class BaseEntity
    {
        #region Properties
        /// <summary>
        /// 表ID属性
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public virtual string Id { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 对象比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(string));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        /// <summary>
        /// 获取hash码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (Equals(Id, default(string)))
            {
                return base.GetHashCode();
            }
            return Id.GetHashCode();
        }

        /// <summary>
        /// 对象比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(BaseEntity other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!IsTransient(this) && !IsTransient(other) && Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
            }

            return false;
        }
        #endregion

        #region Operators
        /// <summary>
        /// 扩展==操作符
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// 扩展！=操作符
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
        #endregion
    }
}
