using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.Engine
{
    public interface IIncludeStrategy
    {
        bool ShouldIncludeNavigation(EagerLoadContext context);

        void FilterIncludePathsBeforeInclude(EagerLoadContext context);

        void ExecuteBeforeInclude(EagerLoadContext context);
    }
}
