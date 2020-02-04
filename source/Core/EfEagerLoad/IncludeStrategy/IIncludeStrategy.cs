using System;

namespace EfEagerLoad.IncludeStrategy
{
    public interface IIncludeStrategy
    {
        bool ShouldIncludeNavigation(EagerLoadContext context);
    }
}
