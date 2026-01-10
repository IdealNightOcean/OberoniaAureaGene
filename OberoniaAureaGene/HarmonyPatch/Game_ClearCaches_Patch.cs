using HarmonyLib;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Game), "ClearCaches")]
public static class Game_ClearCaches_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        GoodwillSituationWorker_HasHegemonicFlag.ClearStaticCache();
        ThoughtWorker_Precept_HasHegemonicFlag.ClearStaticCache();
    }
}
[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Game), nameof(Game.InitNewGame))]
public static class Game_InitNewGame_Patch
{
    [HarmonyPrefix]
    public static void Prefix()
    {
        GoodwillSituationWorker_HasHegemonicFlag.ClearStaticCache();
        ThoughtWorker_Precept_HasHegemonicFlag.ClearStaticCache();
    }
}