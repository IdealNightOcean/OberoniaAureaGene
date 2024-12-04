using RimWorld;
using RimWorld.BaseGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class SymbolResolver_RuinedEmptyRoom : SymbolResolver
{
    public string interior;
    public bool useRandomCarpet;

    public bool allowRoof = true;

    public override void Resolve(ResolveParams rp)
    {
        BaseGen.symbolStack.Push("doors", rp);
        if (!interior.NullOrEmpty())
        {
            ResolveParams resolveParams1 = rp;
            resolveParams1.rect = rp.rect.ContractedBy(1);
            BaseGen.symbolStack.Push(interior, resolveParams1);
        }
        ResolveParams resolveParams2 = rp;
        if (useRandomCarpet)
        {
            resolveParams2.floorDef = DefDatabase<TerrainDef>.AllDefsListForReading.Where((TerrainDef x) => x.IsCarpet).RandomElement();
        }
        if (allowRoof)
        {
            BaseGen.symbolStack.Push("roof", resolveParams2);
        }

        ResolveParams resolveParams3 = rp;
        resolveParams3.wallStuff = ThingDefOf.WoodLog;
        foreach (IntVec3 edgeCell in resolveParams3.rect.EdgeCells)
        {
            if (edgeCell.InBounds(BaseGen.globalSettings.map))
            {
                TrySpawnWall(edgeCell, resolveParams3);
            }
        }

        ResolveParams resolveParams4 = rp;
        resolveParams4.floorDef = Snowstrom_RimWorldDefOf.BurnedWoodPlankFloor;
        BaseGen.symbolStack.Push("floor", resolveParams4);

        if (rp.addRoomCenterToRootsToUnfog.HasValue && rp.addRoomCenterToRootsToUnfog.Value && Current.ProgramState == ProgramState.MapInitializing)
        {
            MapGenerator.rootsToUnfog.Add(rp.rect.CenterCell);
        }
    }

    private Thing TrySpawnWall(IntVec3 c, ResolveParams rp)
    {
        Map map = BaseGen.globalSettings.map;
        ThingDef wallStuff = rp.wallStuff ?? ThingDefOf.WoodLog;
        List<Thing> thingList = c.GetThingList(map);
        for (int i = 0; i < thingList.Count; i++)
        {
            if (!thingList[i].def.destroyable)
            {
                return null;
            }
            if (thingList[i] is Building_Door)
            {
                return null;
            }
        }
        for (int num = thingList.Count - 1; num >= 0; num--)
        {
            thingList[num].Destroy();
        }
        if (rp.chanceToSkipWallBlock.HasValue && Rand.Chance(rp.chanceToSkipWallBlock.Value))
        {
            return null;
        }
        if (Rand.Chance(0.3f))
        {
            return null;
        }
        ThingDef wallDef = ThingDefOf.WoodLog;
        Thing wall = ThingMaker.MakeThing(wallDef, wallDef.MadeFromStuff ? wallStuff : null);
        wall.SetFaction(rp.faction);
        if (wall.def.useHitPoints)
        {
            wall.HitPoints = Mathf.Max((int)(wall.MaxHitPoints * Rand.Range(0.1f, 1f)), 1);
        }
        return GenSpawn.Spawn(wall, c, map);
    }
}