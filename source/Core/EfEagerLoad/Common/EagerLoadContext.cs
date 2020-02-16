using System;
using System.Collections.Generic;
using System.Threading;
using EfEagerLoad.Engine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Common
{
    public class EagerLoadContext
    {
        private static readonly AsyncLocal<IServiceProvider> ThreadLocalServiceProvider = new AsyncLocal<IServiceProvider>();
        public static bool SkipEntityFrameworkCheckForTesting { get; set; } = false;
        public static bool SkipQueryIncludeForTesting { get; set; } = false;
        internal static char SeparatorChar = char.Parse("."); 

        private readonly Stack<INavigation> _navigationPath = new Stack<INavigation>();

        public EagerLoadContext(DbContext dbContext, IIncludeStrategy includeStrategy, IList<string> includePathsToIgnore = null,
                                IncludeExecution includeExecution = IncludeExecution.Cached, Type rootType = null)
        {
            Guard.IsNotNull(nameof(dbContext), dbContext);
            Guard.IsNotNull(nameof(includeStrategy), includeStrategy);

            RootType = rootType;
            DbContext = dbContext;
            IncludePathsToIgnore =  new List<string>(includePathsToIgnore ?? new string[0]);
            IncludeStrategy = includeStrategy;
            IncludeExecution = includeExecution;
        }

        public Type RootType { get; internal set; }

        public Type CurrentType => CurrentNavigation?.GetNavigationType();

        public INavigation CurrentNavigation => (_navigationPath.Count > 0) ? _navigationPath.Peek() : null;

        public string CurrentIncludePath { get; private set; } = string.Empty;

        public IReadOnlyCollection<INavigation> NavigationPath => _navigationPath;

        public DbContext DbContext { get; }

        public IList<string> IncludePathsToIgnore { get; }
        
        public IIncludeStrategy IncludeStrategy { get; set; }

        public IncludeExecution IncludeExecution { get; }

        public IList<string> IncludePathsToInclude { get; internal set; } = new List<string>();

        public IDictionary<string, object> Bag = new Dictionary<string, object>(1);

        public IServiceProvider ServiceProvider
        {
            get => ThreadLocalServiceProvider.Value;
            set => InitializeServiceProvider(value);
        }

        internal void SetCurrentNavigation(INavigation navigation)
        {
            if(navigation == null) { return; }

            _navigationPath.Push(navigation);
            CurrentIncludePath = (_navigationPath.Count > 1) ?
                                $"{CurrentIncludePath}.{CurrentNavigation.Name}" :
                                CurrentNavigation.Name;
        }

        internal void RemoveCurrentNavigation()
        {
            if (_navigationPath.Count == 0) { return; }

            _navigationPath.Pop();
            CurrentIncludePath = (_navigationPath.Count > 1) ?
                                    CurrentIncludePath.AsSpan().GetParentIncludePathSpan().ToString() :
                                    CurrentNavigation?.Name ?? string.Empty;
        }

        public static void InitializeServiceProvider(IServiceProvider serviceProvider)
        {
            ThreadLocalServiceProvider.Value = serviceProvider;
        }

    }
}
