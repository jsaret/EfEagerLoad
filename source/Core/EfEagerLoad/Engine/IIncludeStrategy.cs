using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.Engine
{
    public interface IIncludeStrategy
    {
        bool ShouldIncludeNavigation(EagerLoadContext context);

        bool ShouldIgnoreNavigationPath(EagerLoadContext context, string path);

        void PreBuildExecute(EagerLoadContext context);
    }
}
