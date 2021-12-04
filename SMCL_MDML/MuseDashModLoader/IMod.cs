namespace ModHelper
{
    public interface IMod
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string HomePage { get; }
        void DoPatching();
    }
}