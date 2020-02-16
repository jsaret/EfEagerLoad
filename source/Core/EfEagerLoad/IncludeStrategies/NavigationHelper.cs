using System;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfEagerLoad.IncludeStrategies
{
    public class NavigationHelper
    {
        public virtual Type GetTypeForNavigation(INavigation navigation)
        {
            return navigation.GetNavigationType();
        }
    }
}
