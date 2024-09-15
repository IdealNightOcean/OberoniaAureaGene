using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class CompProperties_WarmTorch : CompProperties
{
    public float hypothermiaReducePerHour;
    public int hitPointsReducePerHour;
    public int hitPointsExtraReducePerHourSnowStorm;

    public CompProperties_WarmTorch()
    {
        compClass = typeof(CompProperties_WarmTorch);
    }
}

public class CompWarmTorch : ThingComp
{
    public int ticksRemainings = 2500;

    CompProperties_WarmTorch Props => props as CompProperties_WarmTorch;
    public override void CompTickRare()
    {
        ticksRemainings -= 250;
        if (ticksRemainings <= 0)
        {
            CheckTorch();
            ticksRemainings = 2500;
        }
    }
    protected void CheckTorch()
    {
        Pawn wear = ((Apparel)parent)?.Wearer;
        if (wear == null)
        {
            return;
        }
        HealthUtility.AdjustSeverity(wear, OAGene_RimWorldDefOf.Hypothermia, -Props.hypothermiaReducePerHour);
        parent.HitPoints -= Props.hitPointsReducePerHour;
        if (OAGeneUtility.IsSnowExtremeWeather(wear.Map))
        {
            parent.HitPoints -= Props.hitPointsExtraReducePerHourSnowStorm;
        }
    }
}
