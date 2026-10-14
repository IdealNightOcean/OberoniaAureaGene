using OberoniaAurea_Frame;
using OberoniaAurea_Frame.Utility;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public static class Snowstorm_MiscUtility
{
    public static MapComponent_Snowstorm SnowstormMapComp(this Map map)
    {
        return map?.GetComponent<MapComponent_Snowstorm>();
    }

    private static readonly HediffGiveParams ColdPreparationHediffGiveParams = new();

    public static void SetColdPreparation(Pawn pawn, HediffDef coldPreparation)
    {
        pawn.health.AddHediff(coldPreparation);
        if (Rand.Chance(0.15f))
        {
            ColdPreparationHediffGiveParams.HediffToGive = HediffDefOf.AlcoholHigh;
            ColdPreparationHediffGiveParams.InitSeverity = Rand.Range(0.1f, 0.5f);
            pawn.GetOrAddHediff(ColdPreparationHediffGiveParams);
        }
    }
}