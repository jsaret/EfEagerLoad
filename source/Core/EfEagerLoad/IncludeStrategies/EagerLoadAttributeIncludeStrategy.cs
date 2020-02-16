using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using EfEagerLoad.Attributes;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.IncludeStrategies
{
    public class EagerLoadAttributeIncludeStrategy : IncludeStrategy
    {
        internal static readonly ConcurrentDictionary<INavigation, EagerLoadAttribute> EagerLoadAttributeCache = new ConcurrentDictionary<INavigation, EagerLoadAttribute>();

        public override bool ShouldIncludeCurrentNavigation(EagerLoadContext context)
        {
            if (context.CurrentNavigation?.PropertyInfo == null) { return false; }

            //Find EagerLoad Attributes
            var attribute = EagerLoadAttributeCache.GetOrAdd(context.CurrentNavigation, nav => 
                    Attribute.GetCustomAttributes(nav.PropertyInfo, typeof(EagerLoadAttribute))
                                                                    .OfType<EagerLoadAttribute>().FirstOrDefault());

            // No EagerLoad Attribute
            if (attribute == null) { return false; }

            //Always
            if (attribute.Always) { return true; }

            //Never
            if (attribute.Never) { return false; }

            //MaxDepth
            if (attribute.MaxDepth < context.NavigationPath.Count) { return false; }

            //NotOnRoot
            if (context.NavigationPath.Count == 1 && attribute.NotOnRoot && 
                context.CurrentNavigation.GetNavigationType() == context.RootType) { return false; }

            //OnlyOnRoot
            if (context.NavigationPath.Count > 1 && attribute.OnlyOnRoot && 
                context.CurrentNavigation.GetNavigationType() == context.RootType) { return false; }

            //NotIfRootType
            if (context.CurrentNavigation.GetNavigationType() == context.RootType) { return false; }

            // Max Path Depth
            //if (attribute.NotIfRootType CanTypeBeLazyLoadedBasedOnAllowedDepthLimit(context, attribute)) { return false; }
            //


            //if (context.CurrentNavigation == null) { return true; }

            return true;
        }

        
        internal static bool CanTypeBeLazyLoadedBasedOnAllowedDepthLimit(EagerLoadContext context, EagerLoadAttribute eagerLoadAttribute)
        {
            //if ()
            //var firstRootNavigation = 
            var currentType = typeof(IEnumerable).IsAssignableFrom(context.CurrentNavigation?.ClrType) ? context.CurrentNavigation?.GetTargetType().ClrType :
                context.CurrentNavigation?.ClrType;

            if (currentType == null) { return false; }

            return true;
            // These need to be changes to examine path and not total count...
            //if (currentType == context.RootType)
            //{
            //    return !typeAddedList.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxRootTypeCount).Any();
            //}

            //return !typeAddedList.Where(type => type == currentType).Skip(eagerLoadAttribute.MaxTypeCount).Any();
        }

    }
}



/*

--Always = always;
--Never = never;
--OnlyOnRoot = onlyOnRoot;
--NotOnRoot = notOnRoot;
NotIfParentsParentType = notIfParentsParentType;
--NotIfRootType = notIfRootType;
--MaxDepth = maxDepth;
MaxDepthPosition = maxDepthPosition;
MaxRootTypeCount = maxRootTypeCount;
MaxTypeCount = maxTypeCount;

*/
