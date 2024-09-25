using RimWorld;
using Verse;

namespace OberoniaAureaGene;


[DefOf]
public static class OAGene_HediffDefOf
{
    public static HediffDef OAGene_BloodCellsAutophagy;
    public static HediffDef OAGene_DeepSleep;
    public static HediffDef OAGene_SurvivalInstinct;
    public static HediffDef OAGene_XenogermRepairing;

    public static HediffDef OAGene_Hediff_PreparationWarm; //充足御寒准备
    public static HediffDef OAGene_Hediff_ExperienceSnowstorm; //经历暴风雪

    static OAGene_HediffDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_HediffDefOf));
    }
}

[DefOf]
public static class OAGene_GeneDefOf
{
    public static GeneDef OAGene_AbnormalBodyStructure;
    public static GeneDef OAGene_AgriculturalEnthusiasm;
    public static GeneDef OAGene_SpecificHemogen;
    public static GeneDef OAGene_Suspicious;
    public static GeneDef OAGene_BillInspiration;
    public static GeneDef OAGene_MeleeIntouchable;
    static OAGene_GeneDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_GeneDefOf));
    }
}

[DefOf]
public static class OAGene_IncidentDefOf
{
    public static IncidentDef OAGene_ExtremeIceStorm; //冰晶暴风雪事件
    public static IncidentDef OAGene_SnowstormWarm; //暴风雪的暖和
    public static IncidentDef OAGene_SnowstormCold; //暴风雪的骤冷

    static OAGene_IncidentDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_IncidentDefOf));
    }
}

[DefOf]
public static class OAGene_MiscDefOf
{
    public static FleckDef OAGene_ColdGlow; //

    [MayRequireIdeology]
    public static HistoryEventDef OAGene_ThreatBig;
    [MayRequireIdeology]
    public static HistoryEventDef OAGene_PlayerThreatBig;

    public static GameConditionDef OAGene_Snowstorm; //漫长风雪
    public static GameConditionDef OAGene_ExtremeSnowstorm; //极端暴风雪

    public static LetterDef OAGene_SnowstormStart; //暴风雪信件

    public static MusicTransitionDef OAGene_Transition_ClairDeLune; //暴风雪韩流BGM

    public static RaidStrategyDef OAGene_SnowstormImmediateAttackBreaching; //暴风雪破墙袭击

    public static ThoughtDef OAGene_Thought_SnowstormEnd;
    [MayRequireIdeology]
    public static ThingDef OAGene_HegemonicFlag; //霸权旗

    public static TraitDef OAGene_ExtremeSnowSurvivor; //暴风雪幸存者

    public static WeatherDef OAGene_SnowExtreme; //极端暴风雪
    public static WeatherDef OAGene_IceSnowExtreme; //冰晶暴风雪

    static OAGene_MiscDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_MiscDefOf));
    }
}

[DefOf]
public static class OAGene_RimWorldDefOf
{
    public static HediffDef Frail; //体弱
    public static HediffDef Hypothermia; //低温症

    public static TraitDef Faith;
    public static TraitDef Nerves;

    public static WeatherDef SnowHard;
    public static WeatherDef SnowGentle;

    public static WorkTypeDef Tailoring;

    [MayRequireIdeology]
    public static SitePartDef WorkSite_Farming;

    static OAGene_RimWorldDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_RimWorldDefOf));
    }
}
