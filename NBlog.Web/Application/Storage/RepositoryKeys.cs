using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq.Expressions;

namespace NBlog.Web.Application.Storage
{
    public class RepositoryKeys
    {
        private readonly Dictionary<Type, Expression<Func<object, object>>> _keys = new Dictionary<Type, Expression<Func<object, object>>>();

        public void Add<T>(Expression<Func<T, object>> expression)
        {
            // use a conversion expression:

            Expression<Func<object, T>> convert = myObj => (T)myObj;

            var param = Expression.Parameter(typeof(object), "obj");  // don't need name?
            var body = Expression.Invoke(expression, Expression.Invoke(convert, param));
            var lambda = Expression.Lambda<Func<object, object>>(body, param);


            //var objectParam = new[] { Expression.Parameter(typeof(object), expression.Parameters[0].Name) };
            //var convertedExpression = Expression.Lambda<Func<object, object>>(expression.Body, objectParam);


            _keys.Add(typeof(T), lambda);
        }


        public static Expression<Func<TModel, TToProperty>> Cast<TModel, TFromProperty, TToProperty>(Expression<Func<TModel, TFromProperty>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(TToProperty));

            return Expression.Lambda<Func<TModel, TToProperty>>(converted, expression.Parameters);
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

        //private static string GetMemberName<T>(Expression<Func<object, object>> expression)
        //{
        //    var conversionBody = (InvocationExpression)expression.Body;
        //    var conversionExpression = (Expression<Func<T, object>>)conversionBody.Expression;
        //    var body = (MemberExpression)conversionExpression.Body;
        //    var memberName = body.Member.Name;
        //    return memberName;
        //}
    }
}