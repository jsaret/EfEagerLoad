using System;
using System.Collections;
using System.Linq;
using EfEagerLoad.Attributes;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Internal
{
    internal static class EagerLoadAttributeFunctions
    {
        public static Predicate<EfEagerLoadContext> PredicateForEagerLoadAttribute = (context) =>
            {
                if (context.CurrentNavigation == null) { return true; }

                var eagerLoadAttribute = Attribute.GetCustomAttributes(context.CurrentNavigation.PropertyInfo, typeof(EagerLoadAttribute))
                                                                            .OfType<EagerLoadAttribute>().FirstOrDefault();

                if (eagerLoadAttribute == null) { return false; }

                if (eagerLoadAttribute.OnlyIfOnRoot && context.CurrentNavigation.DeclaringType.ClrType != context.RootType) { return false; }

                if (eagerLoadAttribute.NotIfOnRoot && context.CurrentNavigation.DeclaringType.ClrType == context.RootType) { return false; }

                if (!CanTypeBeLazyLoadedBasedOnAllowedLimit(context, eagerLoadAttribute)) { return false; }
                
                return true;
            };

        private static bool CanTypeBeLazyLoadedBasedOnAllowedLimit(EfEagerLoadContext context, EagerLoadAttribute eagerLoadAttribute)
        {
            if (context.TypesVisited.Count == 1) { return true; }

            var currentType = typeof(IEnumerable).IsAssignableFrom(context.CurrentNavigation?.ClrType) ? context.CurrentNavigation?.GetTargetType().ClrType :
                                                                context.CurrentNavigation?.ClrType;

            if (currentType == null) { return false; }

            if (currentType == context.RootType)
            {
                return !context.TypesVisited.Where(type => type == currentType).Skip(eagerLoadAttribute.AllowedVisitsForRootType).Any();
            }

            return !context.TypesVisited.Where(type => type == currentType).Skip(eagerLoadAttribute.AllowedVisitsForType).Any();
        }
    }
}
