using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;
public class HediffCompProperties_SnowstormFog : HediffCompProperties
{
    public ThoughtDef thought;
    public HediffCompProperties_SnowstormFog()
    {
        compClass = typeof(HediffComp_SnowstormFog);
    }
}

public class HediffComp_SnowstormFog : HediffComp
{
    public HediffCompProperties_SnowstormFog Props => props as HediffCompProperties_SnowstormFog;
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        Pawn pawn = parent.pawn;
        if (pawn.RaceProps.IsMechanoid)
        {
            parent.Severity = 2f;
        }
        else if (pawn.RaceProps.Humanlike)
        {
            TraitSet traitSet = parent.pawn.story?.traits;
            if (traitSet is not null && traitSet.HasTrait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor))
            {
                parent.Severity = 2f;
            }
            TryAddMemory();
        }
    }

    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        parent.pawn.needs?.mood?.thoughts?.memories?.RemoveMemoriesOfDef(Props.thought);
    }

    public override void Notify_Spawned()
    {
        Pawn pawn = parent.pawn;
        if (pawn.RaceProps.IsMechanoid)
        {
            parent.Severity = 2f;
        }
        else if (pawn.RaceProps.Humanlike)
        {
            TraitSet traitSet = parent.pawn.story?.traits;
            if (traitSet is not null && traitSet.HasTrait(OAGene_MiscDefOf.OAGene_ExtremeSnowSurvivor))
            {
                parent.Severity = 2f;
            }
            TryAddMemory();
        }
    }
    private void TryAddMemory()
    {
        if (parent.pawn.needs?.mood?.thoughts?.memories?.GetFirstMemoryOfDef(Props.thought) is null)
        {
            int stage = parent.Severity > 1f ? 0 : 1;
            Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(Props.thought);
            thought_Memory.permanent = true;
            thought_Memory.SetForcedStage(stage);
            parent.pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thought_Memory);
        }
    }
}
