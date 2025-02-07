using RimWorld;
using UnityEngine;
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

    protected int stage;
    protected int Stage
    {
        get { return stage; }
        set { stage = Mathf.Clamp(value, 0, 6); }
    }

    protected int ticksRemaining = 600000;

    protected bool longCherishedTrigged;

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            RecacheDiaryAndThought();
            ticksRemaining = 600000;
        }
    }

    protected void RecacheDiaryAndThought(bool slience = false)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.LongingForHome)
        {
            parent.pawn.health.RemoveHediff(parent);
            return;
        }
        Pawn protagonist = parent.pawn;
        int years = GenDate.YearsPassed;
        switch (Stage)
        {
            case 0:
                if (years >= 2)
                {
                    Stage++;
                    SendDiaryLetter(protagonist, Stage, slience);
                }
                break;
            case 1:
                if (years >= 4)
                {
                    RecacheThought(0, 1200000);
                    Stage++;
                    SendDiaryLetter(protagonist, Stage, slience);
                }
                break;
            case 2:
                if (years >= 6)
                {
                    Stage++;
                    SendDiaryLetter(protagonist, Stage, slience);
                }
                break;
            case 3:
                if (years >= 8)
                {
                    RecacheThought(1, 3600000);
                    Stage++;
                    SendDiaryLetter(protagonist, Stage, slience);
                }
                break;
            case 4:
                if (years >= 10)
                {
                    Stage++;
                    SendDiaryLetter(protagonist, Stage, slience);
                }
                break;
            case 5:
                if (years >= 12 && !longCherishedTrigged)
                {
                    RecacheThought(2, -1, true);
                    longCherishedTrigged = true;
                    Stage++;
                    SendDiaryLetter(protagonist, Stage, slience);
                }
                break;
            case 6:
                if (!longCherishedTrigged)
                {
                    RecacheThought(2, -1, true);
                    longCherishedTrigged = true;
                    Stage = 6;
                    SendDiaryLetter(protagonist, Stage, slience);
                }
                break;
            default: break;
        }
    }

    public static void SendDiaryLetter(Pawn protagonist, int stage, bool slience = false)
    {
        if (slience)
        {
            return;
        }
        int curYear = 5500 + GenDate.YearsPassed;
        TaggedString letterLabel = $"OAGene_LetterlabelProtagonistHomecoming_{stage}".Translate(protagonist.Named("PAWN"));
        TaggedString letterText = $"OAGene_LetterProtagonistHomecoming_{stage}".Translate(protagonist.Named("PAWN"), curYear);
        Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NegativeEvent, protagonist);
    }
    public void RecacheThought(int thoughtStage, int durationTicksOverride = -1, bool permanent = false)
    {
        Pawn protagonist = parent.pawn;
        ThoughtDef homecoming = Snowstorm_ThoughtDefOf.OAGene_Thought_ProtagonistHomecoming;
        Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
        if (memory == null)
        {
            memory = ThoughtMaker.MakeThought(homecoming, thoughtStage);
            memory.permanent = permanent;
            protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
        }
        parent.Severity = thoughtStage;

        memory.SetForcedStage(thoughtStage);
        memory.permanent = permanent;
        if (durationTicksOverride > 0)
        {
            memory.durationTicksOverride = durationTicksOverride;
        }
    }

    public void RecacheDiaryAndThoughtNow(bool slience)
    {
        int years = GenDate.YearsPassed;
        Stage = years / 2;
        RecacheDiaryAndThought(slience);
    }
    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        parent.pawn.needs.mood?.thoughts.memories.RemoveMemoriesOfDef(Snowstorm_ThoughtDefOf.OAGene_Thought_ProtagonistHomecoming);
    }

    public override void CompPostMerged(Hediff other)
    {
        base.CompPostMerged(other);
        HediffComp_ProtagonistHomecoming otherHomecomingComp = other.TryGetComp<HediffComp_ProtagonistHomecoming>();
        if (otherHomecomingComp != null)
        {
            stage = Mathf.Max(stage, otherHomecomingComp.stage);
            longCherishedTrigged |= otherHomecomingComp.longCherishedTrigged;
        }
    }
    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref stage, "stage", 0);
        Scribe_Values.Look(ref longCherishedTrigged, "longCherishedTrigged", defaultValue: false);
    }
}
