using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonPriorityAttribute : Attribute
    {
        /// <summary>
        /// Priority of the Melon.
        /// </summary>
        public int Priority;

        public MelonPriorityAttribute(int priority = 0) => Priority = priority;
    }
}