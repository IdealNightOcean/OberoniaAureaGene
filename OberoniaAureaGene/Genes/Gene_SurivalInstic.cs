using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace OberoniaAureaGene;

public class Gene_SurivalInstic : Gene
{
    private static readonly HediffGiveParams hediffGiveParams = new()
    {
        InitSeverity = 1.0f,
        OverrideDisappearTicks = 500
    };
    private static readonly List<IAttackTarget> TempTargets = [];
    public override void Tick()
    {
        if (pawn.IsHashIntervalTick(250))
        {
            CheckEnemy(pawn);
        }
    }
    private static void CheckEnemy(Pawn pawn)
    {
        if (!pawn.Spawned)
        {
            return;
        }
        float minDistance = TryGetValidTarget(pawn);
        if (minDistance <= 10f)
        {
            hediffGiveParams.HediffToGive = OAGene_HediffDefOf.OAGene_SurvivalInstinct;
            hediffGiveParams.InitSeverity = 2.0f;
            pawn.GetOrAddHediff(hediffGiveParams);
        }
        else if (minDistance <= 20f)
        {
            hediffGiveParams.HediffToGive = OAGene_HediffDefOf.OAGene_SurvivalInstinct;
            hediffGiveParams.InitSeverity = 1.0f;
            pawn.GetOrAddHediff(hediffGiveParams);
        }
    }

    private static float TryGetValidTarget(Pawn pawn)
    {
        IntVec3 rootPos = pawn.Position;
        TempTargets.Clear();
        TempTargets.AddRange(pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn));
        TempTargets.RemoveAll(t => !ValidTarget(t));
        Thing closestThing = GenClosest.ClosestThing_Global(pawn.Position, TempTargets, 30f);
        if (closestThing is not null)
        {
            return pawn.Position.DistanceTo(closestThing.Position);
        }
        else
        {
            return 99999f;
        }
    }
    private static bool ValidTarget(IAttackTarget t)
    {
        if (t is Pawn pawn && !pawn.IsCombatant())
        {
            return false;
        }
        return true;
    }
}
