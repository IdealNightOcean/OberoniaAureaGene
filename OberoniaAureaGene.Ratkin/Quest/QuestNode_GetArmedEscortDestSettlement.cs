using OberoniaAurea_Frame;
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
    public SlateRef<WorldObject> startSettlement;
    public SlateRef<int> minEscortTileDistance;

    protected override bool ValidSettlement(Settlement settlement, Slate slate)
    {
        int originTile = startSettlement.GetValue(slate)?.Tile ?? -1;
        if (originTile < 0)
        {
            return false;
        }
        if (Find.WorldGrid.ApproxDistanceInTiles(originTile, settlement.Tile) < minEscortTileDistance.GetValue(slate))
        {
            return false;
        }
        return true;
    }
}
