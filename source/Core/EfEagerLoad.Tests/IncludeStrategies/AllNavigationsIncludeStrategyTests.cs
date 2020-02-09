using System;
using EfEagerLoad.Common;
using EfEagerLoad.IncludeStrategies;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using Xunit;

namespace EfEagerLoad.Tests.IncludeStrategies
{
    public class AllNavigationsIncludeStrategyTests
    {
        [Fact]
        public void ShouldEagerLoad_ForAllOffRoot()
        {
            var strategy = new AllNavigationsIncludeStrategy();
            var context = new EagerLoadContext(new Mock<DbContext>().Object, strategy);
            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            context.SetCurrentNavigation(navigationMock.Object);

            Assert.True(strategy.ShouldIncludeNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_IfIncludePathToConsiderShouldBeIgnored()
        {
            var strategy = new AllNavigationsIncludeStrategy();
            var context = new EagerLoadContext(new Mock<DbContext>().Object, strategy);
            context.IncludePathsToIgnore.Add(nameof(INavigation));
            //context.ParentIncludePath = "";
            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(INavigation));
            context.SetCurrentNavigation(navigationMock.Object);

            Assert.False(strategy.ShouldIncludeNavigation(context));
        }
    }
}
