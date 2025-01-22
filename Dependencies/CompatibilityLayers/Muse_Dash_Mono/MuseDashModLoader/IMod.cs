#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ModHelper;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public interface IMod
{
    string Name { get; }
    string Description { get; }
    string Author { get; }
    string HomePage { get; }
    void DoPatching();
}