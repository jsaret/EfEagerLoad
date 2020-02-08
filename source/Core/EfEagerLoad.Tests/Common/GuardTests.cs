using EfEagerLoad.Common;
using System;
using Xunit;

namespace EfEagerLoad.Tests.Common
{
    public class GuardTests
    {
        [Fact]
        public void ShouldThrow_WhenArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.IsNotNull(nameof(ShouldThrow_WhenArgumentIsNull), null));
        }

        [Fact]
        public void ShouldNotThrow_WhenArgumentIsNotNull()
        {
            Guard.IsNotNull(nameof(ShouldNotThrow_WhenArgumentIsNotNull), new object());
        }

    }
}
