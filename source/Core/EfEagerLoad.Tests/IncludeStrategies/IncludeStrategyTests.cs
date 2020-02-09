using System;
using System.Collections.Generic;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using EfEagerLoad.Tests.Testing.Extensions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EfEagerLoad.Tests.IncludeStrategies
{
    public class IncludeStrategyTests
    {
        private static readonly IList<string> PathsFound = new[]
            {
                "Book.Test.Author", "Book.Test.Author.Books", "Book.Author.Books.Category",
                "Book.Test.Books", "Book.Test.Author", 
                "Book.Author", "Book.Author.Books.Author", "Books.Category", "Books.Publisher"
            };

        [Fact]
        public void FilterIncludePathsBeforeInclude_RemovesPathsThatMatchTheExcludeList()
        {
            var pathsToFilter = new[] { "Book.Author" };
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object, pathsToFilter);
            PathsFound.ForEach(i => context.IncludePathsToInclude.Add(i));
            var strategyMock = new Mock<IncludeStrategy>();
            strategyMock.Setup(s => s.FilterIncludePathsBeforeInclude(context)).CallBase();
            var strategy = strategyMock.Object;

            strategy.FilterIncludePathsBeforeInclude(context);

            Assert.Equal(6, context.IncludePathsToInclude.Count);
        }
    }
}
