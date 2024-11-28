using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_ProtagonistHomecoming : HediffCompProperties
{
    public HediffCompProperties_ProtagonistHomecoming()
    {
        compClass = typeof(HediffComp_ProtagonistHomecoming);
    }
}

public class HediffComp_ProtagonistHomecoming : HediffComp
{
    protected bool expectationTrigged;
    protected bool obsessionTrigged;
    protected bool longCherishedTrigged;

    protected string labelInBrackets = null;

    protected int ticksRemaining = 600000;

    public override string CompLabelInBracketsExtra => labelInBrackets;
    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            RecacheThought();
            ticksRemaining = 600000;
        }
    }

    protected void RecacheThought()
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.LongingForHome)
        {
            parent.pawn.health.RemoveHediff(parent);
            return;
        }

        string letterLabel = string.Empty;
        string letterText = string.Empty;
        bool sendLetter = false;

        Pawn protagonist = parent.pawn;
        ThoughtDef homecoming = Snowstrom_ThoughtDefOf.OAGene_Thought_ProtagonistHomecoming;
        int years = GenDate.YearsPassed;
        if (!longCherishedTrigged && years >= 12)
        {
            Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
            if (memory == null)
            {
                memory ??= (Thought_Memory)ThoughtMaker.MakeThought(homecoming);
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
            }
            memory.SetForcedStage(2);
            memory.permanent = true;
            longCherishedTrigged = true;
            labelInBrackets = "OAGene_Homecoming_LongCherished".Translate();
            letterLabel = "OAGene_LetterlabelProtagonistHomecoming_LongCherished".Translate(protagonist.Named("PAWN"));
            letterText = "OAGene_LetterProtagonistHomecoming_LongCherished".Translate(protagonist.Named("PAWN"));
            sendLetter = true;
        }
        else if (!obsessionTrigged && years >= 8)
        {
            Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
            if (memory == null)
            {
                memory ??= (Thought_Memory)ThoughtMaker.MakeThought(homecoming);
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
            }
            memory.SetForcedStage(1);
            memory.durationTicksOverride = 3600000;
            obsessionTrigged = true;
            labelInBrackets = "OAGene_Homecoming_Obsession".Translate();
            letterLabel = "OAGene_LetterlabelProtagonistHomecoming_Obsession".Translate(protagonist.Named("PAWN"));
            letterText = "OAGene_LetterProtagonistHomecoming_Obsession".Translate(protagonist.Named("PAWN"));
            sendLetter = true;
        }
        else if (!expectationTrigged && years >= 4)
        {
            Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
            if (memory == null)
            {
                memory ??= (Thought_Memory)ThoughtMaker.MakeThought(homecoming);
                protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
            }
            memory.SetForcedStage(0);
            memory.durationTicksOverride = 1200000;
            expectationTrigged = true;
            labelInBrackets = "OAGene_Homecoming_Expectation".Translate();
            letterLabel = "OAGene_LetterlabelProtagonistHomecoming_Expectation".Translate(protagonist.Named("PAWN"));
            letterText = "OAGene_LetterProtagonistHomecoming_Expectation".Translate(protagonist.Named("PAWN"));
            sendLetter = true;
        }
        if (sendLetter)
        {
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NegativeEvent, protagonist);
        }
    }

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        RecacheThought();
    }
    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        parent.pawn.needs.mood?.thoughts.memories.RemoveMemoriesOfDef(Snowstrom_ThoughtDefOf.OAGene_Thought_ProtagonistHomecoming);
    }

    public override bool CompDisallowVisible()
    {
        return !expectationTrigged;
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);

        Scribe_Values.Look(ref expectationTrigged, "expectationTrigged", defaultValue: false);
        Scribe_Values.Look(ref obsessionTrigged, "obsessionTrigged", defaultValue: false);
        Scribe_Values.Look(ref longCherishedTrigged, "longCherishedTrigged", defaultValue: false);

        Scribe_Values.Look(ref labelInBrackets, "labelInBrackets", null);
    }
}
