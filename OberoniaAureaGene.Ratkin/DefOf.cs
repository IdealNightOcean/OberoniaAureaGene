using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[DefOf]
public static class OAGene_RatkinDefOf
{
    public static FactionDef Rakinia; //鼠族王国
    public static FactionDef Rakinia_RockRatkin; //岩鼠王国
    public static FactionDef Rakinia_TravelRatkin; //旅鼠联邦
    public static FactionDef Rakinia_SnowRatkin; //雪鼠王国

    public static HediffDef OAGene_RatkinTail;

    public static HistoryEventDef OAGene_SuspectedBehavior;
    public static HistoryEventDef OAGene_EspionageBehavior;

    public static JobDef OAGene_HaulToDiscriminator;

    public static PawnKindDef RatkinVagabond; //鼠族漂泊者

    public static ThingDef Ratkin;
    public static ThingDef Ratkin_Su;

    [MayRequire("OARK.RatkinFaction.OberoniaAurea")]
    public static ThingDef OAGene_OAGeneBank; //金鸢尾兰基因储存箱

    public static WorldObjectDef OAGene_FixedCaravan_Espionage;

    public static XenotypeDef OAGene_RatkinBase;

    static OAGene_RatkinDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_RatkinDefOf));
    }
}

[DefOf]
public static class OAGene_RatkinGeneDefOf
{
    public static GeneDef OAGene_RatkinEar; //鼠耳基因
    public static GeneDef OAGene_RatkinTail; //鼠尾基因

    static OAGene_RatkinGeneDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_RatkinGeneDefOf));
    }
}
