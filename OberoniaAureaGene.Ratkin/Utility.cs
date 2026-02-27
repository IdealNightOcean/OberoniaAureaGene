using System.Runtime.CompilerServices;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public static class OAGene_RatkinUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRatkin(this Pawn pawn) => pawn.def == OAGene_RatkinDefOf.Ratkin;
}