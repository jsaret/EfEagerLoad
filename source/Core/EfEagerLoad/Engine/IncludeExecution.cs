using System;

namespace EfEagerLoad.Engine
{
    public enum IncludeExecution
    {
        Cached = 0,
        UseOnlyCache = 1,
        Recache = 2,
        NoCache = 3,
        Skip = 4
    }
}
