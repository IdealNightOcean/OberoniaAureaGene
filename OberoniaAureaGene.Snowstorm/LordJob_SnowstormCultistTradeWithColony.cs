using RimWorld;
using Verse;
using Verse.AI.Group;

namespace OberoniaAureaGene.Snowstorm;

public class LordJob_SnowstormCultistTradeWithColony : LordJob_TradeWithColony
{
    public LordJob_SnowstormCultistTradeWithColony(Faction faction, IntVec3 chillSpot) : base(faction, chillSpot) { }
    public override void Notify_PawnLost(Pawn p, PawnLostCondition condition)
    {
        base.Notify_PawnLost(p, condition);
        if (condition == PawnLostCondition.MadePrisoner)
        {
            if (p.Faction != null && !p.Faction.HostileTo(Faction.OfPlayer))
            {
                p.Faction.SetRelationDirect(Faction.OfPlayer, FactionRelationKind.Hostile);
            }
        }
    }
}
