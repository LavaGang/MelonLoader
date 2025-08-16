namespace MelonLoader
{
    // TO-DO: Replace with LemonTuple<MelonBase[], RottenMelon[]>
    public sealed class ResolvedMelons
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
