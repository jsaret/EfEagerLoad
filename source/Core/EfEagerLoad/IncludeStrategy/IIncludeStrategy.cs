using System;

namespace EfEagerLoad.IncludeStrategy
{
    public interface IIncludeStrategy
    {
        bool ShouldIncludeNavigation(EagerLoadContext context);

        bool ShouldIgnoreNavigationPath(EagerLoadContext context, string path);

        void PreBuildExecute(EagerLoadContext context);
    }
}
