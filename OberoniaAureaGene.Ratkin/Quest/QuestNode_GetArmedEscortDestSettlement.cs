using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OberoniaAureaGene.Ratkin;

public class QuestNode_GetArmedEscortDestSettlement : QuestNode_GetNearbySettlementOfFaction
{
    public SlateRef<Settlement> startSettlement;
    public SlateRef<int> minEscortTileDistance;
    protected override Settlement RandomNearbySettlement(int originTile, Slate slate)
    {
        Settlement startSettle = startSettlement.GetValue(slate);
        if (startSettle == null || startSettle.Tile < 0)
        {
            return null;
        }
        Faction faction = this.faction.GetValue(slate);
        if (faction == null)
        {
            return null;
        }
        Settlement outSettlement = Find.WorldObjects.SettlementBases.Where(delegate (Settlement settlement)
        {
            return IsGoodSettlement(settlement, startSettle, originTile, slate);
        }).RandomElementWithFallback();

        if (ignoreConditionsIfNecessary.GetValue(slate) && outSettlement == null)
        {
            outSettlement = Find.WorldObjects.SettlementBases.Where(delegate (Settlement settlement)
            {
                if (!settlement.Visitable || settlement.Faction != faction)
                {
                    return false;
                }
                if(settlement == startSettle)
                {
                    return false;
                }
                return true;
            }).RandomElementWithFallback();
        }
        return outSettlement;

    }
    protected bool IsGoodSettlement(Settlement settlement, Settlement startSettle, int originTile, Slate slate)
    {
        if (settlement == startSettle)
        {
            return false;
        }
        if (!base.IsGoodSettlement(settlement, originTile, slate))
        {
            return false;
        }
        if (Find.WorldGrid.ApproxDistanceInTiles(startSettle.Tile, settlement.Tile) < minEscortTileDistance.GetValue(slate))
        {
            return false;
        }
        return true;
    }
}
