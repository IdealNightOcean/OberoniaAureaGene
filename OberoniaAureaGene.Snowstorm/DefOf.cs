using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[DefOf]
public static class OAGene_SnowstromDefOf
{
    public static HediffDef OAGene_Hediff_PreparationWarm; //充足御寒准备
    public static HediffDef OAGene_Hediff_ExperienceSnowstorm; //经历暴风雪

    public static IncidentDef OAGene_ExtremeIceStorm; //冰晶暴风雪事件
    public static IncidentDef OAGene_SnowstormWarm; //暴风雪的暖和
    public static IncidentDef OAGene_SnowstormCold; //暴风雪的骤冷
    public static IncidentDef OAGene_AfterSnowstormTraderCaravanArrival; //暴风雪后的商队

    public static RaidStrategyDef OAGene_SnowstormImmediateAttackBreaching; //暴风雪破墙袭击

    public static ThoughtDef OAGene_Thought_SnowstormEnd; //暴风雪结束心情

    public static WeatherDef OAGene_SnowExtreme; //极端暴风雪
    public static WeatherDef OAGene_IceSnowExtreme; //冰晶暴风雪
    static OAGene_SnowstromDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_SnowstromDefOf));
    }
}