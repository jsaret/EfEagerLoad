using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
