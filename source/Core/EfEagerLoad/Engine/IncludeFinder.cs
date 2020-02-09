using System;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Engine
{
    public class IncludeFinder
    {
        private static readonly NavigationFinder CachedNavigationFinder = new NavigationFinder();

        private readonly NavigationFinder _navigationFinder;

        public IncludeFinder() : this(CachedNavigationFinder) { }

        public IncludeFinder(NavigationFinder navigationFinder)
        {
            _navigationFinder = navigationFinder;
        }

        public IList<string> BuildIncludePathsForRootType(EagerLoadContext context)
        {
            BuildIncludesForType(context, context.RootType, string.Empty);
            return context.IncludePathsToInclude;
        }

        internal void BuildIncludesForType(EagerLoadContext context, Type type, string parentIncludePath)
        {
            context.AddTypeVisited(type);
            context.ParentIncludePath = parentIncludePath;
            var navigationToConsider = _navigationFinder.GetNavigationsForType(context, type);
            var navigationToInclude = navigationToConsider.Where(navigation => ShouldIncludeNavigation(context, navigation));

            foreach (var navigation in navigationToInclude)
            {
                context.IncludePathsToInclude.Add(context.CurrentIncludePath);
                context.SetCurrentNavigation(navigation);
                BuildIncludesForType(context, navigation.GetNavigationType(), parentIncludePath);
                context.RemoveCurrentNavigation();
            }
        }

        private static bool ShouldIncludeNavigation(EagerLoadContext context, INavigation navigation)
        {
            context.SetCurrentNavigation(navigation);
            var result = context.IncludeStrategy.ShouldIncludeNavigation(context);
            context.RemoveCurrentNavigation();
            return result;
        }

    }
}
