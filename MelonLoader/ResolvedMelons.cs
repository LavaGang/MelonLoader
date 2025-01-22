namespace MelonLoader
{
    public sealed class ResolvedMelons // This class only exists because I can't use Tuples
    {
        public readonly MelonBase[] loadedMelons;
        public readonly RottenMelon[] rottenMelons;

        public ResolvedMelons(MelonBase[] loadedMelons, RottenMelon[] rottenMelons)
        {
            this.loadedMelons = loadedMelons ?? new MelonBase[0];
            this.rottenMelons = rottenMelons ?? new RottenMelon[0];
        }
    }
}
