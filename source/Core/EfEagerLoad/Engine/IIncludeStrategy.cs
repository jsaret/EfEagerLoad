using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.Engine
{
    public interface IIncludeStrategy
    {
        bool ShouldIncludeCurrentNavigation(EagerLoadContext context);

        void FilterIncludePathsBeforeInclude(EagerLoadContext context);

        void ExecuteBeforeInclude(EagerLoadContext context);
    }
}
