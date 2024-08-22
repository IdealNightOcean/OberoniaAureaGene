using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public static class OAGene_RatkinUtility
{
    public static bool IsRatkin(this Pawn pawn)
    {
        return pawn.def == OAGene_RatkinDefOf.Ratkin || pawn.def == OAGene_RatkinDefOf.Ratkin_Su;
    }

    public static bool IsRatkinKindomFaction(this Faction faction)
    {
        if (faction == null || faction.def == null)
        {
            return false;
        }
        FactionDef fDef = faction.def;
        if (fDef == OAGene_RatkinDefOf.Rakinia || fDef == OAGene_RatkinDefOf.Rakinia_RockRatkin || fDef == OAGene_RatkinDefOf.Rakinia_SnowRatkin)
        {
            return true;
        }
        return false;
    }
}