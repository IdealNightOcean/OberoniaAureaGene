using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[DefOf]
public static class Snowstrom_MiscDefOf
{
    public static HediffDef OAGene_Hediff_PreparationWarm; //充足御寒准备
    public static HediffDef OAGene_Hediff_HopeForSurvival; //求生的希望
    public static HediffDef OAGene_Hediff_ExperienceSnowstorm; //经历暴风雪

    public static RaidStrategyDef OAGene_SnowstormImmediateAttackBreaching; //暴风雪破墙袭击

    public static ThingDef OAGene_IceStormCrystal;

    public static ThoughtDef OAGene_Thought_SnowstormEnd; //暴风雪结束心情

    public static WeatherDef OAGene_SnowExtreme; //极端暴风雪
    public static WeatherDef OAGene_IceSnowExtreme; //冰晶暴风雪
    static Snowstrom_MiscDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_MiscDefOf));
    }
}

[DefOf]
public static class Snowstrom_IncidentDefOf
{
    public static IncidentDef OAGene_ExtremeIceStorm; //冰晶暴风雪事件
    public static IncidentDef OAGene_SnowstormWarm; //暴风雪的暖和
    public static IncidentDef OAGene_SnowstormCold; //暴风雪的骤冷

    public static IncidentDef OAGene_SnowstormMaliceRaid; //暴风雪破墙袭击
    public static IncidentDef OAGene_SnowstormRaidSource; //暴风雪中的恶意（袭击）
    public static IncidentDef OAGene_SnowstormClimateAdjuster; //暴风雪中的恶意（气候）

    public static IncidentDef OAGene_SnowstromStrugglers; //暴风雪中的挣扎者
    public static IncidentDef OAGene_AffectedMerchant; //暴风雪中的遇难商人

    public static IncidentDef OAGene_AfterSnowstormTraderCaravanArrival; //暴风雪后的商队
    public static IncidentDef OAGene_SnowstormSurvivorJoins; //风雪后的幸存者
    static Snowstrom_IncidentDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Snowstrom_IncidentDefOf));
    }
}