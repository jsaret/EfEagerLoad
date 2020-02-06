using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Engine
{
    public class IncludeFinder
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<INavigation>> CachedTypeNavigations = new ConcurrentDictionary<Type, IEnumerable<INavigation>>();
        
        public IEnumerable<string> BuildIncludesForRootType(EagerLoadContext context)
        {
            BuildIncludesForType(context, context.RootType, string.Empty);
            return context.NavigationPathsFoundToInclude;
        }

        private static void BuildIncludesForType(EagerLoadContext context, Type type, string prefix)
        {
            context.AddTypeVisited(type);
            var navigations = CachedTypeNavigations.GetOrAdd(type, typeToFind => 
                        context.DbContext.Model.FindEntityType(type).GetNavigations().ToArray());
            navigations = navigations.Where(currentNavigation => context.IncludeStrategy.ShouldIncludeNavigation(context))
                                    .ToArray();

            foreach (var navigation in navigations)
            {
                var navigationName = $"{prefix}{navigation.Name}";

                if (context.IncludeStrategy.ShouldIgnoreNavigationPath(context, navigationName)) { continue; }

                context.CurrentPath = navigationName;
                context.SetCurrentNavigation(navigation);
                context.NavigationPathsFoundToInclude.Add(navigationName);
                context.NavigationPathsToIgnore.Add(navigationName);

                var typeToExamine = navigation.IsCollection() ? navigation.GetTargetType().ClrType : navigation.ClrType;

                BuildIncludesForType(context, typeToExamine, $"{navigationName}.");
                context.RemoveCurrentNavigation();
            }
        }
    }
}
