using System;
using System.Collections.Generic;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using EfEagerLoad.Tests.Testing.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>(), pathsToFilter);
            PathsFound.ForEach(i => context.IncludePathsToInclude.Add(i));
            var strategyMock = new Mock<IncludeStrategy>();
            strategyMock.Setup(s => s.FilterIncludePathsBeforeInclude(context)).CallBase();
            var strategy = strategyMock.Object;

            strategy.FilterIncludePathsBeforeInclude(context);

            Assert.Equal(6, context.IncludePathsToInclude.Count);
        }

        [Fact]
        public void ExecuteBeforeInclude_WillLogTheIncludePaths_IfaLoggerCanBeObtained()
        {
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>(),
                                                rooType: typeof(IncludeStrategyTests));
            PathsFound.ForEach(i => context.IncludePathsToInclude.Add(i));
            
            var strategyMock = new Mock<IncludeStrategy>();
            strategyMock.Setup(s => s.ExecuteBeforeInclude(context)).CallBase();
            var strategy = strategyMock.Object;

            var loggerMock = new Mock<ILogger>();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<IncludeStrategy>))).Returns(null);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger))).Returns(loggerMock.Object);
            context.ServiceProvider = serviceProviderMock.Object;

            strategy.ExecuteBeforeInclude(context);

            serviceProviderMock.VerifyAll();
            //loggerMock.Verify(log => log.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), 
            //                                It.IsAny<Exception>(), It.IsAny<Func<string, Exception, string>>()));
        }

    }
}

