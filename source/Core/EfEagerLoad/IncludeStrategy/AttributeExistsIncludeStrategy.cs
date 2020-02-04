using System;

namespace EfEagerLoad.IncludeStrategy
{
    public class AttributeExistsIncludeStrategy<TAttribute> : IIncludeStrategy
    {
        public bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            return (context.CurrentNavigation?.PropertyInfo != null) && Attribute.IsDefined(context.CurrentNavigation.PropertyInfo, typeof(TAttribute));
        }
    }
}
