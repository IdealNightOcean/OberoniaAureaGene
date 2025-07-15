using HarmonyLib;
using OberoniaAureaGene.Snowstorm;
using Verse;

namespace OberoniaAurea;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Game), "ClearCaches")]
public static class Game_ClearCaches_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        Snowstorm_StoryUtility.ClearStaticCache();
    }
}
