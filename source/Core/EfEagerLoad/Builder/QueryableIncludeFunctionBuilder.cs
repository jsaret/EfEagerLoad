using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Builder
{
    public class QueryableIncludeFunctionBuilder
    {
        private static readonly ConcurrentDictionary<Type, object> IncludeFunctions = new ConcurrentDictionary<Type, object>();

        public Func<IQueryable<TEntity>, IQueryable<TEntity>> GetIncludeFunction<TEntity>(EagerLoadContext eagerLoadContext) where TEntity : class
        {
            if (eagerLoadContext.NavigationPropertiesToIgnore.Count == 0)
            {
                return (Func<IQueryable<TEntity>, IQueryable<TEntity>>)IncludeFunctions.GetOrAdd(typeof(TEntity), type =>
                {
                    static IQueryable<TEntity> CachedResultFunction(IQueryable<TEntity> queryable) => queryable;
                    return ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty,
                        (Func<IQueryable<TEntity>, IQueryable<TEntity>>)CachedResultFunction, eagerLoadContext);
                });
            }

            static IQueryable<TEntity> ResultFunction(IQueryable<TEntity> queryable) => queryable;
            return ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty, (Func<IQueryable<TEntity>, IQueryable<TEntity>>)ResultFunction,
                eagerLoadContext);
        }

        private static Func<IQueryable<TEntity>, IQueryable<TEntity>> ComposeFunctionForIncludesForType<TEntity>(Type type, string prefix,
                        Func<IQueryable<TEntity>, IQueryable<TEntity>> originalFunction, EagerLoadContext eagerLoadContext)
            where TEntity : class
        {
            eagerLoadContext.AddTypeVisited(type);

            var navigationProperties = eagerLoadContext.DbContext.Model.FindEntityType(type).GetNavigations()
                                        .Where(currentNavigation => eagerLoadContext.IncludeStrategy.ShouldIncludeNavigation(eagerLoadContext));

            var resultingFunction = originalFunction;

            resultingFunction = ComposeIncludeFunctionsForNavigationProperties(prefix, navigationProperties, resultingFunction, eagerLoadContext);

            return resultingFunction;
        }

        private static Func<IQueryable<TEntity>, IQueryable<TEntity>> ComposeIncludeFunctionsForNavigationProperties<TEntity>(
                                                                string prefix, IEnumerable<INavigation> navigationProperties,
                                                                Func<IQueryable<TEntity>, IQueryable<TEntity>> outerFunction,
                                                                EagerLoadContext eagerLoadContext) where TEntity : class
        {
            var resultingFunction = outerFunction;

            foreach (var navigationProperty in navigationProperties)
            {
                var navigationName = $"{prefix}{navigationProperty.Name}";
                if (eagerLoadContext.NavigationPropertiesToIgnore.Contains(navigationName)) { continue; }

                eagerLoadContext.SetCurrentNavigation(navigationProperty);

                IQueryable<TEntity> ChainedIncludeFunc(IQueryable<TEntity> f) => f.Include(navigationName);

                resultingFunction = ComposeQueryFunctions(resultingFunction, ChainedIncludeFunc);
                eagerLoadContext.NavigationPropertiesToIgnore.Add(navigationName);

                if (navigationProperty.IsCollection())
                {
                    var newCollectionPrefix = $"{navigationName}.";
                    var collectionTargetType = navigationProperty.GetTargetType().ClrType;

                    resultingFunction = ComposeFunctionForIncludesForType(collectionTargetType, newCollectionPrefix,
                                    resultingFunction, eagerLoadContext);
                    continue;
                }

                var targetType = navigationProperty.ClrType;
                var newPrefix = $"{navigationName}.";
                resultingFunction = ComposeFunctionForIncludesForType(targetType, newPrefix, resultingFunction, eagerLoadContext);

                eagerLoadContext.RemoveCurrentNavigation();
            }

            return resultingFunction;
        }

        private static Func<T, T> ComposeQueryFunctions<T>(Func<T, T> innerFunction, Func<T, T> outerFunction)
        {
            return arg => outerFunction(innerFunction(arg));
        }
    }
}
