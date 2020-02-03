using System;

namespace EfEagerLoad.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : Attribute
    {
        public EagerLoadAttribute(bool onlyIfOnRoot = false, bool notIfOnRoot = false, int allowedVisitsForType = 2, int allowedVisitsForRootType = 2)
        {
            OnlyIfOnRoot = onlyIfOnRoot;
            NotIfOnRoot = notIfOnRoot;
            AllowedVisitsForType = allowedVisitsForType;
            AllowedVisitsForRootType = allowedVisitsForRootType;
        }

        public bool OnlyIfOnRoot { get; }

        public bool NotIfOnRoot { get; }

        public int AllowedVisitsForType { get; }

        public int AllowedVisitsForRootType { get; }
    }
}
