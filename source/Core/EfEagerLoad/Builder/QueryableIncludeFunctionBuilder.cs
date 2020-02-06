using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Builder
{
    public class QueryableIncludeFunctionBuilder
    {
        private static readonly ConcurrentDictionary<Type, object> CachedIncludeFunctions = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, IEnumerable<INavigation>> CachedTypeNavigations = new ConcurrentDictionary<Type, IEnumerable<INavigation>>();

        public Func<IQueryable<TEntity>, IQueryable<TEntity>> GetIncludeFunction<TEntity>(EagerLoadContext eagerLoadContext) where TEntity : class
        {
            static IQueryable<TEntity> ResultFunction(IQueryable<TEntity> queryable) => queryable;

            eagerLoadContext.RootType = typeof(TEntity);

            if (eagerLoadContext.IncludeExecution == IncludeExecution.Skip) { return ResultFunction;}

            if (eagerLoadContext.IncludeExecution == IncludeExecution.Cached)
            {
                return (Func<IQueryable<TEntity>, IQueryable<TEntity>>)CachedIncludeFunctions.GetOrAdd(typeof(TEntity), (type) =>
                        ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty, (Func<IQueryable<TEntity>, IQueryable<TEntity>>)ResultFunction,
                            eagerLoadContext));
            }

            var result = ComposeFunctionForIncludesForType(typeof(TEntity), string.Empty, (Func<IQueryable<TEntity>, IQueryable<TEntity>>)ResultFunction,
                                                            eagerLoadContext);

            if (eagerLoadContext.IncludeExecution == IncludeExecution.Recache)
            {
                CachedIncludeFunctions.TryAdd(typeof(TEntity), result);
            }

            return ResultFunction;
        }

        private static void BuildIncludesForRootType(EagerLoadContext eagerLoadContext)
        {
            BuildIncludesForType(eagerLoadContext, eagerLoadContext.RootType, string.Empty);
        }

        private static void BuildIncludesForType(EagerLoadContext eagerLoadContext, Type type, string prefix)
        {
            eagerLoadContext.AddTypeVisited(type);
            var navigations = CachedTypeNavigations.GetOrAdd(type, typeToFind => 
                        eagerLoadContext.DbContext.Model.FindEntityType(type).GetNavigations().ToArray());
            navigations = navigations.Where(currentNavigation => eagerLoadContext.IncludeStrategy.ShouldIncludeNavigation(eagerLoadContext))
                                    .ToArray();

            BuildIncludesForNavigations(eagerLoadContext, navigations, prefix);
        }

        private static void BuildIncludesForNavigations(EagerLoadContext eagerLoadContext, IEnumerable<INavigation> navigationToInclude, string prefix)
        {
            foreach (var navigation in navigationToInclude)
            {
                var navigationName = $"{prefix}{navigation.Name}";
                if (eagerLoadContext.IncludeStrategy.ShouldIgnoreNavigationPath(eagerLoadContext, navigationName)) { continue; }

                eagerLoadContext.CurrentPath = navigationName;
                eagerLoadContext.SetCurrentNavigation(navigation);
                eagerLoadContext.NavigationPathsFoundToInclude.Add(navigationName);
                eagerLoadContext.NavigationPathsToIgnore.Add(navigationName);

                var navigationPathNameToFollow = navigation.IsCollection() ? $"{navigationName}." : navigationName;
                var typeToExamine = navigation.IsCollection() ? navigation.GetTargetType().ClrType : navigation.ClrType;

                BuildIncludesForType(eagerLoadContext, typeToExamine, navigationPathNameToFollow);
                eagerLoadContext.RemoveCurrentNavigation();
            }
        }


        private static Func<IQueryable<TEntity>, IQueryable<TEntity>> ComposeFunctionForIncludesForType<TEntity>(Type type, string prefix,
                        Func<IQueryable<TEntity>, IQueryable<TEntity>> originalFunction, EagerLoadContext eagerLoadContext)
            where TEntity : class
        {
            eagerLoadContext.AddTypeVisited(type);

            var navigationProperties = CachedTypeNavigations.GetOrAdd(type, _ => eagerLoadContext.DbContext.Model.FindEntityType(type).GetNavigations().ToArray());
            navigationProperties = navigationProperties.Where(currentNavigation => eagerLoadContext.IncludeStrategy.ShouldIncludeNavigation(eagerLoadContext)).ToArray();

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
                if (eagerLoadContext.NavigationPathsToIgnore.Contains(navigationName)) { continue; }

                eagerLoadContext.SetCurrentNavigation(navigationProperty);

                IQueryable<TEntity> ChainedIncludeFunc(IQueryable<TEntity> f)
                {
                    Console.WriteLine(navigationName);
                    return f.Include(navigationName);
                }

                resultingFunction = ComposeQueryFunctions(resultingFunction, ChainedIncludeFunc);
                eagerLoadContext.NavigationPathsToIgnore.Add(navigationName);

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
