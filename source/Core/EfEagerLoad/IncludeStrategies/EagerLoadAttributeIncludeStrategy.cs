using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using EfEagerLoad.Attributes;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.IncludeStrategies
{
    public class EagerLoadAttributeIncludeStrategy : IncludeStrategy
    {
        private static readonly ConcurrentDictionary<PropertyInfo, EagerLoadAttribute> EagerLoadAttributeCache = new ConcurrentDictionary<PropertyInfo, EagerLoadAttribute>();

        public override bool ShouldIncludeNavigation(EagerLoadContext context, string navigationPath)
        {
            if (context.CurrentNavigation == null) { return true; }

            var eagerLoadAttribute = EagerLoadAttributeCache.GetOrAdd(context.CurrentNavigation?.PropertyInfo, property => 
                                                                    Attribute.GetCustomAttributes(property, typeof(EagerLoadAttribute))
                                                                    .OfType<EagerLoadAttribute>().FirstOrDefault());


            if (eagerLoadAttribute == null) { return false; }

            if (eagerLoadAttribute.OnlyIfOnRoot && context.CurrentNavigation.DeclaringType.ClrType != context.RootType) { return false; }

            if (eagerLoadAttribute.NotIfOnRoot && context.CurrentNavigation.DeclaringType.ClrType == context.RootType) { return false; }

            if (!CanTypeBeLazyLoadedBasedOnAllowedLimit(context, eagerLoadAttribute)) { return false; }

            return true;
        }

        private static bool CanTypeBeLazyLoadedBasedOnAllowedLimit(EagerLoadContext context, EagerLoadAttribute eagerLoadAttribute)
        {
            if (context.TypesVisited.Count() == 1) { return true; }

            var currentType = typeof(IEnumerable).IsAssignableFrom(context.CurrentNavigation?.ClrType) ? context.CurrentNavigation?.GetTargetType().ClrType :
                context.CurrentNavigation?.ClrType;

            if (currentType == null) { return false; }

            if (currentType == context.RootType)
            {
                return !context.TypesVisited.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxVisitsForRootType).Any();
            }

            return !context.TypesVisited.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxVisitsForType).Any();
        }

    }
}
