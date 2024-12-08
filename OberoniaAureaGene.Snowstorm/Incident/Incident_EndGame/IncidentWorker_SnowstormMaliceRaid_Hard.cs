using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class IncidentWorker_SnowstormMaliceRaid_Hard : IncidentWorker_SnowstormMaliceRaid_Reinforce
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!OAGene_SnowstormSettings.AllowDifficultEnemy)
        {
            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, parms.target);
            float points = Mathf.Max(raidParms.points, parms.points);
            raidParms.points = Mathf.Min(points, 10000f);
            raidParms.faction = Faction.OfMechanoids;
            raidParms.raidStrategy = Snowstorm_RimWorldDefOf.ImmediateAttackBreaching;
            return OAFrame_MiscUtility.TryFireIncidentNow(IncidentDefOf.RaidEnemy, raidParms);
        }
        else
        {
            return base.TryExecuteWorker(parms);
        }
    }
    protected override void PostProcessSpawnedPawns(IncidentParms parms, List<Pawn> pawns)
    {
        if (pawns != null)
        {
            foreach (Pawn pawn in pawns)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    pawn.story?.traits?.GainTrait(new Trait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor, 1, forced: true), suppressConflicts: true);
                    pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_PreparationWarm);
                    pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_HideInSnowstorm);
                }
            }
        }
    }
    protected override string GetLetterLabel(IncidentParms parms)
    {
        return "OAGene_LetterLabel_SnowstormMaliceRaid_Hard".Translate();
    }

    protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
    {
        return "OAGene_Letter_SnowstormMaliceRaid_Hard".Translate();
    }
}
