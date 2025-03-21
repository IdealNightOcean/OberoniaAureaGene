﻿using RimWorld;
using Verse;

namespace OberoniaAureaGene;


[DefOf]
public static class OAGene_HediffDefOf
{
    public static HediffDef OAGene_BloodCellsAutophagy;
    public static HediffDef OAGene_DeepSleep;
    public static HediffDef OAGene_SurvivalInstinct;
    public static HediffDef OAGene_XenogermRepairing;

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
public static class OAGene_MiscDefOf
{
    public static FleckDef OAGene_ColdGlow; //暴风雪冰晶特效
    public static FleckDef OAGene_BigColdGlow; //暴风雪冰晶特效（大）

    [MayRequireIdeology]
    public static HistoryEventDef OAGene_ThreatBig;
    [MayRequireIdeology]
    public static HistoryEventDef OAGene_PlayerThreatBig;

    public static GameConditionDef OAGene_Snowstorm; //漫长风雪
    public static GameConditionDef OAGene_ExtremeSnowstorm; //极端暴风雪

    public static LetterDef OAGene_SnowstormStart; //暴风雪信件

    public static MusicTransitionDef OAGene_Transition_ClairDeLune; //暴风雪寒流BGM

    [MayRequireIdeology]
    public static ThingDef OAGene_HegemonicFlag; //霸权旗

    [MayRequire("OARK.RatkinFaction.OberoniaAurea")]
    public static ThingDef OAGene_OAGeneBank; //金鸢尾兰基因储存箱

    public static ThoughtDef OAGene_Thought_SnowstormEnd; //暴风雪结束心情

    [MayRequire("Solaris.RatkinRaceMod")]
    public static TraitDef Faith; //信念坚定
    public static TraitDef OAGene_ExtremeSnowSurvivor; //暴风雪幸存者

    public static WeatherDef OAGene_SnowExtreme; //极端暴风雪

    [MayRequire("OARK.RatkinFaction.ScriptExpand.Snow")]
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

    public static IncidentDef ColdSnap; //寒潮

    public static TraitDef Nerves;

    public static WeatherDef SnowHard;
    public static WeatherDef SnowGentle;

    public static WorkTypeDef Tailoring;

    public static MentalBreakDef Wander_Psychotic;

    [MayRequireIdeology]
    public static SitePartDef WorkSite_Farming;

    static OAGene_RimWorldDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_RimWorldDefOf));
    }
}
