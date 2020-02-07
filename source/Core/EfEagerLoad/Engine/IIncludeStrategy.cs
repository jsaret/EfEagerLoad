using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.Engine
{
    public interface IIncludeStrategy
    {
        bool ShouldIncludeNavigation(EagerLoadContext context, string navigationPath);

        void FilterNavigationPathsBeforeInclude(EagerLoadContext context);

        void ExecuteBeforeInclude(EagerLoadContext context);
    }
}
