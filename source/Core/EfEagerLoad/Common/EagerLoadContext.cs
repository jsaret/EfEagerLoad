using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EfEagerLoad.Engine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Common
{
    public class EagerLoadContext
    {
        private static readonly AsyncLocal<IServiceProvider> ThreadLocalServiceProvider = new AsyncLocal<IServiceProvider>();

        private readonly Stack<INavigation> _navigationPath = new Stack<INavigation>();
        private readonly List<Type> _typesVisited = new List<Type>();

        public EagerLoadContext(DbContext dbContext, IIncludeStrategy includeStrategy, IList<string> includePathsToIgnore = null,
                                IncludeExecution includeExecution = IncludeExecution.Cached, Type rooType = null)
        {
            Guard.IsNotNull(nameof(dbContext), dbContext);
            Guard.IsNotNull(nameof(includeStrategy), includeStrategy);

            RootType = rooType;
            DbContext = dbContext;
            IncludePathsToIgnore =  new List<string>(includePathsToIgnore ?? new string[0]);
            IncludeStrategy = includeStrategy;
            IncludeExecution = includeExecution;
        }

        public Type RootType { get; internal set; }

         public INavigation CurrentNavigation { get; private set; }

         public string CurrentIncludePath
         {
             get
             {
                 if (CurrentNavigation == null) { return string.Empty; }

                 return (NavigationPath.Skip(1).Any()) ? $"{ParentIncludePath}.{CurrentNavigation.Name}" :
                                                            $"{CurrentNavigation.Name}";
             }
         }

         public string ParentIncludePath { get; internal set; }


        public IEnumerable<INavigation> NavigationPath => _navigationPath;

        public DbContext DbContext { get; }

        public IList<string> IncludePathsToIgnore { get; }

        public IEnumerable<Type> TypesVisited => _typesVisited;

        public IIncludeStrategy IncludeStrategy { get; set; }

        public IncludeExecution IncludeExecution { get; }

        public IList<string> IncludePathsToInclude { get; internal set; } = new List<string>();

        public IServiceProvider ServiceProvider
        {
            get => ThreadLocalServiceProvider.Value;
            set => ThreadLocalServiceProvider.Value = value;
        }

        internal void AddTypeVisited(Type visitedType) => _typesVisited.Add(visitedType);

        internal void SetCurrentNavigation(INavigation navigation)
        {
            CurrentNavigation = navigation;
            _navigationPath.Push(navigation);
        }

        internal INavigation RemoveCurrentNavigation()
        {
            if (_navigationPath.Count == 0) { return null; }
            
            var currentNavigationToRemove = _navigationPath.Pop();
            CurrentNavigation = (_navigationPath.Count > 0) ? _navigationPath.Peek() : null;
            return currentNavigationToRemove;

        }

        public static void InitializeServiceProvider(IServiceProvider serviceProvider)
        {
            ThreadLocalServiceProvider.Value = serviceProvider;
        }

    }
}
