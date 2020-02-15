using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Engine
{
    internal class NavigationFinder
    {
        private static readonly ConcurrentDictionary<Type, INavigation[]> CachedTypeNavigations = new ConcurrentDictionary<Type, INavigation[]>();

        internal IEnumerable<INavigation> GetNavigationsForType(EagerLoadContext context, Type type)
        {
            return CachedTypeNavigations.GetOrAdd(type, typeToFind => context.DbContext.Model.FindEntityType(typeToFind).GetNavigations().ToArray());
        }
    }
}
