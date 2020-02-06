using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Builder
{
    public class QueryableIncludeFunctionBuilder
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<string>> CachedIncludeNavigationPaths = new ConcurrentDictionary<Type, IEnumerable<string>>();
        private static readonly ConcurrentDictionary<Type, IEnumerable<INavigation>> CachedTypeNavigations = new ConcurrentDictionary<Type, IEnumerable<INavigation>>();

        public IEnumerable<string> BuildIncludesForRootType<TEntity>(EagerLoadContext eagerLoadContext) where TEntity : class
        {
           if(eagerLoadContext.RootType == null) { eagerLoadContext.RootType = typeof(TEntity); }

            return BuildIncludesForRootType(eagerLoadContext);
        }

        public static IEnumerable<string> BuildIncludesForRootType(EagerLoadContext eagerLoadContext)
        {
            Guard.IsNotNull(nameof(EagerLoadContext.RootType), eagerLoadContext.RootType);
            IEnumerable<string> includeStatements = new string[0];

            switch (eagerLoadContext.IncludeExecution)
            {
                case IncludeExecution.Cached:
                {
                    includeStatements = CachedIncludeNavigationPaths.GetOrAdd(eagerLoadContext.RootType, (type) => BuildIncludesForRootTypeCore(eagerLoadContext));
                    break;
                }
                case IncludeExecution.NoCache:
                {
                    includeStatements = BuildIncludesForRootTypeCore(eagerLoadContext);
                    break;
                }
                case IncludeExecution.Recache:
                {
                    includeStatements = BuildIncludesForRootTypeCore(eagerLoadContext);
                    CachedIncludeNavigationPaths.TryAdd(eagerLoadContext.RootType, includeStatements);
                    break;
                }
            }

            return includeStatements;
        }

        private static IEnumerable<string> BuildIncludesForRootTypeCore(EagerLoadContext eagerLoadContext)
        {
            BuildIncludesForType(eagerLoadContext, eagerLoadContext.RootType, string.Empty, _ => {});
            return eagerLoadContext.NavigationPathsFoundToInclude;
        }

        private static void BuildIncludesForType(EagerLoadContext eagerLoadContext, Type type, string prefix, Action<EagerLoadContext> closingAction)
        {
            eagerLoadContext.AddTypeVisited(type);
            var navigations = CachedTypeNavigations.GetOrAdd(type, typeToFind => 
                        eagerLoadContext.DbContext.Model.FindEntityType(type).GetNavigations().ToArray());
            navigations = navigations.Where(currentNavigation => eagerLoadContext.IncludeStrategy.ShouldIncludeNavigation(eagerLoadContext))
                                    .ToArray();

            foreach (var navigation in navigations)
            {
                var navigationName = eagerLoadContext.NavigationPath.Any() ? $"{prefix}.{navigation.Name}" : $"{navigation.Name}";

                if (eagerLoadContext.IncludeStrategy.ShouldIgnoreNavigationPath(eagerLoadContext, navigationName)) { continue; }

                eagerLoadContext.CurrentPath = navigationName;
                eagerLoadContext.SetCurrentNavigation(navigation);
                eagerLoadContext.NavigationPathsFoundToInclude.Add(navigationName);
                eagerLoadContext.NavigationPathsToIgnore.Add(navigationName);

                var typeToExamine = navigation.IsCollection() ? navigation.GetTargetType().ClrType : navigation.ClrType;

                BuildIncludesForType(eagerLoadContext, typeToExamine, navigationName, (cxt) =>
                {
                    cxt.RemoveCurrentNavigation();
                    closingAction(cxt);
                });
            }
        }
    }
}
