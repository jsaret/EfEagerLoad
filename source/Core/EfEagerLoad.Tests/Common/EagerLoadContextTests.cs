using System;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using Xunit;

namespace EfEagerLoad.Tests.Common
{
    public class EagerLoadContextTests
    {
        [Fact]
        public void ShouldDisplayEmpty_ParentIncludePath_WhenNoNavigationsSet()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);

            Assert.Equal(string.Empty, context.ParentIncludePath);
        }

        [Fact]
        public void ShouldDisplayEmpty_CurrentIncludePath_WhenNoNavigationsSet()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);

            Assert.Equal(string.Empty, context.CurrentIncludePath);
            Assert.Equal(string.Empty.ToCharArray(), context.CurrentIncludePathSpan.ToString());
        }

        [Fact]
        public void ShouldDisplayEmpty_ParentIncludePath_WhenAddingFirstNavigation()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);
            
            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));

            Assert.Equal(string.Empty, context.ParentIncludePath);
        }

        [Fact]
        public void ShouldDisplayCorrect_CurrentIncludePath_WhenAddingFirstNavigation()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);
            
            var navigationMock = new Mock<INavigation>();
            navigationMock.Setup(nav => nav.Name).Returns(nameof(Book));

            context.SetCurrentNavigation(navigationMock.Object);

            Assert.Equal(nameof(Book), context.CurrentIncludePath);
            Assert.Equal(nameof(Book).ToCharArray(), context.CurrentIncludePathSpan.ToString());
        }

        [Fact]
        public void ShouldDisplayCorrect_ParentIncludePath_WhenMoreNavigationsAdded()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);
            
            var bookNavigationMock = new Mock<INavigation>();
            bookNavigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            var authorNavigationMock = new Mock<INavigation>();
            authorNavigationMock.Setup(nav => nav.Name).Returns(nameof(Author));
            var publisherNavigationMock = new Mock<INavigation>();
            publisherNavigationMock.Setup(nav => nav.Name).Returns(nameof(Publisher));

            context.SetCurrentNavigation(bookNavigationMock.Object);
            context.SetCurrentNavigation(authorNavigationMock.Object);

            Assert.Equal(nameof(Book), context.ParentIncludePath);

            context.SetCurrentNavigation(publisherNavigationMock.Object);

            Assert.Equal($"{nameof(Book)}.{nameof(Author)}", context.ParentIncludePath);
        }

        [Fact]
        public void ShouldDisplayCorrect_CurrentIncludePath_WhenMoreNavigationsAdded()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);

            var bookNavigationMock = new Mock<INavigation>();
            bookNavigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            var authorNavigationMock = new Mock<INavigation>();
            authorNavigationMock.Setup(nav => nav.Name).Returns(nameof(Author));
            var publisherNavigationMock = new Mock<INavigation>();
            publisherNavigationMock.Setup(nav => nav.Name).Returns(nameof(Publisher));

            context.SetCurrentNavigation(bookNavigationMock.Object);
            context.SetCurrentNavigation(authorNavigationMock.Object);

            Assert.Equal($"{nameof(Book)}.{nameof(Author)}", context.CurrentIncludePath);

            context.SetCurrentNavigation(publisherNavigationMock.Object);

            Assert.Equal($"{nameof(Book)}.{nameof(Author)}.{nameof(Publisher)}", context.CurrentIncludePath);
        }


        [Fact]
        public void ShouldDisplayCorrect_ParentIncludePath_WhenRemovingNavigation()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);

            var bookNavigationMock = new Mock<INavigation>();
            bookNavigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            var authorNavigationMock = new Mock<INavigation>();
            authorNavigationMock.Setup(nav => nav.Name).Returns(nameof(Author));
            var publisherNavigationMock = new Mock<INavigation>();
            publisherNavigationMock.Setup(nav => nav.Name).Returns(nameof(Publisher));

            context.SetCurrentNavigation(bookNavigationMock.Object);
            context.SetCurrentNavigation(authorNavigationMock.Object);
            context.SetCurrentNavigation(publisherNavigationMock.Object);

            Assert.Equal($"{nameof(Book)}.{nameof(Author)}", context.ParentIncludePath);

            context.RemoveCurrentNavigation();
            Assert.Equal($"{nameof(Book)}", context.ParentIncludePath);

            context.RemoveCurrentNavigation();
            Assert.Equal(string.Empty, context.ParentIncludePath);

            context.RemoveCurrentNavigation();
            Assert.Equal(string.Empty, context.ParentIncludePath);
        }


        [Fact]
        public void ShouldDisplayCorrect_CurrentIncludePath_WhenRemovingNavigation()
        {
            var context = new EagerLoadContext(new Mock<DbContext>().Object, new Mock<IIncludeStrategy>().Object);

            var bookNavigationMock = new Mock<INavigation>();
            bookNavigationMock.Setup(nav => nav.Name).Returns(nameof(Book));
            var authorNavigationMock = new Mock<INavigation>();
            authorNavigationMock.Setup(nav => nav.Name).Returns(nameof(Author));
            var publisherNavigationMock = new Mock<INavigation>();
            publisherNavigationMock.Setup(nav => nav.Name).Returns(nameof(Publisher));

            context.SetCurrentNavigation(bookNavigationMock.Object);
            context.SetCurrentNavigation(authorNavigationMock.Object);
            context.SetCurrentNavigation(publisherNavigationMock.Object);

            Assert.Equal($"{nameof(Book)}.{nameof(Author)}.{nameof(Publisher)}", context.CurrentIncludePath);

            context.RemoveCurrentNavigation();
            Assert.Equal($"{nameof(Book)}.{nameof(Author)}", context.CurrentIncludePath);

            context.RemoveCurrentNavigation();
            Assert.Equal($"{nameof(Book)}", context.CurrentIncludePath);

            context.RemoveCurrentNavigation();
            Assert.Equal(string.Empty, context.CurrentIncludePath);

            context.RemoveCurrentNavigation();
            Assert.Equal(string.Empty, context.CurrentIncludePath);
        }


    }
}
