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
public static class OberoniaAureaGeneDefOf
{
    [MayRequireIdeology]
    public static HistoryEventDef OAGene_ThreatBig;
    [MayRequireIdeology]
    public static HistoryEventDef OAGene_PlayerThreatBig;

    public static GameConditionDef OAGene_Snowstorm; //漫长风雪
    public static GameConditionDef OAGene_ExtremeSnowstorm; //极端暴风雪

    public static LetterDef OAGene_SnowstormColdSnap; //暴风雪寒流信件

    public static SongDef OAGene_ExtremeSnowstormStart; //暴风雪BGM

    [MayRequireIdeology]
    public static ThingDef OAGene_HegemonicFlag; //霸权旗

    public static TraitDef OAGene_ExtremeSnowSurvivor; //暴风雪幸存者

    public static WeatherDef OAGene_SnowExtreme; //极端暴风雪

    static OberoniaAureaGeneDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OberoniaAureaGeneDefOf));
    }
}

[DefOf]
public static class OAGene_RimWorldDefOf
{
    public static HediffDef Frail;

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
