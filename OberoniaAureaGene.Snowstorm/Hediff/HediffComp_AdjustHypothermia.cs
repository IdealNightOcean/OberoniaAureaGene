using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompPropertiesp_AdjustHypothermia : HediffCompProperties
{
    public HediffDef hediffHuman;
    public HediffDef hediffInsectoid;

    public float severityChange;
    public int severityChangeInterval;
    public HediffCompPropertiesp_AdjustHypothermia()
    {
        compClass = typeof(HediffComp_AdjustHypothermia);
    }
}
public class HediffComp_AdjustHypothermia : HediffComp
{
    protected HediffDef hediffDef;
    protected int ticksRemainings;
    HediffCompPropertiesp_AdjustHypothermia Props => props as HediffCompPropertiesp_AdjustHypothermia;
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        ticksRemainings = Props.severityChangeInterval;
        hediffDef = (parent.pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid) ? Props.hediffInsectoid : Props.hediffHuman;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemainings--;
        if (ticksRemainings < 0)
        {
            HealthUtility.AdjustSeverity(parent.pawn, hediffDef, Props.severityChange);
            ticksRemainings = Props.severityChangeInterval;
        }
    }


    public override void CompExposeData()
    {
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            hediffDef = (parent.pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid) ? Props.hediffInsectoid : Props.hediffHuman;
        }
    }
}
