using System;

namespace EfEagerLoad.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : Attribute
    {
        public EagerLoadAttribute(bool onlyIfOnRoot = false, bool notIfOnRoot = false)
        {
            OnlyIfOnRoot = onlyIfOnRoot;
            NotIfOnRoot = notIfOnRoot;
        }

        public bool OnlyIfOnRoot { get; }

        public bool NotIfOnRoot { get; }
    }
}
