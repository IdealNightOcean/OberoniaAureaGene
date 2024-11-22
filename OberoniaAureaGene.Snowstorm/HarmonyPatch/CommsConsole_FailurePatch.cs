using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;


[StaticConstructorOnStartup]
[HarmonyPatch(typeof(Building_CommsConsole), "GetFailureReason")]
public static class CommsConsole_FailurePatch
{
    [HarmonyPostfix]
    public static void Postfix(ref FloatMenuOption __result, Pawn myPawn)
    {
        if (__result == null)
        {
            GameCondition_ExtremeSnowstorm snowstorm = (GameCondition_ExtremeSnowstorm)myPawn.Map?.gameConditionManager.GetActiveCondition(OAGene_MiscDefOf.OAGene_ExtremeSnowstorm);
            if (snowstorm != null && snowstorm.blockCommsconsole)
            {
                __result = new FloatMenuOption("CannotUseReason".Translate("OAGene_CommunicationTowerCollapse".Translate()), null);
            }
        }
    }
}
