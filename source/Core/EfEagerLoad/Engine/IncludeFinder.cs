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
            BuildIncludesForEagerLoadContext(context);
            return context.IncludePathsToInclude;
        }

        private void BuildIncludesForEagerLoadContext(EagerLoadContext context)
        {
            BuildIncludesForEagerLoadContext();

            void BuildIncludesForEagerLoadContext()
            {
                var navigationsToConsider = _navigationFinder.GetNavigationsForType(context, context.CurrentType ?? context.RootType);

                foreach (var navigation in navigationsToConsider)
                {
                    context.SetCurrentNavigation(navigation);

                    if (context.IncludeStrategy.ShouldIncludeCurrentNavigation(context))
                    {
                        context.IncludePathsToInclude.Add(context.CurrentIncludePath);
                        BuildIncludesForEagerLoadContext();
                    }

                    context.RemoveCurrentNavigation();
                }
            }
        }
    }
}
