using System;

namespace EfEagerLoad.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : System.Attribute
    {
        public EagerLoadAttribute(bool onlyIfOnRoot = false, bool notIfOnRoot = false, int maxVisitsForType = 2, int maxVisitsForRootType = 2, 
                                    int maxDepth = 5)
        {
            OnlyIfOnRoot = onlyIfOnRoot;
            NotIfOnRoot = notIfOnRoot;
            MaxVisitsForType = maxVisitsForType;
            MaxVisitsForRootType = maxVisitsForRootType;
        }

        public bool OnlyIfOnRoot { get; }

        public bool NotIfOnRoot { get; }

        public int MaxVisitsForType { get; }

        public int MaxVisitsForRootType { get; }
    }
}
