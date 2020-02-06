using System;
using System.Collections.Concurrent;
using System.Reflection;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategies
{
    public class AttributeExistsIncludeStrategy<TAttribute> : IncludeStrategy where TAttribute : Attribute
    {
        private static readonly ConcurrentDictionary<PropertyInfo, bool> AttributeCache = new ConcurrentDictionary<PropertyInfo, bool>();

        public override bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            if (context.CurrentNavigation?.PropertyInfo == null)
            {
                return false;
            }

            return AttributeCache.GetOrAdd(context.CurrentNavigation.PropertyInfo, prop =>
                Attribute.IsDefined(context.CurrentNavigation.PropertyInfo, typeof(TAttribute)));
        }
    }
}
