using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using EfEagerLoad.Attributes;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.IncludeStrategies
{
    public class EagerLoadAttributeIncludeStrategy : IncludeStrategy
    {
        private static readonly NavigationHelper CachedNavigationHelper = new NavigationHelper();
        internal static readonly ConcurrentDictionary<INavigation, EagerLoadAttribute> EagerLoadAttributeCache = new ConcurrentDictionary<INavigation, EagerLoadAttribute>();

        private readonly NavigationHelper _navigationHelper;

        public EagerLoadAttributeIncludeStrategy() : this(CachedNavigationHelper) { }

        public EagerLoadAttributeIncludeStrategy(NavigationHelper navigationHelper)
        {
            _navigationHelper = navigationHelper;
        }

        public override bool ShouldIncludeCurrentNavigation(EagerLoadContext context)
        {
            if (context.CurrentNavigation?.PropertyInfo == null) { return false; }

            // Find EagerLoad Attribute
            var attribute = GetAttributeForNavigation(context);

            // No EagerLoad Attribute
            if (attribute == null) { return false; }

            // Always
            if (attribute.Always) { return true; }

            // Never
            if (attribute.Never) { return false; }

            // NotOnRoot
            if (attribute.NotOnRoot && context.NavigationPath.Count == 1) { return false; }

            // OnlyOnRoot
            if (attribute.OnlyOnRoot && context.NavigationPath.Count > 1) { return false; }

            // MaxDepthPosition
            if (attribute.MaxDepthPosition < context.NavigationPath.Count) { return false; }

            // Get Current NavigationType
            var currentNavigationType = _navigationHelper.GetTypeForNavigation(context.CurrentNavigation);

            // NotIfRootType
            if (attribute.NotIfRootType && currentNavigationType == context.RootType) { return false; }

            // Get Root Navigation
            var rootNavigation = context.NavigationPath.LastOrDefault();

            // MaxDepth - needs to be off the Root Navigation
            if (rootNavigation != null)
            {
                if (EagerLoadAttributeCache[rootNavigation].MaxDepth < context.NavigationPath.Count) { return false; }
            }

            // NotIfRootNavigationType
            if (attribute.NotIfRootNavigationType && context.NavigationPath.Count > 0)
            {
                if (_navigationHelper.GetTypeForNavigation(rootNavigation) == currentNavigationType) { return false; }
            }

            // NotIfParentsParentType
            if (attribute.NotIfParentsParentType && context.NavigationPath.Count > 2)
            {
                var parentsParentNavigation = context.NavigationPath.ElementAtOrDefault(2);
                if(_navigationHelper.GetTypeForNavigation(parentsParentNavigation) == currentNavigationType) { return false; }
            }

            // MaxRootTypeCount && MaxTypeCount
            if (DoesNavigationGoOverTheMaxTypeLimits(context, attribute, currentNavigationType)) { return false; }

            return true;
        }

        private bool DoesNavigationGoOverTheMaxTypeLimits(EagerLoadContext context, EagerLoadAttribute attribute,
            Type currentNavigationType)
        {
            if (context.NavigationPath.Count <= attribute.MaxRootTypeCount && context.NavigationPath.Count <= attribute.MaxTypeCount)
            {
                return false;
            }
            
            var rootCount = 0;
            var typeCount = 0;
            foreach (var nav in context.NavigationPath)
            {
                var type = _navigationHelper.GetTypeForNavigation(nav);
                if (type == context.RootType)
                {
                    rootCount++;
                }

                if (type == currentNavigationType)
                {
                    typeCount++;
                }

                if (rootCount > attribute.MaxRootTypeCount) { return true; }

                if (typeCount > attribute.MaxTypeCount) { return true; }
            }

            return false;
        }

        private static EagerLoadAttribute GetAttributeForNavigation(EagerLoadContext context)
        {
            var attribute = EagerLoadAttributeCache.GetOrAdd(context.CurrentNavigation, nav =>
                Attribute.GetCustomAttributes(nav.PropertyInfo, typeof(EagerLoadAttribute))
                    .OfType<EagerLoadAttribute>().FirstOrDefault());
            return attribute;
        }
    }
}




