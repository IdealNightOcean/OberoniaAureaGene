using OberoniaAurea_Frame;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class HediffCompProperties_SnowstormOblivious : HediffCompProperties
{
    public HediffDef giverHediff;
    public List<HediffDef> removeHediffs;
    public HediffCompProperties_SnowstormOblivious()
    {
        compClass = typeof(HediffComp_SnowstormOblivious);
    }
}

public class HediffComp_SnowstormOblivious : HediffComp
{
    public HediffCompProperties_SnowstormOblivious Props => props as HediffCompProperties_SnowstormOblivious;

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        Pawn_HealthTracker healthTracker = parent.pawn.health;
        foreach (HediffDef hediffDef in Props.removeHediffs)
        {
            OAFrame_PawnUtility.RemoveFirstHediffOfDef(parent.pawn, hediffDef);
        }
        Hediff_SnowExtremePlayerHidden giverHediff = (Hediff_SnowExtremePlayerHidden)healthTracker.hediffSet.GetFirstHediffOfDef(Props.giverHediff);
        if (giverHediff != null)
        {
            giverHediff.obliviousGived = true;
        }
    }
}
