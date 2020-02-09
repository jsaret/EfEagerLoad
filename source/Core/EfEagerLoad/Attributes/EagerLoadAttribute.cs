using System;

namespace EfEagerLoad.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : Attribute
    {
        public EagerLoadAttribute(bool always = false, bool onlyOnRoot = false, bool notOnRoot = false, 
                                    bool notIfParentType = false, bool notIfRootType = false,
                                    int maxDepth = 6, int maxDepthPosition = 6,
                                    int maxRootTypeCount = 2, int maxTypeCount = 3)
        {
            Always = always;
            OnlyOnRoot = onlyOnRoot;
            NotOnRoot = notOnRoot;
            NotIfParentType = notIfParentType;
            NotIfRootType = notIfRootType;
            MaxDepth = maxDepth;
            MaxDepthPosition = maxDepthPosition;
            MaxRootTypeCount = maxRootTypeCount;
            MaxTypeCount = maxTypeCount;
        }

        public bool Always { get; }

        public bool OnlyOnRoot { get; }

        public bool NotOnRoot { get; }

        public bool NotIfParentType { get; }

        public bool NotIfRootType { get; }

        public int MaxDepth { get; }

        public int MaxDepthPosition { get; }

        public int MaxRootTypeCount { get; }

        public int MaxTypeCount { get; }
    }
}
