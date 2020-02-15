using System;
using System.Collections.Generic;
using System.Text;
using EfEagerLoad.Attributes;
using EfEagerLoad.Common;
using EfEagerLoad.IncludeStrategies;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using Xunit;

namespace EfEagerLoad.Tests.IncludeStrategies
{
    public class PredicateIncludeStrategyTests
    {

        [Fact]
        public void ShouldUseSuppliedContextPredicate_ToTestIfTheNavigationShouldBeIncluded()
        {
            var testCount = 0;
            var strategy = new PredicateIncludeStrategy((EagerLoadContext contextInput) =>
            {
                var result = (testCount == 1);
                testCount++;
                return result;
            });

            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);
            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));

            context.SetCurrentNavigation(navigationMock.Object);

            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));

            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));

            Assert.Equal(2, testCount);
        }

        [Fact]
        public void ShouldUseSuppliedPathPredicate_ToTestIfTheNavigationShouldBeIncluded()
        {
            var pathChecked = string.Empty;
            var strategy = new PredicateIncludeStrategy(path =>
            {
                pathChecked = path;
                return true;
            });

            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);
            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));

            context.SetCurrentNavigation(navigationMock.Object);

            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));

            Assert.Equal(nameof(Book), pathChecked);
        }

    }
}
