using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public static class OAGene_RatkinUtility
{
    public static bool IsRatkin(this Pawn pawn)
    {
        return pawn.def == OAGene_RatkinDefOf.Ratkin || pawn.def == OAGene_RatkinDefOf.Ratkin_Su;
    }
}