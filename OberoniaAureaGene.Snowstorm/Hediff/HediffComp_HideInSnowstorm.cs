using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_HideInSnowstorm : HediffCompProperties
{
    public HediffCompProperties_HideInSnowstorm()
    {
        compClass = typeof(HediffComp_HideInSnowstorm);
    }
}

public class HediffComp_HideInSnowstorm : HediffComp
{
    protected int lastDetectedCyclePassed = 0;

    [Unsaved]
    protected HediffComp_Invisibility invisibility;
    protected HediffComp_Invisibility Invisibility => invisibility ??= parent.TryGetComp<HediffComp_Invisibility>();

    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(120))
        {
            Pawn parentPawn = parent.pawn;
            if (!parentPawn.Spawned)
            {
                parentPawn.health.RemoveHediff(parent);
                return;
            }
            Map map = parentPawn.Map;
            IntVec3 parentPos = parentPawn.Position;
            lastDetectedCyclePassed++;
            if (map.roofGrid.Roofed(parentPos))
            {
                Invisibility.BecomeVisible();
                lastDetectedCyclePassed = 0;
            }
            else if (lastDetectedCyclePassed >= 10 && CheckDetected(map, parentPos))
            {
                Invisibility.BecomeVisible();
                lastDetectedCyclePassed = 0;
            }

            if (lastDetectedCyclePassed >= 10)
            {
                Invisibility.BecomeInvisible();
            }
        }
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref lastDetectedCyclePassed, "lastDetectedCyclePassed", 0);
    }

    protected static bool CheckDetected(Map map, IntVec3 parentPos)
    {

        foreach (Pawn pawn in map.listerThings.ThingsInGroup(ThingRequestGroup.Pawn).Cast<Pawn>())
        {
            if (PawnCanDetect(pawn, map, parentPos))
            {
                return true;
            }
        }
        return false;
    }

    protected static bool PawnCanDetect(Pawn pawn, Map map, IntVec3 parentPos)
    {
        if (pawn.Faction != Faction.OfPlayer || pawn.Downed || !pawn.Awake())
        {
            return false;
        }
        if (pawn.RaceProps.Animal)
        {
            return false;
        }
        if (!parentPos.InHorDistOf(pawn.Position, GetPawnSightRadius(pawn, map, parentPos)))
        {
            return false;
        }
        return GenSight.LineOfSight(pawn.Position, parentPos, map);
    }

    protected static float GetPawnSightRadius(Pawn pawn, Map map, IntVec3 parentPos)
    {
        float factor = 14f;
        if (pawn.genes == null || pawn.genes.AffectedByDarkness)
        {
            float t = map.glowGrid.GroundGlowAt(parentPos);
            factor *= Mathf.Lerp(0.33f, 1f, t);
        }
        return factor * pawn.health.capacities.GetLevel(PawnCapacityDefOf.Sight);
    }

}
