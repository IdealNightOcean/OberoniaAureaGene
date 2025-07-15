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
