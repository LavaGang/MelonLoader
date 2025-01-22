using System;

namespace MelonLoader;
/// <summary>
/// AdditionalCredits constructor
/// </summary>
/// <param name="credits">The additional credits of the mod</param>
[AttributeUsage(AttributeTargets.Assembly)]
public class MelonAdditionalCreditsAttribute(string credits) : Attribute
{
    /// <summary>
    /// Any additional credits that the mod author might want to include
    /// </summary>
    public string Credits { get; internal set; } = credits;
}
