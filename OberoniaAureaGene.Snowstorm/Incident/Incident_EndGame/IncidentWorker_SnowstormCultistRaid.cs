using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
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
        if (parms.faction != null)
        {
            return true;
        }
        else
        {
            Faction tempFaction = null;
            if (Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist)
            {
                tempFaction ??= OAFrame_FactionUtility.ValidTempFactionsOfDef(FactionDefOf.OutlanderCivil).Where(f => !f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback(null);
                tempFaction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil, FactionRelationKind.Ally);
                tempFaction ??= Find.FactionManager.RandomNonHostileFaction(allowNonHumanlike: false);
            }
            else
            {
                tempFaction ??= OAFrame_FactionUtility.ValidTempFactionsOfDef(FactionDefOf.OutlanderCivil).Where(f => f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback(null);
                tempFaction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil, FactionRelationKind.Hostile);
                tempFaction ??= Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
            }
            if (tempFaction == null)
            {
                return false;
            }
            parms.faction = tempFaction;
            AdjuestFactionRelation(parms.faction);
            return true;
        }

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
        parms.raidStrategy = Snowstorm_MiscDefOf.OAGene_ImmediateAttack_SnowstormCultist;
        parms.canTimeoutOrFlee = false;
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
                    Snowstorm_MiscUtility.SetColdPreparation(pawn, Snowstorm_HediffDefOf.OAGene_Hediff_ColdPreparation_Enemy);
                }
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

    protected static void AdjuestFactionRelation(Faction faction)
    {
        Faction ofPlayer = Faction.OfPlayer;
        List<FactionRelation> relations = [];
        foreach (Faction otherF in Find.FactionManager.AllFactionsListForReading)
        {
            if (otherF == faction || otherF == ofPlayer)
            {
                continue;
            }
            if (!otherF.def.PermanentlyHostileTo(faction.def))
            {
                relations.Add(new FactionRelation
                {
                    other = otherF,
                    kind = FactionRelationKind.Hostile
                });
            }
        }
        for (int i = 0; i < relations.Count; i++)
        {
            faction.SetRelation(relations[i]);
        }
    }
}
