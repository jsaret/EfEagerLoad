using System;
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
    public class EagerLoadAttributeIncludeStrategyTests
    {
        public EagerLoadAttributeIncludeStrategyTests()
        {
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.Clear();
        }

        public object TestProperty { get; set; }

        [Fact]
        public void ShouldNotEagerLoad_WhenAttributeDoesNotExistOnNavigation()
        {
            var strategy = new EagerLoadAttributeIncludeStrategy();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            navigationMock.Setup(nav => nav.PropertyInfo)
                .Returns(typeof(EagerLoadAttributeIncludeStrategyTests).GetProperty("TestProperty"));
            context.SetCurrentNavigation(navigationMock.Object);

            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldEagerLoad_WhenAttributeSetToAlways()
        {
            
            var strategy = new EagerLoadAttributeIncludeStrategy();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            navigationMock.Setup(nav => nav.PropertyInfo)
                .Returns(typeof(EagerLoadAttributeIncludeStrategyTests).GetProperty("TestProperty"));
            var navigation = navigationMock.Object;

            context.SetCurrentNavigation(navigation);
            var attribute = new EagerLoadAttribute(always: true);

            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(navigation, attribute);

            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenAttributeSetToNever()
        {

            var strategy = new EagerLoadAttributeIncludeStrategy();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            navigationMock.Setup(nav => nav.PropertyInfo)
                .Returns(typeof(EagerLoadAttributeIncludeStrategyTests).GetProperty("TestProperty"));
            var navigation = navigationMock.Object;

            context.SetCurrentNavigation(navigation);
            var attribute = new EagerLoadAttribute(never: true);

            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(navigation, attribute);

            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }
    }
}
