﻿using System;

namespace EfEagerLoad.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : Attribute
    {
        private const int DefaultMaxDepth = 5;
        private const int DefaultMaxDepthPosition = int.MaxValue;
        private const int DefaultMaxRootTypeCount = 3;
        private const int DefaultMaxTypeCount = 4;

        /// <summary>Initializes a new instance of the <see cref="EagerLoadAttribute"/> class.</summary>
        /// <param name="always">If set to <c>true</c> will always Eager Load this Property regardless of other rules.</param>
        /// <param name="never">If set to <c>true</c> will never Eager Load this Property regardless of other rules.</param>
        /// <param name="onlyOnRoot">If set to <c>true</c> will only Eager Load this Property if it's on the Root Entity being loaded.</param>
        /// <param name="notOnRoot">If set to <c>true</c> will not Eager Load this Property if it's on the Root Entity being loaded.</param>
        /// <param name="notIfParentsParentType">
        /// If set to <c>true</c> will not Eager Load this Property if it's the same type as the Parent's Parent Entity being loaded.
        /// </param>
        /// <param name="notIfRootType">If set to <c>true</c> will not Eager Load this Property if it's the same type as the Root Entity being loaded.
        /// </param>
        /// <param name="notIfRootNavigationType">>If set to <c>true</c> will not Eager Load this Property if it's the same type as the Root Navigation (Property) being loaded.</param>
        /// <param name="maxDepth">
        /// The maximum depth allowed for this this Property's Navigation Path. This will only be applied for a Root Navigation.
        /// </param>
        /// <param name="maxDepthPosition">The maximum depth allowed for this Property to be Eager Loaded on the Navigation Path.</param>
        /// <param name="maxRootTypeCount">The maximum count for the Type if it matches the Root Type being loaded.</param>
        /// <param name="maxTypeCount">The maximum count for the type being loaded. This includes the Root Entity's Type.</param>
        public EagerLoadAttribute(bool always = false, bool never = false, bool onlyOnRoot = false, bool notOnRoot = false, 
                                    bool notIfParentsParentType = false, bool notIfRootType = false,
                                    bool notIfRootNavigationType = false,
                                    int maxDepth = DefaultMaxDepth, int maxDepthPosition = DefaultMaxDepthPosition,
                                    int maxRootTypeCount = DefaultMaxRootTypeCount, int maxTypeCount = DefaultMaxTypeCount)
        {
            Always = always;
            Never = never;
            OnlyOnRoot = onlyOnRoot;
            NotOnRoot = notOnRoot;
            NotIfParentsParentType = notIfParentsParentType;
            NotIfRootType = notIfRootType;
            NotIfRootNavigationType = notIfRootNavigationType;
            MaxDepth = maxDepth;
            MaxDepthPosition = maxDepthPosition;
            MaxRootTypeCount = maxRootTypeCount;
            MaxTypeCount = maxTypeCount;
        }

        public bool Always { get; }

        public bool Never { get; }

        public bool OnlyOnRoot { get; }

        public bool NotOnRoot { get; }


        public bool NotIfParentsParentType { get; }

        public bool NotIfRootType { get; }

        public bool NotIfRootNavigationType { get; }

        public int MaxDepth { get; }

        public int MaxDepthPosition { get; }

        public int MaxRootTypeCount { get; }

        public int MaxTypeCount { get; }
    }
}
