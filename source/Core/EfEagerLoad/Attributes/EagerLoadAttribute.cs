using System;

namespace EfEagerLoad.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : Attribute
    {
        public const int DefaultMaxDepth = 7;
        public const int DefaultMaxDepthPosition = 8;
        public const int DefaultMaxRootTypeCount = 2;
        public const int DefaultMaxTypeCount = 3;

        /// <summary>Initializes a new instance of the <see cref="EagerLoadAttribute"/> class.</summary>
        /// <param name="always">if set to <c>true</c> Will always Eager Load this Property regardless of other rules.</param>
        /// <param name="onlyOnRoot">if set to <c>true</c> Will only Eager Load this Property if it's on the Root Entity being loaded.</param>
        /// <param name="notOnRoot">if set to <c>true</c> Will not Eager Load this Property if it's on the Root Entity being loaded.</param>
        /// <param name="notIfParentType">
        /// if set to <c>true</c> Will only Eager Load this Property if it's the same type as the Root Entity being loaded.
        /// </param>
        /// <param name="notIfRootType">if set to <c>true</c> [not if root type].</param>
        /// <param name="maxDepth">
        /// The maximum depth allowed for this this Property's Navigation Path. This will only be applied for a Root Navigation.
        /// </param>
        /// <param name="maxDepthPosition">The maximum depth allowed for this Property to be Eager Loaded on the Navigation Path.</param>
        /// <param name="maxRootTypeCount">The maximum root type count.</param>
        /// <param name="maxTypeCount">The maximum type count.</param>
        public EagerLoadAttribute(bool always = false, bool onlyOnRoot = false, bool notOnRoot = false, 
                                    bool notIfParentType = false, bool notIfRootType = false,
                                    int maxDepth = DefaultMaxDepth, int maxDepthPosition = DefaultMaxDepthPosition,
                                    int maxRootTypeCount = DefaultMaxRootTypeCount, int maxTypeCount = DefaultMaxTypeCount)
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
