using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace OberoniaAureaGene.Ratkin;

public class QuestNode_GetArmedEscortSettlement : QuestNode
{
    [NoTranslate]
    public SlateRef<string> storeStartSettleAs;
    [NoTranslate]
    public SlateRef<string> storeDestSettleAs;


    public SlateRef<int> originTile = -1;
    public SlateRef<float> maxTileDistance;
    public SlateRef<bool> preferCloser = true;
    public SlateRef<Faction> faction;
    public SlateRef<int> minEscortTileDistance;

    protected override bool TestRunInt(Slate slate)
    {
        return SetVars(slate);
    }

    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        SetVars(slate);
    }

    protected bool SetVars(Slate slate)
    {
        Faction faction = this.faction.GetValue(slate);
        if (faction is null)
        {
            return false;
        }
        int originTile = this.originTile.GetValue(slate);
        if (originTile < 0)
        {
            Map map = slate.Get<Map>("map");
            originTile = map.Tile;
        }
        if (VaildSettlementPair(slate, originTile, faction, out Pair<Settlement, Settlement> validSettlePair))
        {
            if (validSettlePair.First is null || validSettlePair.Second is null)
            {
                return false;
            }
            else
            {
                slate.Set(storeStartSettleAs.GetValue(slate), validSettlePair.First);
                slate.Set(storeDestSettleAs.GetValue(slate), validSettlePair.Second);
                return true;
            }
        }
        return false;
    }

    public bool VaildSettlementPair(Slate slate, int originTile, Faction faction, out Pair<Settlement, Settlement> validSettlePair)
    {
        validSettlePair = new Pair<Settlement, Settlement>(null, null);
        List<Settlement> settlementList = Find.WorldObjects.SettlementBases;
        List<Pair<Settlement, float>> potentialSettle = [];
        for (int i = 0; i < settlementList.Count; i++)
        {
            Settlement settle = settlementList[i];
            if (IsGoodSettlement(settle, faction, originTile, slate, out float distance))
            {
                potentialSettle.Add(new Pair<Settlement, float>(settle, distance));
            }
        }
        if (potentialSettle.Count < 2)
        {
            return false;
        }

        int matrixLen = potentialSettle.Count;
        WorldGrid worldGrid = Find.WorldGrid;
        List<Pair<Pair<Settlement, Settlement>, float>> potentialPair = [];
        float minEscortTileDistance = this.minEscortTileDistance.GetValue(slate);
        for (int i = 0; i < matrixLen; i++)
        {
            Settlement startSettle = potentialSettle[i].First;
            for (int j = i + 1; j < matrixLen; j++)
            {
                Settlement destSettle = potentialSettle[j].First;
                float sdDistance = worldGrid.ApproxDistanceInTiles(startSettle.Tile, destSettle.Tile);
                if (sdDistance >= minEscortTileDistance)
                {
                    potentialPair.Add(new Pair<Pair<Settlement, Settlement>, float>(new Pair<Settlement, Settlement>(startSettle, destSettle), potentialSettle[i].Second + potentialSettle[j].Second));
                }
            }
        }
        if (!potentialPair.Any())
        {
            return false;
        }

        if (preferCloser.GetValue(slate))
        {
            potentialPair.OrderBy(p => p.Second);
            validSettlePair = potentialPair.First().First;
        }
        else
        {
            validSettlePair = potentialPair.RandomElement().First;
        }

        return true;
    }

    protected bool IsGoodSettlement(Settlement settlement, Faction faction, int originTile, Slate slate, out float distance)
    {
        distance = 999999f;
        if (!settlement.Visitable || settlement.Faction != faction)
        {
            return false;
        }
        distance = Find.WorldGrid.ApproxDistanceInTiles(originTile, settlement.Tile);
        if (distance > maxTileDistance.GetValue(slate))
        {
            return false;
        }
        if (!Find.WorldReachability.CanReach(originTile, settlement.Tile))
        {
            return false;
        }
        return true;
    }
}
