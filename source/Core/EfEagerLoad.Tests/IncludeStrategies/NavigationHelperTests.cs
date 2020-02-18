using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EfEagerLoad.Common;
using EfEagerLoad.IncludeStrategies;
using EfEagerLoad.Tests.Testing;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfEagerLoad.Tests.IncludeStrategies
{
    public class NavigationHelperTests
    {

        private static readonly TestDbContext TestingDbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        [Fact]
        public void ShouldReturnNavigationClassType_WhenNavigationIsNotCollection()
        {
            var navigation = TestingDbContext.Model.FindEntityType(typeof(Book)).GetNavigations()
                .First(nav => nav.Name == nameof(Book.Publisher));

            var navigationHelper = new NavigationHelper();


            Assert.Equal(typeof(Publisher), navigationHelper.GetTypeForNavigation(navigation));
        }

    }
}
