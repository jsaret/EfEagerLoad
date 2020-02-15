using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Tests.Testing;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfEagerLoad.Tests.Common
{
    public class NavigationTypeExtensionsTests
    {
        private static readonly TestDbContext TestingDbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        [Fact]
        public void ShouldReturnNavigationClassType_WhenNavigationIsNotCollection()
        {
            var navigation = TestingDbContext.Model.FindEntityType(typeof(Book)).GetNavigations()
                .First(nav => nav.Name == nameof(Book.Publisher));

            Assert.Equal(typeof(Publisher), navigation.GetNavigationType());
        }

        [Fact]
        public void ShouldReturnNavigationCollectionType_WhenNavigationIsCollection()
        {
            var navigation = TestingDbContext.Model.FindEntityType(typeof(Author)).GetNavigations()
                .First(nav => nav.Name == nameof(Author.Books));

            Assert.Equal(typeof(Book), navigation.GetNavigationType());
        }
    }
}
