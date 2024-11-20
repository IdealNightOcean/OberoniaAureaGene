using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_ProtagonistDiary : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (Find.TickManager.TicksGame.TicksToDays() < 230f)
        {
            return false;
        }
        GameComponent_SnowstormStory storyComp = Current.Game.GetComponent<GameComponent_SnowstormStory>();
        if(storyComp == null)
        {
            return false;
        }
        if (!storyComp.StoryActive || storyComp.StoryStart)
        {
            return false;
        }
        if (!Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist))
        {
            return false;
        }
        if (protagonist.Dead)
        {
            return false;
        }
        return true;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist) || protagonist.Dead)
        {
            return false;
        }
        float days = Find.TickManager.TicksGame.TicksToDays();
        ThoughtDef homecoming = Snowstrom_ThoughtDefOf.OAGene_Thought_ProtagonistHomecoming;
        string letterLabel = string.Empty;
        string letterText = string.Empty;
        bool sendLetter = false;
        if (days > 715f)
        {
            Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
            if (memory == null)
            {
                memory ??= (Thought_Memory)ThoughtMaker.MakeThought(homecoming);
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
            }
            memory.SetForcedStage(2);
            memory.permanent = true;
            letterLabel = "OAGene_LetterlabelProtagonistHomecoming_LongCherished".Translate(protagonist.Named("PAWN"));
            letterText = "OAGene_LetterProtagonistHomecoming_LongCherished".Translate(protagonist.Named("PAWN"));
            sendLetter = true;
        }
        else if (days > 475f)
        {
            Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
            if (memory == null)
            {
                memory ??= (Thought_Memory)ThoughtMaker.MakeThought(homecoming);
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
            }
            memory.SetForcedStage(1);
            memory.durationTicksOverride = 3600000;
            letterLabel = "OAGene_LetterlabelProtagonistHomecoming_Obsession".Translate(protagonist.Named("PAWN"));
            letterText = "OAGene_LetterProtagonistHomecoming_Obsession".Translate(protagonist.Named("PAWN"));
            sendLetter = true;
        }
        else if (days > 235f)
        {
            Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
            if (memory == null)
            {
                memory ??= (Thought_Memory)ThoughtMaker.MakeThought(homecoming);
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
            }
            memory.SetForcedStage(0);
            memory.durationTicksOverride = 1200000;
            letterLabel = "OAGene_LetterlabelProtagonistHomecoming_Expectation".Translate(protagonist.Named("PAWN"));
            letterText = "OAGene_LetterProtagonistHomecoming_Expectation".Translate(protagonist.Named("PAWN"));
            sendLetter = true;
        }
        if (sendLetter)
        {
            SendStandardLetter(letterLabel, letterText, def.letterDef, parms, protagonist);
        }
        return true;
    }
}
