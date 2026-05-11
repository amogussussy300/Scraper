using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Scraper.Core
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<TSource> OrderBy<TSource>
            (this IQueryable<TSource> source, string propertyName) 
            => ApplyOrdering(source, propertyName, ascending: true);

        public static IOrderedQueryable<TSource> OrderByDescending<TSource>
            (this IQueryable<TSource> source, string propertyName)
            => ApplyOrdering(source, propertyName, ascending: false);

        private static IOrderedQueryable<TSource> ApplyOrdering<TSource>(IQueryable<TSource> source, string propertyName, bool ascending)
        {
            var param = Expression.Parameter(typeof(TSource), "x");
            var property = typeof(TSource).GetProperty(propertyName) ?? throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(TSource).Name}'.");
            var propertyAccess = Expression.Property(param, property);
            var lambda = Expression.Lambda(propertyAccess, param);
            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var orderByExpression = Expression.Call(typeof(Queryable), methodName, [typeof(TSource), property.PropertyType], source.Expression, Expression.Quote(lambda));
            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery(orderByExpression);
        }
    }
}
