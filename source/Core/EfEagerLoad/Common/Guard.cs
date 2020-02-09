using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("EfEagerLoad.Tests")]
namespace EfEagerLoad.Common
{
    internal static class Guard
    {
        internal static void IsNotNull(string parameterName, object value)
        {
            if (value == null) { throw new ArgumentNullException(parameterName); }
        }
    }
}
