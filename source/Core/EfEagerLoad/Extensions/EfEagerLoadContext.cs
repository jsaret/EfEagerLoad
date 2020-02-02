using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Extensions
{
    public class EfEagerLoadContext
    {
        public DbContext DbContext { get; set; }

        public IList<string> NavigationPropertiesToIgnore { get; set; } = new List<string>();

        public IList<Type> TypesVisited { get; set; } = new List<Type>();

        public Predicate<INavigation> NavigationsToIncludePredicate { get; set; }
    }
}
