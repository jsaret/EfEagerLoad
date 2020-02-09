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

        public override bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            if (context.CurrentNavigation?.PropertyInfo == null) { return true; }

            //Find EagerLoad Attributes
            var attribute = EagerLoadAttributeCache.GetOrAdd(context.CurrentNavigation.PropertyInfo, property => 
                                                                    Attribute.GetCustomAttributes(property, typeof(EagerLoadAttribute))
                                                                    .OfType<EagerLoadAttribute>().FirstOrDefault());

            // No EagerLoad Attribute
            if (attribute == null) { return false; }

            //On Root Type vs Others
            if (context.NavigationPath.Count() == 1 && ShouldNotEagerLoadOffRoot(context, attribute)) { return false; }

            if (context.NavigationPath.Skip(1).Any() && ShouldOnlyEagerLoadOffRootType(context, attribute)) { return false; }

            // Max Depth
            if (!CanTypeBeLazyLoadedBasedOnAllowedDepthLimit(context, attribute)) { return false; }
            //


            //if (context.CurrentNavigation == null) { return true; }

            return true;
        }

        internal static bool ShouldNotEagerLoadOffRoot(EagerLoadContext context, EagerLoadAttribute attribute)
        {
            return attribute.NotOnRoot && context.CurrentNavigation.DeclaringType.ClrType == context.RootType;
        }

        internal static bool ShouldOnlyEagerLoadOffRootType(EagerLoadContext context, EagerLoadAttribute attribute)
        {
            return attribute.NotOnRoot && context.CurrentNavigation.DeclaringType.ClrType == context.RootType;
        }
        
        internal static bool CanTypeBeLazyLoadedBasedOnAllowedDepthLimit(EagerLoadContext context, EagerLoadAttribute eagerLoadAttribute)
        {
            if (context.TypesVisited.Count() == 1) { return true; }

            var currentType = typeof(IEnumerable).IsAssignableFrom(context.CurrentNavigation?.ClrType) ? context.CurrentNavigation?.GetTargetType().ClrType :
                context.CurrentNavigation?.ClrType;

            if (currentType == null) { return false; }

            // These need to be changes to examine path and not total count...
            if (currentType == context.RootType)
            {
                return !context.TypesVisited.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxRootTypeCount).Any();
            }

            return !context.TypesVisited.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxTypeCount).Any();
        }

    }
}
