using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormMaliceRaid_Reinforce : IncidentWorker_SnowstormMaliceRaid
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!GameComponent_SnowstormStory.Instance.storyInProgress)
        {
            return false;
        }
        return base.CanFireNowSub(parms);
    }
    protected override void PostProcessSpawnedPawns(IncidentParms parms, List<Pawn> pawns)
    {
        if (pawns is not null)
        {
            foreach (Pawn pawn in pawns)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    pawn.story?.traits?.GainTrait(new Trait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor, 1, forced: true), suppressConflicts: true);
                    Snowstorm_MiscUtility.SetColdPreparation(pawn, Snowstorm_HediffDefOf.OAGene_Hediff_ColdPreparation_Enemy);
                }
            }
        }
    }
}
