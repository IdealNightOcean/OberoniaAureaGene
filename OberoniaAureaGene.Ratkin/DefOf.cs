using RimWorld;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[DefOf]
public static class OAGene_RatkinDefOf
{
    public static ThingDef Ratkin;
    public static ThingDef Ratkin_Su;

    //public static GeneDef OAGene_RatkinEar;
    //public static GeneDef OAGene_RatkinTail;
    //public static GeneDef OAGene_RatkinBody;

    static OAGene_RatkinDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(OAGene_RatkinDefOf));
    }
}
