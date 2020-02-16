using System;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EfEagerLoad.Tests.Engine
{
    public class IncludeEngineTests : IDisposable
    {

        public IncludeEngineTests()
        {
            EagerLoadContext.SkipEntityFrameworkCheckForTesting = true;
            IncludeEngine.CachedIncludePaths.Clear();
        }

        public void Dispose()
        {
            EagerLoadContext.SkipEntityFrameworkCheckForTesting = false;
        }

        [Fact]
        public void ShouldReturnOriginalQuery_WithNoExecution_WhenIncludeExecutionIsSkip()
        {
            var includeFinderMock = new Mock<IncludeFinder>();
            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>(), includeExecution: IncludeExecution.Skip);


            Assert.Equal(query, engine.RunIncludesForType(query, context));

            includeFinderMock.VerifyNoOtherCalls();
            queryIncluderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldSetRootType_IfNoRootTypeOnTheContext()
        {
            var includeFinderMock = new Mock<IncludeFinder>();
            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>());
            
            engine.RunIncludesForType(query, context);

            Assert.Equal(typeof(Book), context.RootType);
        }

        [Fact]
        public void ShouldReturnOriginalQuery_IfNoIncludePathsGetFound()
        {
            var includeFinderMock = new Mock<IncludeFinder>();
            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>());

            Assert.Equal(query, engine.RunIncludesForType(query, context));

            queryIncluderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldSetFinderResults_IntoTheContext()
        {
            var expectedIncludes = new List<string>() { "Test", "Cache" };

            var includeFinderMock = new Mock<IncludeFinder>();
            includeFinderMock.Setup(finder => finder.BuildIncludePathsForRootType(It.IsAny<EagerLoadContext>()))
                .Returns(expectedIncludes);

            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>());

            engine.RunIncludesForType(query, context);

            Assert.Equal(expectedIncludes, context.IncludePathsToInclude);
        }

        [Fact]
        public void WhenUsingCachedResult_TheCacheShouldBeSetAndUsedOnFurtherCalls()
        {
            var expectedIncludes = new List<string>() { "Test", "Cache" };

            var includeFinderMock = new Mock<IncludeFinder>();
            includeFinderMock.Setup(finder => finder.BuildIncludePathsForRootType(It.IsAny<EagerLoadContext>()))
                .Returns(expectedIncludes);

            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>());

            engine.RunIncludesForType(query, context);
            engine.RunIncludesForType(query, context);

            Assert.Single(IncludeEngine.CachedIncludePaths);

            includeFinderMock.Verify(finder => finder.BuildIncludePathsForRootType(It.IsAny<EagerLoadContext>()), Times.Once);
        }

        [Fact]
        public void WhenUsingNoCachedResult_TheCacheShouldBeNotBeSet()
        {
            var expectedIncludes = new List<string>() { "Test", "Cache" };

            var includeFinderMock = new Mock<IncludeFinder>();
            includeFinderMock.Setup(finder => finder.BuildIncludePathsForRootType(It.IsAny<EagerLoadContext>()))
                .Returns(expectedIncludes);

            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>(), 
                includeExecution: IncludeExecution.NoCache);

            engine.RunIncludesForType(query, context);

            Assert.Empty(IncludeEngine.CachedIncludePaths);

            Assert.Equal(expectedIncludes, context.IncludePathsToInclude);
        }

        [Fact]
        public void WhenUsingReadOnlyCachedResult_TheCacheShouldBeNotBeSet()
        {
            var expectedIncludes = new List<string>() { "Test", "Cache" };

            var includeFinderMock = new Mock<IncludeFinder>();
            includeFinderMock.Setup(finder => finder.BuildIncludePathsForRootType(It.IsAny<EagerLoadContext>()))
                .Returns(expectedIncludes);

            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>(), 
                includeExecution: IncludeExecution.ReadOnlyCache);

            engine.RunIncludesForType(query, context);

            Assert.Empty(IncludeEngine.CachedIncludePaths);

            Assert.Equal(expectedIncludes, context.IncludePathsToInclude);
        }

        [Fact]
        public void WhenUsingReadOnlyCachedResult_IfTheCacheIsSet_ThenShouldGetCacheResults()
        {
            var expectedIncludes = new List<string>() { "Test", "Cache" };
            IncludeEngine.CachedIncludePaths.TryAdd(typeof(Book), expectedIncludes);

            var includeFinderMock = new Mock<IncludeFinder>();
            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>(), 
                includeExecution: IncludeExecution.ReadOnlyCache);

            engine.RunIncludesForType(query, context);

            Assert.Equal(expectedIncludes, context.IncludePathsToInclude);

            includeFinderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void WhenUsingReCachedResult_TheCacheShouldBeReSet()
        {
            var expectedIncludes = new List<string>() { "Test", "Cache" };
            var notExpectedIncludes = new List<string>() { "Test" };
            IncludeEngine.CachedIncludePaths.TryAdd(typeof(Book), notExpectedIncludes);

            var includeFinderMock = new Mock<IncludeFinder>();
            includeFinderMock.Setup(finder => finder.BuildIncludePathsForRootType(It.IsAny<EagerLoadContext>()))
                .Returns(expectedIncludes);

            var queryIncluderMock = new Mock<QueryIncluder>();

            var engine = new IncludeEngine(includeFinderMock.Object, queryIncluderMock.Object);

            var query = new Book[0].AsQueryable();
            var context = new EagerLoadContext(Mock.Of<DbContext>(), Mock.Of<IIncludeStrategy>(), 
                includeExecution: IncludeExecution.Recache);

            engine.RunIncludesForType(query, context);

            Assert.Equal(expectedIncludes, context.IncludePathsToInclude);
            var cachedIncludes = IncludeEngine.CachedIncludePaths.GetOrAdd(typeof(Book), t => null);
            Assert.Equal(expectedIncludes, cachedIncludes);

            includeFinderMock.Verify(finder => finder.BuildIncludePathsForRootType(It.IsAny<EagerLoadContext>()), Times.Once);
        }

    }
}
