using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(IncidentWorker_ThrumboPasses), "CanFireNowSub")]
public class ThrumboPasses_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, IncidentParms parms)
    {
        if (__result)
        {
            Map map = (Map)parms.target;
            if (SnowstormUtility.IsSnowExtremeWeather(map))
            {
                __result = false;
            }
        }
    }
}
