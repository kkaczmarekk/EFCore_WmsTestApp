using System.Linq.Expressions;
using System.Reflection;

namespace WmsApp.Persistence.Common.Extensions
{
    public static class QueryFilterExtensions
    {
        public static void ApplyQueryFiltersForCommonInterfaces(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var interfaces = entity.ClrType.GetInterfaces();

                if (interfaces.Contains(typeof(ISoftDeletable)))
                {
                    var softDeletebeFilterQuery = typeof(QueryFilterExtensions)
                        .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(entity.ClrType)
                        .Invoke(null, new object[] { });

                    entity.SetQueryFilter((LambdaExpression)softDeletebeFilterQuery);
                }
            }
        }

        private static LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : class, ISoftDeletable
        {
            Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
            return filter;
        }
    }
}
