using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class HediffCompPropertiesp_ColdImmersion : HediffCompProperties
{
    public HediffDef hediffHuman;
    public HediffDef hediffInsectoid;
    public HediffCompPropertiesp_ColdImmersion()
    {
        compClass = typeof(HediffComp_ColdImmersion);
    }
}

[StaticConstructorOnStartup]
public class HediffComp_ColdImmersion : HediffComp
{
    protected HediffDef hediffDef;
    HediffCompPropertiesp_ColdImmersion Props => props as HediffCompPropertiesp_ColdImmersion;
    public override string CompLabelInBracketsExtra => parent.Severity.ToStringPercent();
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        hediffDef = (parent.pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid) ? Props.hediffInsectoid : Props.hediffHuman;
    }
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (parent.pawn.IsHashIntervalTick(250))
        {
            CheckHypothermia(parent.pawn, hediffDef, parent.Severity);
        }
    }

    protected static void CheckHypothermia(Pawn pawn, HediffDef hediffDef, float immersionSeverity)
    {
        if (pawn.AmbientTemperature < OAGeneUtility.ComfyTemperatureMin(pawn))
        {
            float hypothermiaIncrease;
            if (immersionSeverity < 0.04f)
            {
                hypothermiaIncrease = 0f;
            }
            else
            {
                hypothermiaIncrease = (int)(immersionSeverity % 0.1f) * 0.005f + 0.01f;
            }
            HealthUtility.AdjustSeverity(pawn, hediffDef, hypothermiaIncrease);
        }
    }
    public override void CompExposeData()
    {
        Scribe_Defs.Look(ref hediffDef, "hediffDef");
    }
}
