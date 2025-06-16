using Verse;

namespace OberoniaAureaGene;

public class HediffCompProperties_WarmTorch : HediffCompProperties
{
    public HediffCompProperties_WarmTorch()
    {
        compClass = typeof(HediffComp_WarmTorch);
    }

}

public class HediffComp_WarmTorch : HediffComp
{
    protected Thing warmTorch;

    [Unsaved]
    protected CompWarmTorch warmTorchComp;
    protected CompWarmTorch WarmTorchComp => warmTorchComp ??= warmTorch?.TryGetComp<CompWarmTorch>();

    public int ticksRemainings = 2500;
    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemainings--;
        if (ticksRemainings < 0)
        {
            CheckTorch();
            ticksRemainings = 2500;
        }
    }
    public void InitWarmTorchHediff(Thing torch)
    {
        warmTorch = torch;
        CompWarmTorch torchComp = WarmTorchComp;
        if (torchComp is null || torchComp.Holder != parent.pawn)
        {
            parent.pawn.health.RemoveHediff(parent);
        }
    }
    protected void CheckTorch()
    {
        Pawn holder = parent.pawn;
        CompWarmTorch torchComp = WarmTorchComp;
        if (torchComp is null || torchComp.Holder != holder)
        {
            holder.health.RemoveHediff(parent);
            return;
        }
        torchComp.CheckTorch();
    }

    public override void CompExposeData()
    {
        Scribe_Values.Look(ref ticksRemainings, "ticksRemainings", 2500);
        Scribe_References.Look(ref warmTorch, "warmTorch");
    }
}
