using OberoniaAurea_Frame;
using Verse;

namespace OberoniaAureaGene;

public class HediffCompProperties_SnowstormHiden : HediffCompProperties
{
    public HediffDef depressHediff;
    public HediffDef underDarkHediff;
    public HediffCompProperties_SnowstormHiden()
    {
        compClass = typeof(HediffComp_SnowstormHiden);
    }
}
public class HediffComp_SnowstormHiden : HediffComp
{
    HediffCompProperties_SnowstormHiden Props => props as HediffCompProperties_SnowstormHiden;
    public override void CompPostPostRemoved()
    {
        Pawn pawn = parent.pawn;
        OberoniaAureaFrameUtility.RemoveFirstHediffOfDef(pawn, Props.depressHediff);
        OberoniaAureaFrameUtility.RemoveFirstHediffOfDef(pawn, Props.underDarkHediff);
    }
}
