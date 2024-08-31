using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
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

        Settlement outSettlement = null;

        List<Settlement> settlementList = Find.WorldObjects.SettlementBases;
        Dictionary<Settlement, float> potentialSettle = [];
        float distance = 999999f;
        for (int i = 0; i < settlementList.Count; i++)
        {
            Settlement settle = settlementList[i];
            if (IsGoodSettlement(settle, startSettle, faction, originTile, slate, out distance))
            {
                potentialSettle.Add(settle, distance);
            }
        }
        if (potentialSettle.Any())
        {
            if (nearFirst.GetValue(slate))
            {
                potentialSettle.OrderBy(sd => sd.Value);
                outSettlement = potentialSettle.First().Key;
            }
            else
            {
                outSettlement = potentialSettle.RandomElement().Key;
            }
        }

        if (ignoreConditionsIfNecessary.GetValue(slate) && outSettlement == null)
        {
            outSettlement = Find.WorldObjects.SettlementBases.Where(delegate (Settlement settlement)
            {
                if (!settlement.Visitable || settlement.Faction != faction)
                {
                    return false;
                }
                if (settlement == startSettle)
                {
                    return false;
                }
                return true;
            }).RandomElementWithFallback();
        }
        return outSettlement;

    }
    protected bool IsGoodSettlement(Settlement settlement, Settlement startSettle, Faction faction, int originTile, Slate slate, out float distance)
    {
        distance = 999999f;
        if (settlement == startSettle)
        {
            return false;
        }
        if (!base.IsGoodSettlement(settlement, faction, originTile, slate, out distance))
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
