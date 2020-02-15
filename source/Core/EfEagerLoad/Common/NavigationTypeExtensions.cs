using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.Common
{
    public static class NavigationTypeExtensions
    {
        public static Type GetNavigationType(this INavigation navigation)
        {
            return navigation.IsCollection() ? navigation.GetTargetType().ClrType : navigation.ClrType;
        }
    }
}
