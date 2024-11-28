using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(IncidentWorker_HerdMigration), "CanFireNowSub")]
public static class HerdMigration_CanFirePatch
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, IncidentParms parms)
    {
        if (!__result)
        {
            return;
        }
        Map map = (Map)parms.target;
        __result = !SnowstormUtility.IsSnowExtremeWeather(map);
    }
}
