using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class RaidStrategyWorker_SnowstormImmediateAttackBreaching : RaidStrategyWorker_ImmediateAttackBreaching
{
    protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
    {
        if (pawns != null)
        {
            foreach (Pawn pawn in pawns)
            {
                pawn.health.AddHediff(Snowstrom_HediffDefOf.OAGene_Hediff_PreparationWarm);
            }
        }
        Faction faction = parms.faction;
        bool canTimeoutOrFlee = parms.canTimeoutOrFlee;
        return new LordJob_AssistColony_SnowstormAttackBreaching(canKidnap: parms.canKidnap, canTimeoutOrFlee: canTimeoutOrFlee, sappers: false, canSteal: parms.canSteal, assaulterFaction: faction, useAvoidGridSmart: useAvoidGridSmart, breachers: true);
    }
}
