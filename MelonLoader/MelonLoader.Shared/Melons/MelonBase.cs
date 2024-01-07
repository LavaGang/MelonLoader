namespace MelonLoader.Melons;

public abstract class MelonBase
{
    private MelonInfoAttribute[] _games = new MelonInfoAttribute[0];
    
    public virtual void OnEarlyInitializeMelon() { }
    public virtual void OnInitializeMelon() { }
    public virtual void OnDeinitializeMelon() { }
}