using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.Tests.Testing;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EfEagerLoad.Tests.Engine
{
    public class NavigationFinderTests
    {

        [Fact]
        public void ShouldReturnTheCorrectNavigations_ForMappedTypes()
        {
            var context = new EagerLoadContext(TestDbContext.Instance, new Mock<IIncludeStrategy>().Object);

            var expectedNavigations = TestDbContext.Instance.Model.FindEntityType(typeof(Book)).GetNavigations().ToList();

            var navigationFinder = new NavigationFinder();

            var actualNavigations = navigationFinder.GetNavigationsForType(context, typeof(Book)).ToList();

            Assert.Equal(expectedNavigations.Count, actualNavigations.Count);
            expectedNavigations.ForEach(nav => { Assert.Contains(nav, actualNavigations); });
        }

    }
}
