using System;
using System.Collections.Concurrent;
using System.Linq;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Engine
{
    internal class NavigationFinder
    {
        private static readonly ConcurrentDictionary<Type, INavigation[]> CachedTypeNavigations = new ConcurrentDictionary<Type, INavigation[]>();

        internal INavigation[] GetNavigationsForType(EagerLoadContext context, Type type)
        {
            return CachedTypeNavigations.ContainsKey(type) ? CachedTypeNavigations[type] : 
                CachedTypeNavigations.GetOrAdd(type, GetNavigationsForType);

            INavigation[] GetNavigationsForType(Type typeToFind)
            {
                return context.DbContext.Model.FindEntityType(typeToFind).GetNavigations().ToArray();
            }
        }
    }
}
