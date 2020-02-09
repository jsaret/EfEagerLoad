using System;
using System.Collections.Generic;
using EfEagerLoad.Common;

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
            BuildIncludesForType(context, context.RootType);
            return context.IncludePathsToInclude;
        }

        private void BuildIncludesForType(EagerLoadContext context, Type type)
        {
            context.TypesVisited.Add(type);
            var navigationsToConsider = _navigationFinder.GetNavigationsForType(context, type);

            foreach (var navigation in navigationsToConsider)
            {
                context.SetCurrentNavigation(navigation);
                if (context.IncludeStrategy.ShouldIncludeNavigation(context))
                {
                    context.RemoveCurrentNavigation();
                    continue;
                }

                context.IncludePathsToInclude.Add(context.CurrentIncludePath);
                BuildIncludesForType(context, navigation.GetNavigationType());
                
                context.RemoveCurrentNavigation();
            }
        }



    }
}
