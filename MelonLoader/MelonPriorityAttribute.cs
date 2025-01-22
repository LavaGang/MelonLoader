using System;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly)]
public class MelonPriorityAttribute(int priority = 0) : Attribute
{
    /// <summary>
    /// Priority of the Melon.
    /// </summary>
    public int Priority = priority;
}