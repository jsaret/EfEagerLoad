using System;
using System.Collections.Generic;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad
{
    public class EfEagerLoadContext
    {
        private readonly Stack<INavigation> _navigationTreeStack = new Stack<INavigation>();

        public EfEagerLoadContext(Type rootType, DbContext dbContext, IList<string> navigationPropertiesToIgnore,
                                    Func<EfEagerLoadContext, INavigation, bool> shouldIncludeNavigation)
        {
            Guard.IsNotNull(nameof(rootType), rootType);
            Guard.IsNotNull(nameof(dbContext), dbContext);
            Guard.IsNotNull(nameof(shouldIncludeNavigation), shouldIncludeNavigation);

            RootType = rootType;
            DbContext = dbContext;
            NavigationPropertiesToIgnore = navigationPropertiesToIgnore ?? new List<string>();
            ShouldIncludeNavigation = shouldIncludeNavigation;
        }

        public Type RootType { get; }


        public INavigation CurrentNavigation { get; private set; }

        public IReadOnlyList<INavigation> NavigationTree => _navigationTreeStack.ToArray();

        public DbContext DbContext { get; }

        public IList<string> NavigationPropertiesToIgnore { get; }

        public IList<Type> TypesVisited { get; } = new List<Type>();

        public Func<EfEagerLoadContext, INavigation, bool> ShouldIncludeNavigation { get; set; }

        internal void SetCurrentNavigation(INavigation navigation)
        {
            CurrentNavigation = navigation;
            _navigationTreeStack.Push(navigation);
        }

        internal INavigation RemoveCurrentNavigation()
        {
            if (_navigationTreeStack.Count == 0) { return null; }
            
            var currentNavigationToRemove = _navigationTreeStack.Pop();
            CurrentNavigation = (_navigationTreeStack.Count > 0) ? _navigationTreeStack.Peek() : null;
            return currentNavigationToRemove;

        }
    }
}
