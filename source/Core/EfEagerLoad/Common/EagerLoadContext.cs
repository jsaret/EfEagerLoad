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

        public Type CurrentType => CurrentNavigation?.GetNavigationType();

        public INavigation CurrentNavigation => (_navigationPath.Any()) ? _navigationPath.Peek() : null;

        public string CurrentIncludePath { get; private set; } = string.Empty;

        public ReadOnlySpan<char> CurrentIncludePathSpan => CurrentIncludePath.AsSpan();

        public string ParentIncludePath { get; private set; } = string.Empty;

        public ReadOnlySpan<char> ParentIncludePathSpan => ParentIncludePath.AsSpan();

        public IEnumerable<INavigation> NavigationPath => _navigationPath;

        public DbContext DbContext { get; }

        public IList<string> IncludePathsToIgnore { get; }
        
        public IIncludeStrategy IncludeStrategy { get; set; }

        public IncludeExecution IncludeExecution { get; }

        public IList<string> IncludePathsToInclude { get; internal set; } = new List<string>();

        public IDictionary<string, object> Bag = new Dictionary<string, object>(5);

        public IServiceProvider ServiceProvider
        {
            get => ThreadLocalServiceProvider.Value;
            set => InitializeServiceProvider(value);
        }

        internal void SetCurrentNavigation(INavigation navigation)
        {
            if(navigation == null) { return; }

            _navigationPath.Push(navigation);
            CurrentIncludePath = (NavigationPath.Skip(1).Any()) ? $"{CurrentIncludePath}.{CurrentNavigation.Name}" :
                                                                    CurrentNavigation.Name;
            ParentIncludePath = CurrentIncludePathSpan.GetParentIncludePathSpan().ToString();
        }
        
        internal void RemoveCurrentNavigation()
        {
            if (_navigationPath.Count == 0) { return; }

            _navigationPath.Pop();
            CurrentIncludePath = (!NavigationPath.Skip(1).Any()) ? 
                                    CurrentNavigation?.Name ?? string.Empty :
                                    ParentIncludePath;
            ParentIncludePath = CurrentIncludePathSpan.GetParentIncludePathSpan().ToString();
        }

        public static void InitializeServiceProvider(IServiceProvider serviceProvider)
        {
            ThreadLocalServiceProvider.Value = serviceProvider;
        }

    }
}
