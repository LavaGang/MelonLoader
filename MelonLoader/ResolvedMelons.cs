namespace MelonLoader;

public sealed class ResolvedMelons(MelonBase[] loadedMelons, RottenMelon[] rottenMelons) // This class only exists because I can't use Tuples
{
    public readonly MelonBase[] loadedMelons = loadedMelons ?? [];
    public readonly RottenMelon[] rottenMelons = rottenMelons ?? [];
}
