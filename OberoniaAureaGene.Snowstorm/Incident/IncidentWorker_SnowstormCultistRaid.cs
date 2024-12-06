using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormCultistRaid : IncidentWorker_RaidEnemy
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        return base.CanFireNowSub(parms);
    }
    protected override void ResolveRaidPoints(IncidentParms parms)
    {
        parms.points = 10000f;
    }
    public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
    {
        parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        base.ResolveRaidStrategy(parms, groupKind);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        parms.canTimeoutOrFlee = false;
        return base.TryExecuteWorker(parms);
    }
    protected override void PostProcessSpawnedPawns(IncidentParms parms, List<Pawn> pawns)
    {
        if (pawns != null)
        {
            foreach (Pawn pawn in pawns)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_SnowstormCultist);
                }
                pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_PreparationWarm);
            }
        }
    }
}
