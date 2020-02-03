using System;
using System.Linq;
using EfEagerLoad.Attributes;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Internal
{
    internal static class EagerLoadAttributeFunctions
    {
        public static Func<EfEagerLoadContext, INavigation, bool> PredicateForEagerLoadAttribute = (context, navigation) =>
            {
                var eagerLoadAttribute = Attribute.GetCustomAttributes(navigation.PropertyInfo, typeof(EagerLoadAttribute))
                                                .OfType<EagerLoadAttribute>().FirstOrDefault();

                if (eagerLoadAttribute == null)  { return false; }

                if (eagerLoadAttribute.OnlyIfOnRoot && navigation.DeclaringType.ClrType != context.RootType) { return false; }

                if (eagerLoadAttribute.NotIfOnRoot && navigation.DeclaringType.ClrType == context.RootType) { return false; }

                return true;
            };
        
    }
}
