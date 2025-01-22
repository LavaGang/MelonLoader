using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MelonLoader;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[AttributeUsage(AttributeTargets.Assembly)]
public class Demeo_LobbyRequirement : Attribute
{
    public Demeo_LobbyRequirement() { }
}
