using System;

namespace EfEagerLoad.IncludeStrategies
{
    public enum IncludeExecution
    {
        Cached = 0,
        NoCache = 1,
        Recache = 2,
        Skip = 3
    }
}
