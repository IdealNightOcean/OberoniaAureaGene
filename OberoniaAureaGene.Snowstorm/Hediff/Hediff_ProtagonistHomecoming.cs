using RimWorld;
using System.Text;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class Hediff_ProtagonistHomecoming : HediffWithComps
{
    protected int diaryStage;
    protected int DiaryStage
    {
        get { return diaryStage; }
        set { diaryStage = Mathf.Clamp(value, 0, 6); }
    }

    protected int ticksRemaining = 600000;

    protected bool longCherishedTrigged;

    public override string LabelInBrackets
    {
        get
        {
            StringBuilder stringBuilder = new(base.LabelInBrackets);
            stringBuilder.Append(", ");
            stringBuilder.Append(diaryStage);
            return stringBuilder.ToString();
        }
    }

    public override void Tick()
    {
        base.Tick();
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            RecacheDiaryAndThought();
            ticksRemaining = 600000;
        }
    }

    protected void RecacheDiaryAndThought(bool slience = false)
    {
        if (!GameComponent_SnowstormStory.Instance.LongingForHome)
        {
            pawn.health.RemoveHediff(this);
            return;
        }
        Pawn protagonist = pawn;
        int years = GenDate.YearsPassed;
        switch (DiaryStage)
        {
            case 0:
                if (years >= 2)
                {
                    DiaryStage++;
                    SendDiaryLetter(protagonist, DiaryStage, slience);
                }
                break;
            case 1:
                if (years >= 4)
                {
                    RecacheThought(0, 1200000);
                    DiaryStage++;
                    SendDiaryLetter(protagonist, DiaryStage, slience);
                }
                break;
            case 2:
                if (years >= 6)
                {
                    DiaryStage++;
                    SendDiaryLetter(protagonist, DiaryStage, slience);
                }
                break;
            case 3:
                if (years >= 8)
                {
                    RecacheThought(1, 3600000);
                    DiaryStage++;
                    SendDiaryLetter(protagonist, DiaryStage, slience);
                }
                break;
            case 4:
                if (years >= 10)
                {
                    DiaryStage++;
                    SendDiaryLetter(protagonist, DiaryStage, slience);
                }
                break;
            case 5:
                if (years >= 12 && !longCherishedTrigged)
                {
                    RecacheThought(2, -1, true);
                    longCherishedTrigged = true;
                    DiaryStage++;
                    SendDiaryLetter(protagonist, DiaryStage, slience);
                }
                break;
            case 6:
                if (!longCherishedTrigged)
                {
                    RecacheThought(2, -1, true);
                    longCherishedTrigged = true;
                    DiaryStage = 6;
                    SendDiaryLetter(protagonist, DiaryStage, slience);
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
        Pawn protagonist = pawn;
        ThoughtDef homecoming = Snowstorm_ThoughtDefOf.OAGene_Thought_ProtagonistHomecoming;
        Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(homecoming);
        if (memory is null)
        {
            memory = ThoughtMaker.MakeThought(homecoming, thoughtStage);
            memory.permanent = permanent;
            protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
        }
        Severity = thoughtStage;

        memory.SetForcedStage(thoughtStage);
        memory.permanent = permanent;
        if (durationTicksOverride > 0)
        {
            memory.durationTicksOverride = durationTicksOverride;
        }
    }

    public void RecacheDiaryAndThoughtNow(bool slience)
    {
        DiaryStage = GenDate.YearsPassed / 2;
        RecacheDiaryAndThought(slience);
    }

    public override void PostRemoved()
    {
        base.PostRemoved();
        pawn.needs.mood?.thoughts.memories.RemoveMemoriesOfDef(Snowstorm_ThoughtDefOf.OAGene_Thought_ProtagonistHomecoming);
    }

    public override bool TryMergeWith(Hediff other)
    {
        if (other is null || other.def != def)
        {
            return false;
        }
        if (other is Hediff_ProtagonistHomecoming otherHomecoming)
        {
            diaryStage = Mathf.Max(diaryStage, otherHomecoming.diaryStage);
            longCherishedTrigged = longCherishedTrigged || otherHomecoming.longCherishedTrigged;

            for (int i = 0; i < comps.Count; i++)
            {
                comps[i].CompPostMerged(other);
            }
            return true;
        }
        return false;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref diaryStage, "diaryStage", 0);
        Scribe_Values.Look(ref longCherishedTrigged, "longCherishedTrigged", defaultValue: false);
    }
}
