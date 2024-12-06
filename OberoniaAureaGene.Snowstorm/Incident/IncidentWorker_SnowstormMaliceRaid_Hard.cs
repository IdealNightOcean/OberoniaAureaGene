using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class IncidentWorker_SnowstormMaliceRaid_Hard : IncidentWorker_SnowstormMaliceRaid
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!OAGene_SnowstormSettings.AllowDifficultEnemy)
        {
            IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, parms.target);
            float points = 3 * Mathf.Max(raidParms.points, parms.points);
            raidParms.points = Mathf.Min(points, 10000f);
            raidParms.faction = Faction.OfMechanoids;
            raidParms.raidStrategy = Snowstorm_RimWorldDefOf.ImmediateAttackBreaching;
            return OAFrame_MiscUtility.TryFireIncidentNow(IncidentDefOf.RaidEnemy, raidParms);
        }
        return base.TryExecuteWorker(parms);
    }
    protected override void PostProcessSpawnedPawns(IncidentParms parms, List<Pawn> pawns)
    {
        if (pawns != null)
        {
            foreach (Pawn pawn in pawns)
            {
                pawn.story?.traits?.GainTrait(new Trait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor, 1, forced: true), suppressConflicts: true);
                pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_PreparationWarm);
            }
        }
    }


}
