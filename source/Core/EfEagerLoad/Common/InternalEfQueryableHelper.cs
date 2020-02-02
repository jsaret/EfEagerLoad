using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Common
{
    public static class InternalEfQueryableHelper
    {
        private static readonly ConcurrentDictionary<Type, object> IncludeFunctions = new ConcurrentDictionary<Type, object>();
        
        internal static Func<IQueryable<TEntity>, IQueryable<TEntity>> GetIncludeFunction<TEntity>(EfEagerLoadContext eagerLoadContext) where TEntity : class
        {
            //ToDO: Still need to include mechanism for having cached Func based on the ignoredNavigationProperties in addition to the type?
            if (eagerLoadContext.NavigationPropertiesToIgnore.Count == 0)
            {
                return (Func<IQueryable<TEntity>, IQueryable<TEntity>>) IncludeFunctions.GetOrAdd(typeof(TEntity), type =>
                {
                    IQueryable<TEntity> cachedResultFunction(IQueryable<TEntity> queryable) => queryable;
                    return ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty, (Func<IQueryable<TEntity>, IQueryable<TEntity>>) cachedResultFunction, 
                                                            dbContext, ignoredNavigationProperties.ToList(), new List<Type>(), navigationFilter);
                });
            }

            static IQueryable<TEntity> ResultFunction(IQueryable<TEntity> queryable) => queryable;
            return ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty, (Func<IQueryable<TEntity>, IQueryable<TEntity>>)ResultFunction, 
                                                    dbContext, ignoredNavigationProperties.ToList(), new List<Type>(), navigationFilter);
        }

        private static Func<IQueryable<TEntity>, IQueryable<TEntity>> ComposeFunctionForIncludesForType<TEntity>(Type type, string prefix,
                        Func<IQueryable<TEntity>, IQueryable<TEntity>> originalFunction, DbContext dbContext, 
                        ICollection<string> ignoredNavigationProperties, ICollection<Type> typesAlreadyDone, Predicate<INavigation> navigationFilter) 
            where TEntity : class
        {
            if (typesAlreadyDone.Contains(type)) { return originalFunction; }
            typesAlreadyDone.Add(type);

            var navigationProperties = dbContext.Model.FindEntityType(type).GetNavigations().Where(nav => navigationFilter(nav)).ToArray();

            if (!navigationProperties.Any()) { return originalFunction; }

            var resultingFunction = originalFunction;

            resultingFunction = ComposeIncludeFunctionsForNavigationProperties(prefix, dbContext, ignoredNavigationProperties, typesAlreadyDone, 
                navigationProperties, resultingFunction, navigationFilter);

            return resultingFunction;
        }

        private static Func<IQueryable<TEntity>, IQueryable<TEntity>> ComposeIncludeFunctionsForNavigationProperties<TEntity>(
                    string prefix, DbContext dbContext, ICollection<string> ignoredNavigationProperties, ICollection<Type> typesAlreadyDone, 
                    IEnumerable<INavigation> navigationProperties, Func<IQueryable<TEntity>, IQueryable<TEntity>> resultingFunction, 
                    Predicate<INavigation> navigationFilter) where TEntity : class
        {
            foreach (var navigationProperty in navigationProperties)
            {
                var navigationName = $"{prefix}{navigationProperty.Name}";
                if (ignoredNavigationProperties.Contains(navigationName))
                {
                    continue;
                }

                IQueryable<TEntity> ChainedIncludeFunc(IQueryable<TEntity> f) => f.Include(navigationName);

                resultingFunction = JoinQueryFunctions(resultingFunction, ChainedIncludeFunc);
                ignoredNavigationProperties.Add(navigationName);

                if (navigationProperty.IsCollection())
                {
                    var collectionTargetType = navigationProperty.GetTargetType().ClrType;
                    var newCollectionPrefix = $"{navigationName}.";
                    resultingFunction = ComposeFunctionForIncludesForType(collectionTargetType, newCollectionPrefix,
                                    resultingFunction, dbContext, ignoredNavigationProperties, typesAlreadyDone, navigationFilter);
                    continue;
                }

                var targetType = navigationProperty.ClrType;
                var newPrefix = $"{navigationName}.";
                resultingFunction = ComposeFunctionForIncludesForType(targetType, newPrefix, resultingFunction, dbContext,
                                        ignoredNavigationProperties, typesAlreadyDone, navigationFilter);
            }

            return resultingFunction;
        }

        private static Func<T, T> JoinQueryFunctions<T>(Func<T, T> innerFunction, Func<T, T> outerFunction)
        {
            return arg => outerFunction(innerFunction(arg));
        }
    }
}
