using System;
using EfEagerLoad.Attributes;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
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

        private static Mock<INavigation> SetupGenericNavigationMock()
        {
            var mock = new Mock<INavigation>();
            mock.Setup(nav => nav.Name).Returns(nameof(Book));
            mock.Setup(nav => nav.PropertyInfo)
                .Returns(typeof(EagerLoadAttributeIncludeStrategyTests).GetProperty("TestProperty"));
            return mock;
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenNavigationIsNotValid()
        {
            var strategy = new EagerLoadAttributeIncludeStrategy();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            context.SetCurrentNavigation(navigationMock.Object);

            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenAttributeDoesNotExistOnNavigation()
        {
            var strategy = new EagerLoadAttributeIncludeStrategy();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var navigationMock = SetupGenericNavigationMock();
            context.SetCurrentNavigation(navigationMock.Object);

            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldEagerLoad_WhenAttributeSetToAlways()
        {
            
            var strategy = new EagerLoadAttributeIncludeStrategy();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var navigationMock = SetupGenericNavigationMock();
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

            var navigationMock = SetupGenericNavigationMock();
            var navigation = navigationMock.Object;

            context.SetCurrentNavigation(navigation);
            var attribute = new EagerLoadAttribute(never: true);

            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(navigation, attribute);

            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenNavigationIsNotAllowedOnTheRootEntity()
        {
            var attribute = new EagerLoadAttribute(notOnRoot: true);

            var navigationHelperMock = new Mock<NavigationHelper>();
            navigationHelperMock.Setup(helper => helper.GetTypeForNavigation(It.IsAny<INavigation>()))
                .Returns(typeof(Book));

            var strategy = new EagerLoadAttributeIncludeStrategy(navigationHelperMock.Object);
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var firstNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(firstNavigationMock.Object, attribute);

            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));

            var secondNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(secondNavigationMock.Object, new EagerLoadAttribute());

            context.SetCurrentNavigation(secondNavigationMock.Object);

            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenNavigationIOnlyAllowedOnTheRootEntity()
        {
            var attribute = new EagerLoadAttribute(onlyOnRoot: true);

            var navigationHelperMock = new Mock<NavigationHelper>();
            navigationHelperMock.Setup(helper => helper.GetTypeForNavigation(It.IsAny<INavigation>()))
                .Returns(typeof(Book));

            var strategy = new EagerLoadAttributeIncludeStrategy(navigationHelperMock.Object);
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var firstNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(firstNavigationMock.Object, attribute);

            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));

            var secondNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(secondNavigationMock.Object, new EagerLoadAttribute());

            context.SetCurrentNavigation(secondNavigationMock.Object);

            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }

        

        [Fact]
        public void ShouldNotEagerLoad_WhenOverTheNavigationsMaxDepthPosition()
        {
            var attribute = new EagerLoadAttribute(maxDepthPosition: 1);

            var navigationHelperMock = new Mock<NavigationHelper>();
            navigationHelperMock.Setup(helper => helper.GetTypeForNavigation(It.IsAny<INavigation>()))
                .Returns(typeof(Book));

            var strategy = new EagerLoadAttributeIncludeStrategy(navigationHelperMock.Object);
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var firstNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(firstNavigationMock.Object, attribute);

            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));

            var secondNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(secondNavigationMock.Object, attribute);

            context.SetCurrentNavigation(secondNavigationMock.Object);
            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenNavigationTypeIsTheRootEntityType()
        {
            var attribute = new EagerLoadAttribute(notIfRootType: true);

            var navigationHelperMock = new Mock<NavigationHelper>();
            navigationHelperMock.Setup(helper => helper.GetTypeForNavigation(It.IsAny<INavigation>()))
                .Returns(typeof(Book));

            var strategy = new EagerLoadAttributeIncludeStrategy(navigationHelperMock.Object);
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy, rootType: typeof(Book));

            var firstNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(firstNavigationMock.Object, attribute);

            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));

            context.RootType = typeof(Author);
            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenOverTheRootNavigationsMaxDepth()
        {
            var rootAttribute = new EagerLoadAttribute(maxDepth: 2);
            var attribute = new EagerLoadAttribute();

            var navigationHelperMock = new Mock<NavigationHelper>();
            navigationHelperMock.Setup(helper => helper.GetTypeForNavigation(It.IsAny<INavigation>()))
                .Returns(typeof(Book));

            var strategy = new EagerLoadAttributeIncludeStrategy(navigationHelperMock.Object);
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy);

            var firstNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(firstNavigationMock.Object, rootAttribute);
            
            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));

            var secondNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(secondNavigationMock.Object, attribute);

            context.SetCurrentNavigation(secondNavigationMock.Object);
            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));

            var thirdNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(thirdNavigationMock.Object, attribute);

            context.SetCurrentNavigation(thirdNavigationMock.Object);
            Assert.False(strategy.ShouldIncludeCurrentNavigation(context));
        }

        [Fact]
        public void ShouldNotEagerLoad_WhenOverTheRootTypeCount()
        {
            var attribute = new EagerLoadAttribute(maxRootTypeCount: 1);

            var navigationHelperMock = new Mock<NavigationHelper>();
            navigationHelperMock.Setup(helper => helper.GetTypeForNavigation(It.IsAny<INavigation>()))
                .Returns(typeof(Book));

            var strategy = new EagerLoadAttributeIncludeStrategy(navigationHelperMock.Object);
            var context = new EagerLoadContext(Mock.Of<DbContext>(), strategy, rootType: typeof(Book));

            var firstNavigationMock = SetupGenericNavigationMock();
            EagerLoadAttributeIncludeStrategy.EagerLoadAttributeCache.TryAdd(firstNavigationMock.Object, attribute);

            context.SetCurrentNavigation(firstNavigationMock.Object);
            Assert.True(strategy.ShouldIncludeCurrentNavigation(context));
        }


    }
}
