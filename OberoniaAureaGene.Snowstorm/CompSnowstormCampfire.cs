using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class CompProperties_SnowstormCampfire : CompProperties
{
    public HediffDef hediffHuman;
    public HediffDef hediffInsectoid;
    public float affectRadius;
    public float hypothermiaDecreasePreHour;

    public CompProperties_SnowstormCampfire()
    {
        compClass = typeof(CompSnowstormCampfire);
    }

}

public class CompSnowstormCampfire : ThingComp
{
    public CompProperties_SnowstormCampfire Props => props as CompProperties_SnowstormCampfire;
    protected static readonly List<Pawn> TargetPawns = [];
    public override void CompTick()
    {
        base.CompTick();
        if (parent.IsHashIntervalTick(250) && parent.Spawned)
        {
            float sevAdjuest = -(Props.hypothermiaDecreasePreHour * 0.1f);
            GetPawnsInRadius(parent.Position, parent.Map, Props.affectRadius, TargetPawns);
            foreach (Pawn p in TargetPawns)
            {
                HediffDef hediffDef = (p.RaceProps.FleshType == FleshTypeDefOf.Insectoid) ? Props.hediffInsectoid : Props.hediffHuman;
                HealthUtility.AdjustSeverity(p, hediffDef, sevAdjuest);
            }
            TargetPawns.Clear();
        }
    }
    protected static void GetPawnsInRadius(IntVec3 ctrPosition, Map map, float radius, List<Pawn> targetPawns, List<Pawn> ignorePawn = null)
    {
        targetPawns.Clear();
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(ctrPosition, radius, useCenter: true))
        {
            List<Thing> thingList = map.thingGrid.ThingsListAt(cell);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Pawn pawn)
                {
                    targetPawns.Add(pawn);
                }
            }
        }
        if (ignorePawn != null)
        {
            foreach (Pawn pawn in ignorePawn)
            {
                targetPawns.Remove(pawn);
            }
        }
    }
}
