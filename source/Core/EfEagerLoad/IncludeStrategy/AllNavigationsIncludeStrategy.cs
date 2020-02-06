using System;
using System.Collections.Generic;
using System.Text;

namespace EfEagerLoad.IncludeStrategy
{
    public class AllNavigationsIncludeStrategy : IncludeStrategy
    {
        public override bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            return true;
        }
    }
}
