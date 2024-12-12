using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class RaidStrategyWorker_ImmediateAttack_SnowstormCultist : RaidStrategyWorker
{
    public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
    {
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp == null || !storyGameComp.StoryActive || !storyGameComp.storyInProgress)
        {
            return false;
        };
        return base.CanUseWith(parms, groupKind);
    }

    public override float SelectionWeight(Map map, float basePoints)
    {
        if (!SnowstormUtility.IsSnowExtremeWeather(map))
        {
            return 0f;
        }
        return base.SelectionWeight(map, basePoints);
    }
    protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
    {
        IntVec3 originCell = (parms.spawnCenter.IsValid ? parms.spawnCenter : pawns[0].PositionHeld);
        if (parms.attackTargets != null && parms.attackTargets.Count > 0)
        {
            return new LordJob_AssaultThings_NeverFlee(parms.faction, parms.attackTargets);
        }
        if (parms.faction.HostileTo(Faction.OfPlayer))
        {
            return new LordJob_AssaultColony_NeverFlee(parms.faction, canTimeoutOrFlee: parms.canTimeoutOrFlee, canKidnap: parms.canKidnap, sappers: false, useAvoidGridSmart: false, canSteal: parms.canSteal);
        }
        RCellFinder.TryFindRandomSpotJustOutsideColony(originCell, map, out IntVec3 result);
        return new LordJob_AssistColony_NeverFlee(parms.faction, result);
    }
}
