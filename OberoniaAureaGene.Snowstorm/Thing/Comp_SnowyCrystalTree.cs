﻿using RimWorld;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class Comp_SnowyCrystalTree : CompTempControl
{
    public const int PreTreeTempCooler = -2;
    protected const float HeatPerRare = -225f / 2;

    private GameCondition_SnowyCrystalTreeCooler treeCoolerCondition;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        parent.Map?.GetComponent<MapComponent_Snowstorm>()?.snowyCrystalTreeComps.Add(this);
        treeCoolerCondition = TryInitGameCondiation(parent.Map);
    }
    public override void PostDeSpawn(Map map)
    {
        map.GetComponent<MapComponent_Snowstorm>()?.snowyCrystalTreeComps.Remove(this);
        base.PostDeSpawn(map);   
    }
    public override void CompTickRare()
    {
        base.CompTickRare();
        if (parent.Spawned && parent.AmbientTemperature > Props.defaultTargetTemperature)
        {
            GenTemperature.PushHeat(parent.Position, parent.Map, HeatPerRare);
        }
    }
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        return [];
    }
    public override string CompInspectStringExtra()
    {
        return Props.inspectString;
    }
    public static GameCondition_SnowyCrystalTreeCooler TryInitGameCondiation(Map map)
    {
        GameCondition_SnowyCrystalTreeCooler gameCondition_SnowyCrystalTreeCooler = map.gameConditionManager.GetActiveCondition(Snowstrom_MiscDefOf.OAGene_SnowyCrystalTreeCooler) as GameCondition_SnowyCrystalTreeCooler;
        if (gameCondition_SnowyCrystalTreeCooler == null)
        {
            gameCondition_SnowyCrystalTreeCooler = (GameCondition_SnowyCrystalTreeCooler)GameConditionMaker.MakeCondition(Snowstrom_MiscDefOf.OAGene_SnowyCrystalTreeCooler);
            map.GameConditionManager.RegisterCondition(gameCondition_SnowyCrystalTreeCooler);
            gameCondition_SnowyCrystalTreeCooler.Permanent = true;
        }
        return gameCondition_SnowyCrystalTreeCooler;
    }
}