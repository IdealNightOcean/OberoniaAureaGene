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
    protected CompRefuelable refuelableComp;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        refuelableComp = parent.GetComp<CompRefuelable>();
    }
    public override void CompTick()
    {
        base.CompTick();
        if (parent.IsHashIntervalTick(250) && parent.Spawned)
        {
            if (refuelableComp.HasFuel)
            {
                float sevAdjuest = -(Props.hypothermiaDecreasePreHour * 0.1f);
                GetPawnsInRadius(parent.Position, parent.Map, Props.affectRadius);
                foreach (Pawn p in TargetPawns)
                {
                    HediffDef hediffDef = (p.RaceProps.FleshType == FleshTypeDefOf.Insectoid) ? Props.hediffInsectoid : Props.hediffHuman;
                    HealthUtility.AdjustSeverity(p, hediffDef, sevAdjuest);
                }
                TargetPawns.Clear();
            }
        }
    }
    protected static void GetPawnsInRadius(IntVec3 ctrPosition, Map map, float radius)
    {
        TargetPawns.Clear();
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(ctrPosition, radius, useCenter: true))
        {
            List<Thing> thingList = map.thingGrid.ThingsListAt(cell);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Pawn pawn)
                {
                    TargetPawns.Add(pawn);
                }
            }
        }
    }
}
