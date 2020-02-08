using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using EfEagerLoad.Attributes;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

using Moq;

namespace EfEagerLoad.Tests.IncludeStrategies
{
    public class AttributeExistsIncludeStrategyTests
    {
        public object NoAttributes { get; set; }

        [MaxLength, Required]
        public object NoEagerLoadAttributes { get; set; }

        [EagerLoad]
        public object HasEagerLoadAttribute { get; set; }

        [MaxLength, Required, EagerLoad]
        public object HasManyIncludingEagerLoadAttribute { get; set; }

        [Fact]
        public void ShouldNotEagerLoad_WhenAttributeDoesNotExist()
        {
            var strategy = new AttributeExistsIncludeStrategy<EagerLoadAttribute>();
            var context = new EagerLoadContext(new Mock<DbContext>().Object, strategy);
            var navigationMock = new Mock<INavigation>();
            context.SetCurrentNavigation(navigationMock.Object);

            navigationMock.Setup(nav => nav.PropertyInfo)
                .Returns(GetType().GetProperty(nameof(NoAttributes)));
            Assert.False(strategy.ShouldIncludeNavigation(context, string.Empty));

            navigationMock.Setup(nav => nav.PropertyInfo)
                .Returns(GetType().GetProperty(nameof(NoEagerLoadAttributes)));
            Assert.False(strategy.ShouldIncludeNavigation(context, string.Empty));
        }

        [Fact]
        public void ShouldEagerLoad_WhenAttributeExist()
        {
            var strategy = new AttributeExistsIncludeStrategy<EagerLoadAttribute>();
            var context = new EagerLoadContext(new Mock<DbContext>().Object, strategy);
            var navigationMock = new Mock<INavigation>();
            context.SetCurrentNavigation(navigationMock.Object);

            navigationMock.Setup(nav => nav.PropertyInfo)
                .Returns(GetType().GetProperty(nameof(HasEagerLoadAttribute)));
            Assert.True(strategy.ShouldIncludeNavigation(context, string.Empty));

            navigationMock.Setup(nav => nav.PropertyInfo)
                .Returns(GetType().GetProperty(nameof(HasManyIncludingEagerLoadAttribute)));
            Assert.True(strategy.ShouldIncludeNavigation(context, string.Empty));
        }

    }
}
