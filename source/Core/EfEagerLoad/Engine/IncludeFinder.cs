using System;
using System.Collections.Generic;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Engine
{
    internal class IncludeFinder
    {
        private static readonly NavigationFinder CachedNavigationFinder = new NavigationFinder();

        private readonly NavigationFinder _navigationFinder;

        internal IncludeFinder() : this(CachedNavigationFinder) { }

        internal IncludeFinder(NavigationFinder navigationFinder)
        {
            _navigationFinder = navigationFinder;
        }

        internal virtual IList<string> BuildIncludePathsForRootType(EagerLoadContext context)
        {
            BuildIncludesForEagerLoadContext2(context);
            return context.IncludePathsToInclude;
        }

        internal virtual IList<string> BuildIncludePathsForRootType2(EagerLoadContext context)
        {
            BuildIncludesForEagerLoadContext2(context);
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


        private void BuildIncludesForEagerLoadContext2(EagerLoadContext context)
        {
            var includeStack = new Stack<IncludeNavigationStack>();

            var navigations = _navigationFinder.GetNavigationsForType(context, context.RootType);
            includeStack.Push(new IncludeNavigationStack(navigations));

            while (includeStack.Count > 0)
            {
                var currentStack = includeStack.Peek();

                while (currentStack.Position < currentStack.Navigations.Length)
                {
                    var navigation = currentStack.Navigations[currentStack.Position];
                    context.SetCurrentNavigation(navigation);
                    currentStack.Position++;

                    if (!context.IncludeStrategy.ShouldIncludeCurrentNavigation(context))
                    {
                        context.RemoveCurrentNavigation();
                        continue;
                    }

                    context.IncludePathsToInclude.Add(context.CurrentIncludePath);
                    navigations = _navigationFinder.GetNavigationsForType(context, context.CurrentType);
                    var newStack = new IncludeNavigationStack(navigations);
                    includeStack.Push(newStack);
                    currentStack = newStack;
                }

                context.RemoveCurrentNavigation();
                includeStack.Pop();
            }
        }

        internal class IncludeNavigationStack
        {
            public IncludeNavigationStack(INavigation[] navigations)
            {
                Navigations = navigations;
            }

            internal int Position { get; set; }

            internal INavigation[] Navigations { get; }
        }

    }
}




