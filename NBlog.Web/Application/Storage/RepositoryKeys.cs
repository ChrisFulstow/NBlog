using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq.Expressions;

namespace NBlog.Web.Application.Storage
{
    public class RepositoryKeys
    {
        private readonly Dictionary<Type, Expression<Func<object, object>>> _keys =
            new Dictionary<Type, Expression<Func<object, object>>>();

        public void Add<T>(Expression<Func<T, object>> expression)
        {
            // use a conversion expression to convert Expression<Func<T, object>> into Expression<Func<object, object>>:
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