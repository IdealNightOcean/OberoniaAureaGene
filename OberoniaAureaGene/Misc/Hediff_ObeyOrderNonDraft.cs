using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class Hediff_ObeyOrderNonDraft : HediffWithComps
{
    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);
        int stageIndex = CurStageIndex;
        Thought_Memory memory = pawn.needs?.mood.thoughts.memories.GetFirstMemoryOfDef(OAGene_MiscDefOf.OAGene_Thought_ObeyOrderNonDraft);
        if (memory is null)
        {
            memory = ThoughtMaker.MakeThought(OAGene_MiscDefOf.OAGene_Thought_ObeyOrderNonDraft, stageIndex);
            memory.permanent = true;
            pawn.needs?.mood.thoughts.memories.TryGainMemory(memory);
        }
        else
        {
            memory.SetForcedStage(stageIndex);
        }
    }

    protected override void OnStageIndexChanged(int stageIndex)
    {
        base.OnStageIndexChanged(stageIndex);
        Thought_Memory memory = pawn.needs?.mood.thoughts.memories.GetFirstMemoryOfDef(OAGene_MiscDefOf.OAGene_Thought_ObeyOrderNonDraft);
        if (memory is null)
        {
            memory = ThoughtMaker.MakeThought(OAGene_MiscDefOf.OAGene_Thought_ObeyOrderNonDraft, stageIndex);
            memory.permanent = true;
            pawn.needs?.mood.thoughts.memories.TryGainMemory(memory);
        }
        else
        {
            memory.SetForcedStage(stageIndex);
        }
    }

    public override void PostRemoved()
    {
        base.PostRemoved();
        Log.Message("removed?");
        pawn.needs?.mood.thoughts.memories.RemoveMemoriesOfDef(OAGene_MiscDefOf.OAGene_Thought_ObeyOrderNonDraft);
    }
}
