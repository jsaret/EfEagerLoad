using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Internal
{
    public static class InternalEfQueryableHelper
    {
        private static readonly ConcurrentDictionary<Type, object> IncludeFunctions = new ConcurrentDictionary<Type, object>();
        
        internal static Func<IQueryable<TEntity>, IQueryable<TEntity>> GetIncludeFunction<TEntity>(EfEagerLoadContext eagerLoadContext) 
                                                                                                    where TEntity : class
        {
            if (eagerLoadContext.NavigationPropertiesToIgnore.Count == 0)
            {
                return (Func<IQueryable<TEntity>, IQueryable<TEntity>>) IncludeFunctions.GetOrAdd(typeof(TEntity), type =>
                {
                    static IQueryable<TEntity> CachedResultFunction(IQueryable<TEntity> queryable) => queryable;
                    return ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty, 
                                (Func<IQueryable<TEntity>, IQueryable<TEntity>>)CachedResultFunction, eagerLoadContext);
                });
            }

            IQueryable<TEntity> ResultFunction(IQueryable<TEntity> queryable) => queryable;
            return ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty, (Func<IQueryable<TEntity>, IQueryable<TEntity>>)ResultFunction, 
                                                    eagerLoadContext);
        }

        private static Func<IQueryable<TEntity>, IQueryable<TEntity>> ComposeFunctionForIncludesForType<TEntity>(Type type, string prefix,
                        Func<IQueryable<TEntity>, IQueryable<TEntity>> originalFunction, EfEagerLoadContext eagerLoadContext) 
            where TEntity : class
        {
            if (!eagerLoadContext.ShouldIncludeTypePredicate(eagerLoadContext, type)) { return originalFunction; }
            eagerLoadContext.TypesVisited.Add(type);

            var navigationProperties = eagerLoadContext.DbContext.Model.FindEntityType(type).GetNavigations()
                                        .Where(nav => eagerLoadContext.ShouldIncludeNavigationPredicate(eagerLoadContext, nav)).ToArray();

            if (!navigationProperties.Any()) { return originalFunction; }

            var resultingFunction = originalFunction;

            resultingFunction = ComposeIncludeFunctionsForNavigationProperties(prefix, navigationProperties, resultingFunction, eagerLoadContext);

            return resultingFunction;
        }

        private static Func<IQueryable<TEntity>, IQueryable<TEntity>> ComposeIncludeFunctionsForNavigationProperties<TEntity>(
                                                                string prefix, IEnumerable<INavigation> navigationProperties, 
                                                                Func<IQueryable<TEntity>, IQueryable<TEntity>> resultingFunction,
                                                                EfEagerLoadContext eagerLoadContext) where TEntity : class
        {
            foreach (var navigationProperty in navigationProperties)
            {
                var navigationName = $"{prefix}{navigationProperty.Name}";
                if (eagerLoadContext.NavigationPropertiesToIgnore.Contains(navigationName)){ continue; }

                IQueryable<TEntity> ChainedIncludeFunc(IQueryable<TEntity> f) => f.Include(navigationName);

                resultingFunction = JoinQueryFunctions(resultingFunction, ChainedIncludeFunc);
                eagerLoadContext.NavigationPropertiesToIgnore.Add(navigationName);

                if (navigationProperty.IsCollection())
                {
                    var collectionTargetType = navigationProperty.GetTargetType().ClrType;
                    var newCollectionPrefix = $"{navigationName}.";
                    resultingFunction = ComposeFunctionForIncludesForType(collectionTargetType, newCollectionPrefix,
                                    resultingFunction, eagerLoadContext);
                    continue;
                }

                var targetType = navigationProperty.ClrType;
                var newPrefix = $"{navigationName}.";
                resultingFunction = ComposeFunctionForIncludesForType(targetType, newPrefix, resultingFunction, eagerLoadContext);
            }

            return resultingFunction;
        }

        private static Func<T, T> JoinQueryFunctions<T>(Func<T, T> innerFunction, Func<T, T> outerFunction)
        {
            return arg => outerFunction(innerFunction(arg));
        }
    }
}
