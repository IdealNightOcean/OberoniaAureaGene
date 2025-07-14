using HarmonyLib;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(CaravanTicksPerMoveUtility), "GetTicksPerMove",
    [typeof(List<Pawn>), typeof(float), typeof(float), typeof(StringBuilder)])]
public static class GetTicksPerMove_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref int __result, List<Pawn> pawns, float massUsage, float massCapacity, StringBuilder explanation = null)
    {
        if (__result == 3300)
        {
            return;
        }
        if (Snowstorm_MiscUtility.SnowstormGameComp.SnowstormNow)
        {
            __result = (int)(__result * 5f);
            if (explanation is not null)
            {
                explanation.AppendLine();
                explanation.Append("  " + "OAGene_MultiplierForSnowstorm".Translate(0.2f.ToStringPercent()));
            }
        }
    }
}
