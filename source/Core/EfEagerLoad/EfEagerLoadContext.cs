using System;
using System.Collections.Generic;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad
{
    public class EfEagerLoadContext
    {
        public EfEagerLoadContext(Type rootType, DbContext dbContext, IList<string> navigationPropertiesToIgnore,
                                Func<EfEagerLoadContext, Type, bool> shouldIncludeTypePredicate, 
                                Func<EfEagerLoadContext, INavigation, bool> shouldIncludeNavigationPredicate)
        {
            Guard.IsNotNull(nameof(rootType), rootType);
            Guard.IsNotNull(nameof(dbContext), dbContext); 
            Guard.IsNotNull(nameof(shouldIncludeTypePredicate), shouldIncludeTypePredicate);
            Guard.IsNotNull(nameof(shouldIncludeNavigationPredicate), shouldIncludeNavigationPredicate);

            RootType = rootType;
            DbContext = dbContext;
            NavigationPropertiesToIgnore = navigationPropertiesToIgnore ?? new List<string>();
            ShouldIncludeTypePredicate = shouldIncludeTypePredicate;
            ShouldIncludeNavigationPredicate = shouldIncludeNavigationPredicate;
        }

        public Type RootType { get; set; }

        public DbContext DbContext { get; set; }

        public IList<string> NavigationPropertiesToIgnore { get; set; }

        public IList<Type> TypesVisited { get; } = new List<Type>();

        public Func<EfEagerLoadContext, Type, bool> ShouldIncludeTypePredicate { get; set; }

        public Func<EfEagerLoadContext, INavigation, bool> ShouldIncludeNavigationPredicate { get; set; }
    }
}
