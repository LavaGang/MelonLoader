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

    /*
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonPriority : MelonPriorityAttribute { public MelonPriority(int priority = 0) : base(priority) { } }
    */
}