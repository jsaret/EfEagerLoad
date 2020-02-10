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
        private string _parentIncludePath = string.Empty;
        private string _currentIncludePath = string.Empty;

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

        public INavigation CurrentNavigation => (_navigationPath.Any()) ? _navigationPath.Peek() : null;

        public string CurrentIncludePath => _currentIncludePath;

        public ReadOnlySpan<char> CurrentIncludePathSpan => _currentIncludePath.AsSpan();

        public string ParentIncludePath => _parentIncludePath;

        public ReadOnlySpan<char> ParentIncludePathSpan => _parentIncludePath.AsSpan();

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
            set => ThreadLocalServiceProvider.Value = value;
        }

        internal void SetCurrentNavigation(INavigation navigation)
        {
            if(navigation == null) { return; }

            _navigationPath.Push(navigation);
            _currentIncludePath = (NavigationPath.Skip(1).Any()) ? $"{CurrentIncludePath}.{CurrentNavigation.Name}" :
                                                                    CurrentNavigation.Name;
            _parentIncludePath = _currentIncludePath.AsSpan().GetParentIncludePathSpan().ToString();
        }
        
        internal void RemoveCurrentNavigation()
        {
            if (_navigationPath.Count == 0) { return; }

            _navigationPath.Pop();
            _currentIncludePath = (!NavigationPath.Skip(1).Any()) ? 
                                    CurrentNavigation?.Name ?? string.Empty :
                                    _parentIncludePath;
            _parentIncludePath = _currentIncludePath.AsSpan().GetParentIncludePathSpan().ToString();
        }

        public static void InitializeServiceProvider(IServiceProvider serviceProvider)
        {
            ThreadLocalServiceProvider.Value = serviceProvider;
        }

    }
}
