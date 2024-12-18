using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_ColdPreparation : HediffCompProperties
{
    public HediffDef sequelaHediff;

    public HediffCompProperties_ColdPreparation()
    {
        compClass = typeof(HediffComp_ColdPreparation);
    }
}
public class HediffComp_ColdPreparation : HediffComp
{
    public HediffCompProperties_ColdPreparation Props => props as HediffCompProperties_ColdPreparation;

    public override void CompPostPostRemoved()
    {
        if (!parent.pawn.Dead)
        {
            parent.pawn.health.AddHediff(Props.sequelaHediff);
        }
    }
}
