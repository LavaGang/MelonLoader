using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonPriorityAttribute : Attribute
    {
        /// <summary>
        /// Priority of the Melon.
        /// </summary>
        public MelonBase.MelonPriority Priority;

        public MelonPriorityAttribute(MelonBase.MelonPriority priority = MelonBase.MelonPriority.NORMAL) => Priority = priority;
    }
}