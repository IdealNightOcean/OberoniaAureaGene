using HarmonyLib;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(GenTemperature), "GetTemperatureFromSeasonAtTile")]
public static class GetTemperatureFromSeasonAtTile_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref float __result)
    {
        if (GameComponent_Snowstorm.Instance.SnowstormNow)
        {
            __result -= 10f;
        }
    }
}