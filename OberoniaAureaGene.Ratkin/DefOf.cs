using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[DefOf]
public static class OAGene_RatkinDefOf
{
    public static FactionDef Rakinia_TravelRatkin;

    public static HediffDef OAGene_RatkinTail;

    public static JobDef OAGene_HaulToDiscriminator;

    public static ThingDef Ratkin;
    public static ThingDef Ratkin_Su;

    public static WorldObjectDef OAGene_FixedCaravan_Espionage;

    public static XenotypeDef OAGene_RatkinBase;

    static OAGene_RatkinDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_RatkinDefOf));
    }
}
