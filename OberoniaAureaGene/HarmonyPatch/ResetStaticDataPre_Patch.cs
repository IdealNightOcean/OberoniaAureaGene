using HarmonyLib;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(PlayDataLoader), "ResetStaticDataPre")]
public static class ResetStaticDataPre_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {

        GoodwillSituationWorker_HasHegemonicFlag.ResetStaticCache();

        ThoughtWorker_Precept_HasHegemonicFlag.ResetStaticCache();
    }
}
