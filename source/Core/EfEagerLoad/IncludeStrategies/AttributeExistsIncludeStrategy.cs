using System;
using System.Collections.Concurrent;
using System.Reflection;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategies
{
    public class AttributeExistsIncludeStrategy<TAttribute> : AttributeExistsIncludeStrategy where TAttribute : Attribute
    {
        public AttributeExistsIncludeStrategy() : base(typeof(TAttribute))
        {
        }
    }

    public class AttributeExistsIncludeStrategy : IncludeStrategy
    {
        private static readonly ConcurrentDictionary<PropertyInfo, bool> AttributeCache = new ConcurrentDictionary<PropertyInfo, bool>();

        private readonly Type _attributeType;

        public AttributeExistsIncludeStrategy(Type attributeType)
        {
            _attributeType = attributeType;
        }

        public override bool ShouldIncludeCurrentNavigation(EagerLoadContext context)
        {
            return AttributeCache.GetOrAdd(context.CurrentNavigation.PropertyInfo, prop =>
                Attribute.IsDefined(context.CurrentNavigation.PropertyInfo, _attributeType));
        }
    }
}
