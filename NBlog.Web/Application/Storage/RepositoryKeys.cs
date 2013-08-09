using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NBlog.Web.Application.Storage
{
    /// <summary>
    /// A hash table for mapping entitiy types to their lookup key properties. Used by data
    /// repositories.
    /// </summary>
    public class RepositoryKeys
    {
        private readonly Dictionary<Type, Expression<Func<object, object>>> _keys =
            new Dictionary<Type, Expression<Func<object, object>>>();

        public void Add<T>(Expression<Func<T, object>> expression)
        {
            // use a conversion expression to convert Expression<Func<T, object>> to an
            // Expression<Func<object, object>>:
            Expression<Func<object, T>> converter = obj => (T)obj;
            var param = Expression.Parameter(typeof(object));
            var body = Expression.Invoke(expression, Expression.Invoke(converter, param));
            var lambda = Expression.Lambda<Func<object, object>>(body, param);

            _keys.Add(typeof(T), lambda);
        }

        public object GetKeyValue<T>(T item)
        {
            var getValue = _keys[typeof(T)].Compile();
            return getValue(item);
        }

        public string GetKeyName<T>(T item)
        {
            return GetKeyName<T>();
        }

        public string GetKeyName<T>()
        {
            var expression = _keys[typeof(T)];

            var conversionBody = (InvocationExpression)expression.Body;
            var conversionExpression = (Expression<Func<T, object>>)conversionBody.Expression;
            var body = (MemberExpression)conversionExpression.Body;
            var memberName = body.Member.Name;
            return memberName;
        }
    }
}