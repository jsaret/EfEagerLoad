using System;
using EfEagerLoad.Common;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EfEagerLoad.Tests.IncludeStrategies
{
    public class AllNavigationsIncludeStrategyTests
    {
        [Fact]
        public void ShouldEagerLoad_ForAll()
        {
            var strategy = new AllNavigationsIncludeStrategy();
            var context = new EagerLoadContext(new Mock<DbContext>().Object, strategy);

            Assert.True(strategy.ShouldIncludeNavigation(context, string.Empty));
        }
    }
}
