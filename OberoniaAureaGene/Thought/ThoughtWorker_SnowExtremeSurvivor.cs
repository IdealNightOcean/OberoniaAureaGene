using RimWorld;
using Verse;

namespace OberoniaAureaGene;
public class ThoughtWorker_SnowExtremeSurvivor : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.Spawned || !SnowstormUtility.IsSnowExtremeWeather(p.Map))
        {
            return ThoughtState.Inactive;
        }

        return ThoughtState.ActiveDefault;
    }
}