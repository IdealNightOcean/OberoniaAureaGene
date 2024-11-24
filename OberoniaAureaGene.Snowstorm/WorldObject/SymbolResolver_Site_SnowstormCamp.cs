using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class SymbolResolver_Site_SnowstormCamp : SymbolResolver
{
    public override void Resolve(ResolveParams rp)
    {
        CellRect rect = rp.rect;
        CellRect rect2 = rect.ContractedBy(1);
        List<Thing> stockList = [];
        TraderKindDef traderKindDef = Snowstrom_MiscDefOf.OAGene_Trader_SnowstormCamp;
        int forTile = BaseGen.globalSettings.map?.Tile ?? -1;
        for (int i = 0; i < traderKindDef.stockGenerators.Count; i++)
        {
            foreach (Thing item in traderKindDef.stockGenerators[i].GenerateThings(forTile, rp.faction))
            {
                if (item is Pawn || item is Building)
                {
                    item.Destroy(DestroyMode.KillFinalize);
                    continue;
                }
                AddThing(stockList, item);
            }
        }
        foreach (Thing item2 in stockList)
        {
            CompForbiddable compForbiddable = item2.TryGetComp<CompForbiddable>();
            if (compForbiddable != null)
            {
                compForbiddable.Forbidden = true;
            }
        }
        BaseGen.symbolStack.Push("stockpile", new ResolveParams
        {
            rect = rect2,
            stockpileConcreteContents = stockList,
            skipSingleThingIfHasToWipeBuildingOrDoesntFit = true,
        });
        BaseGen.symbolStack.Push("oaframe_EmptyRoom", rp);
    }

    protected static void AddThing(List<Thing> outThings, Thing t)
    {
        if (t.stackCount <= t.def.stackLimit)
        {
            outThings.Add(t);
        }
        else
        {
            int exceededStack = t.stackCount - t.def.stackLimit;
            t.stackCount = t.def.stackLimit;
            outThings.Add(t);
            outThings.AddRange(OAFrame_MiscUtility.TryGenerateThing(t.def, exceededStack));
        }
    }
}