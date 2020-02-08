using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Engine
{
    public class IncludeFinder
    {
        private static readonly ConcurrentDictionary<Type, INavigation[]> CachedTypeNavigations = new ConcurrentDictionary<Type, INavigation[]>();
        
        public IList<string> BuildIncludesForRootType(EagerLoadContext context)
        {
            BuildIncludesForType(context, context.RootType, string.Empty);
            return context.NavigationPathsToInclude;
        }

        private static void BuildIncludesForType(EagerLoadContext context, Type type, string navigationPath)
        {
            context.AddTypeVisited(type);
            var navigations = CachedTypeNavigations.GetOrAdd(type, typeToFind => 
                        context.DbContext.Model.FindEntityType(type).GetNavigations().ToArray());
            navigations = navigations.Where(currentNavigation => context.IncludeStrategy.ShouldIncludeNavigation(context, navigationPath))
                                    .ToArray();

            foreach (var navigation in navigations)
            {
                var navigationName = (!context.NavigationStack.Any()) ? $"{navigation.Name}" : $"{navigationPath}.{navigation.Name}";

                context.NavigationPath = navigationName;
                context.SetCurrentNavigation(navigation);
                context.NavigationPathsToInclude.Add(navigationName);

                var typeToExamine = navigation.IsCollection() ? navigation.GetTargetType().ClrType : navigation.ClrType;

                BuildIncludesForType(context, typeToExamine, navigationName);
                context.RemoveCurrentNavigation();
            }
        }
    }
}
