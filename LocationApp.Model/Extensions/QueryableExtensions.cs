using System.Linq.Expressions;

namespace LocationApp.Model.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string orderByMember, string direction)
        {
            var orderByProps = orderByMember.Split('.');
            var queryElementTypeParam = Expression.Parameter(typeof(T));

            Expression orderByProperty = queryElementTypeParam;
            foreach (var prop in orderByProps)
            {
                orderByProperty = Expression.PropertyOrField(orderByProperty, prop);
            }

            var keySelector = Expression.Lambda(orderByProperty, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                direction.Equals("asc") ? "OrderBy" : "OrderByDescending",
                new[] { typeof(T), orderByProperty.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static IQueryable<T> OrderByPage<T>(this IQueryable<T> query, string orderByMember, string direction, int page, int pageSize)
        {
            // Calculate skip
            var skip = (page - 1) * pageSize;

            return query.OrderByDynamic(orderByMember, direction).Skip(skip).Take(pageSize);
        }
    }
}
