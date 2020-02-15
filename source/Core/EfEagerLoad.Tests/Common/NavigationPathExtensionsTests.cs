using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Tests.Testing;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfEagerLoad.Tests.Common
{
    public class NavigationPathExtensionsTests
    {

        [Fact]
        public void ShouldReturnParentPath_WhenParentPathExists()
        {
            var pathToTest = "First.Second";
            Assert.Equal("First", pathToTest.AsSpan().GetParentIncludePathSpan().ToString());

            pathToTest = "First.Second.Third";
            Assert.Equal("First.Second", pathToTest.AsSpan().GetParentIncludePathSpan().ToString());
        }

        [Fact]
        public void ShouldReturnEmptyString_WhenNoParentPathExists()
        {
            var pathToTest = "First";
            Assert.Equal(string.Empty, pathToTest.AsSpan().GetParentIncludePathSpan().ToString());
        }

    }
}
