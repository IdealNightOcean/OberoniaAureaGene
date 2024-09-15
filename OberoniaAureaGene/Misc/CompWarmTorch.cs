using Verse;

namespace OberoniaAureaGene;

public class CompProperties_WarmTorch : CompProperties
{
    public float hypothermiaReducePerHour;
    public int hitPointsReducePerHour;
    public int hitPointsExtraReducePerHourSnowStorm;
    public HediffDef warmTorchHediff;
    public CompProperties_WarmTorch()
    {
        compClass = typeof(CompWarmTorch);
    }
}

public class CompWarmTorch : ThingComp
{
    protected Pawn holder;
    public Pawn Holder => holder;
    CompProperties_WarmTorch Props => props as CompProperties_WarmTorch;

    public void CheckTorch()
    {
        if (holder == null)
        {
            return;
        }
        HealthUtility.AdjustSeverity(holder, OAGene_RimWorldDefOf.Hypothermia, -Props.hypothermiaReducePerHour);
        parent.HitPoints -= Props.hitPointsReducePerHour;
        if (OAGeneUtility.IsSnowExtremeWeather(holder.Map))
        {
            parent.HitPoints -= Props.hitPointsExtraReducePerHourSnowStorm;
        }
        if (parent.HitPoints <= 0 && !parent.Destroyed)
        {
            parent.Destroy();
        }
    }
    public override void Notify_Equipped(Pawn pawn)
    {
        holder = pawn;
        Hediff hediff = holder?.health.AddHediff(Props.warmTorchHediff);
        hediff?.TryGetComp<HediffComp_WarmTorch>()?.InitWarmTorchHediff(parent);
    }
    public override void Notify_Unequipped(Pawn pawn)
    {
        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.warmTorchHediff);
        if (hediff != null)
        {
            pawn.health.RemoveHediff(hediff);
        }
        holder = null;
    }

    public override void PostExposeData()
    {
        Scribe_References.Look(ref holder, "holder");
    }
}
