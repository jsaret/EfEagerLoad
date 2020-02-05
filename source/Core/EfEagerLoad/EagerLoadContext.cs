using System;
using System.Collections.Generic;
using EfEagerLoad.Common;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad
{
    public class EagerLoadContext
    {
        private readonly Stack<INavigation> _navigationStack = new Stack<INavigation>();
        private readonly List<Type> _typesVisited = new List<Type>();

        public EagerLoadContext(Type rootType, DbContext dbContext, IList<string> navigationPropertiesToIgnore,
                                IIncludeStrategy includeStrategy, IncludeExecution includeExecution)
        {
            Guard.IsNotNull(nameof(rootType), rootType);
            Guard.IsNotNull(nameof(dbContext), dbContext);
            Guard.IsNotNull(nameof(includeStrategy), includeStrategy);

            RootType = rootType;
            DbContext = dbContext;
            NavigationPropertiesToIgnore = navigationPropertiesToIgnore ?? new List<string>();
            IncludeStrategy = includeStrategy;
            IncludeExecution = includeExecution;
        }

        public Type RootType { get; }


        public INavigation CurrentNavigation { get; private set; }

        public IReadOnlyList<INavigation> NavigationPath => _navigationStack.ToArray();

        public DbContext DbContext { get; }

        public IList<string> NavigationPropertiesToIgnore { get; }

        public IReadOnlyList<Type> TypesVisited => _typesVisited;

        public IIncludeStrategy IncludeStrategy { get; set; }

        public IncludeExecution IncludeExecution { get; }

        public IList<string> PathsToInclude { get; } = new List<string>();

        public void AddTypeVisited(Type visitedType) => _typesVisited.Add(visitedType);

        internal void SetCurrentNavigation(INavigation navigation)
        {
            CurrentNavigation = navigation;
            _navigationStack.Push(navigation);
        }

        internal INavigation RemoveCurrentNavigation()
        {
            if (_navigationStack.Count == 0) { return null; }
            
            var currentNavigationToRemove = _navigationStack.Pop();
            CurrentNavigation = (_navigationStack.Count > 0) ? _navigationStack.Peek() : null;
            return currentNavigationToRemove;

        }
    }
}
