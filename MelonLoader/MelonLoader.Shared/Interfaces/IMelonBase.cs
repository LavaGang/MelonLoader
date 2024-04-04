using MelonLoader.Melons;
using MelonLoader.Utils;

namespace MelonLoader.Interfaces;

public interface IMelonBase
{
    public MelonLogger.Instance Logger { get; set; }
    
    public void OnApplicationStart();
}