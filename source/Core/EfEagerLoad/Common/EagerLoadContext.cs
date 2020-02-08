﻿using System;
using System.Collections.Generic;
using EfEagerLoad.Engine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Common
{
    public class EagerLoadContext
    {
        private readonly Stack<INavigation> _navigationStack = new Stack<INavigation>();
        private readonly List<Type> _typesVisited = new List<Type>();

        public EagerLoadContext(DbContext dbContext, IIncludeStrategy includeStrategy, IList<string> navigationPathsToIgnore = null,
                                IncludeExecution includeExecution = IncludeExecution.Cached, Type rooType = null)
        {
            Guard.IsNotNull(nameof(dbContext), dbContext);
            Guard.IsNotNull(nameof(includeStrategy), includeStrategy);

            RootType = rooType;
            DbContext = dbContext;
            NavigationPathsToIgnore =  new List<string>(navigationPathsToIgnore ?? new string[0]);
            IncludeStrategy = includeStrategy;
            IncludeExecution = includeExecution;
        }

        public Type RootType { get; internal set; }

         public INavigation CurrentNavigation { get; private set; }

         public string NavigationPath { get; internal set; }

        public IEnumerable<INavigation> NavigationStack => _navigationStack;

        public DbContext DbContext { get; }

        public IList<string> NavigationPathsToIgnore { get; }

        public IEnumerable<Type> TypesVisited => _typesVisited;

        public IIncludeStrategy IncludeStrategy { get; set; }

        public IncludeExecution IncludeExecution { get; }

        public IList<string> NavigationPathsToInclude { get; internal set; } = new List<string>();

        internal void AddTypeVisited(Type visitedType) => _typesVisited.Add(visitedType);

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
