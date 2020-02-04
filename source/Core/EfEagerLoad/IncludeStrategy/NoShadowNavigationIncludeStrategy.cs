using System;
using System.Collections.Generic;
using System.Text;

namespace EfEagerLoad.IncludeStrategy
{
    public class NoShadowNavigationIncludeStrategy : IIncludeStrategy
    {
        public bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            throw new NotImplementedException();
        }
    }
}
