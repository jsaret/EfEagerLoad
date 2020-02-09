using System;

namespace EfEagerLoad.Common
{
    public static class NavigationPathExtensions
    {
        private static readonly char SeparatorCharacter = char.Parse(".");

        public static ReadOnlySpan<char> GetParentIncludePathSpan(this ReadOnlySpan<char> includePath)
        {
            if (!includePath.Contains(SeparatorCharacter)) { return string.Empty.ToCharArray(); }

            return includePath.Slice(0, includePath.LastIndexOf("."));
        }
    }
}
