using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public override bool ShouldIncludeCurrentNavigation(EagerLoadContext context)
        {
            if (context.CurrentNavigation?.PropertyInfo == null) { return true; }

            

            var typeAddedList = GetTypesAdded(context);

            typeAddedList.Add(context.CurrentNavigation.GetNavigationType());

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
            var currentType = typeof(IEnumerable).IsAssignableFrom(context.CurrentNavigation?.ClrType) ? context.CurrentNavigation?.GetTargetType().ClrType :
                context.CurrentNavigation?.ClrType;

            if (currentType == null) { return false; }

            var typeAddedList = GetTypesAdded(context);

            // These need to be changes to examine path and not total count...
            if (currentType == context.RootType)
            {
                return !typeAddedList.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxRootTypeCount).Any();
            }

            return !typeAddedList.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxTypeCount).Any();
        }

        internal static IList<Type> GetTypesAdded(EagerLoadContext context)
        {
            if (!context.Bag.TryGetValue(EagerLoadContextBagKey.AllTypesVisitedOnBranch, out var typeAddedListObject))
            {
                typeAddedListObject = new List<Type>();
                context.Bag.Add(EagerLoadContextBagKey.AllTypesVisitedOnBranch, new List<Type>());
            }

            return (IList<Type>) typeAddedListObject;
        }

    }
}
