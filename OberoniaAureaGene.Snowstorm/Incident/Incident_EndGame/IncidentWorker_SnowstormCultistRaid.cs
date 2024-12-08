using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormCultistRaid : IncidentWorker_RaidEnemy
{
    protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
    {
        if (Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist)
        {
            return !f.HostileTo(Faction.OfPlayer);
        }
        else
        {
            return f.HostileTo(Faction.OfPlayer);
        }
    }
    protected override bool TryResolveRaidFaction(IncidentParms parms)
    {
        return parms.faction != null;
    }
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
        if (Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist)
        {
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttackFriendly;
        }
        else
        {
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        }
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
    protected override string GetLetterLabel(IncidentParms parms)
    {
        if (Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist)
        {
            return "OAGene_LetterLabel_SnowstormCultistRaidFriendly".Translate();
        }
        else
        {
            return "OAGene_LetterLabel_SnowstormCultistRaid".Translate();
        }

    }
    protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
    {
        if (Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist)
        {
            return "OAGene_Letter_SnowstormCultistRaidFriendly".Translate();
        }
        else
        {
            return "OAGene_Letter_SnowstormCultistRaid".Translate();
        }
    }

    protected override LetterDef GetLetterDef()
    {
        return Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist ? LetterDefOf.PositiveEvent : LetterDefOf.ThreatBig;
    }
}
