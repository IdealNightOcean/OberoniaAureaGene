﻿using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class HediffCompProperties_ProtagonistHomecomed : HediffCompProperties
{
    public ThoughtDef permanentThought;
    public HediffCompProperties_ProtagonistHomecomed()
    {
        compClass = typeof(HediffComp_ProtagonistHomecomed);
    }
}
public class HediffComp_ProtagonistHomecomed : HediffComp
{
    public HediffCompProperties_ProtagonistHomecomed Props => props as HediffCompProperties_ProtagonistHomecomed;

    protected int ticksRemianing;
    protected int cyclesRemianing;
    protected int triggeredCount;

    public override void CompPostMake()
    {
        base.CompPostMake();
        ticksRemianing = 60000;
        cyclesRemianing = 0;
        triggeredCount = 0;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemianing--;
        if (ticksRemianing < 0)
        {  
            cyclesRemianing--;
            ticksRemianing = 10000;
            if (cyclesRemianing <= 0 && TryDoMentalBreak(parent.pawn))
            {
                triggeredCount++;
                cyclesRemianing = 3;
                if (triggeredCount == 3)
                {
                    RecachePermanentThought(parent.pawn, Props.permanentThought, 0);
                }
                else if (triggeredCount == 5)
                {
                    RecachePermanentThought(parent.pawn, Props.permanentThought, 1);
                }
                else if (triggeredCount >= 7)
                {
                    RecachePermanentThought(parent.pawn, Props.permanentThought, 2);
                    parent.pawn.health.RemoveHediff(parent);
                    return;
                }
            }      
        }
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref ticksRemianing, "ticksRemianing", 0);
        Scribe_Values.Look(ref cyclesRemianing, "cyclesRemianing", 0);
        Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
    }

    protected static void RecachePermanentThought(Pawn protagonist, ThoughtDef thoughtDef, int stage)
    {
        Thought_Memory memory = protagonist.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(thoughtDef);
        if (memory == null)
        {
            memory = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);
            protagonist.needs.mood?.thoughts.memories.TryGainMemory(memory);
        }
        memory.permanent = true;
        memory.SetForcedStage(stage);
    }

    protected static bool TryDoMentalBreak(Pawn pawn)
    {
        if(Rand.Chance(0.2f))
        {
            return false;
        }
        if (!pawn.Spawned)
        {
            return false;
        }
        if (!Snowstorm_MiscDefOf.OAGene_LostInMemory.Worker.BreakCanOccur(pawn))
        {
            return false;
        };
        if (OAFrame_MapUtility.ThreatsCountOfPlayerOnMap(pawn.Map) > 0)
        {
            return false;
        }
        return pawn.mindState?.mentalBreaker.TryDoMentalBreak("OAGene_LostInMemoryReason".Translate(), Snowstorm_MiscDefOf.OAGene_LostInMemory) ?? false;
    }
}


