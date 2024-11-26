using RimWorld;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Ratkin;

public class HediffCompProperties_SnowstormSpeech : HediffCompProperties
{
    public RulePackDef speechRulePack;
    public IntRange speechSection = IntRange.one;
    public IntRange speechInterval = new(2500, 5000);
    public HediffCompProperties_SnowstormSpeech()
    {
        compClass = typeof(HediffComp_SnowstormSpeech);
    }
}

public class HediffComp_SnowstormSpeech : HediffComp
{
    protected int ticksRemaining = 60;
    protected bool humanlike;

    [Unsaved]
    public MoteText tempMote;

    HediffCompProperties_SnowstormSpeech Props => props as HediffCompProperties_SnowstormSpeech;
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        humanlike = parent.pawn.RaceProps.Humanlike && !parent.pawn.IsMutant;

    }
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (humanlike)
        {
            ticksRemaining--;
            if (ticksRemaining <= 0)
            {
                string speech = GenerateGrammarRequest(Props.speechRulePack, Props.speechSection.RandomInRange);
                ThrowText(parent.pawn.DrawPos + new Vector3(0f, 0f, 0.75f), parent.pawn.Map, speech, Color.white);
                ticksRemaining = Props.speechInterval.RandomInRange;
            }
        }
    }
    protected void ThrowText(Vector3 loc, Map map, string text, Color color, float timeBeforeStartFadeout = -1f)
    {
        if (tempMote != null && !tempMote.Destroyed)
        {
            tempMote.Destroy(DestroyMode.Vanish);
        }
        MoteText mote = (MoteText)ThingMaker.MakeThing(ThingDefOf.Mote_Text);
        mote.exactPosition = loc;
        mote.text = text;
        mote.textColor = color;
        if (timeBeforeStartFadeout >= 0f)
        {
            mote.overrideTimeBeforeStartFadeout = timeBeforeStartFadeout;
        }
        tempMote = mote;
        GenSpawn.Spawn(mote, loc.ToIntVec3(), map);
    }
    public override void CompPostPostRemoved()
    {
        if (tempMote != null && !tempMote.Destroyed)
        {
            tempMote.Destroy(DestroyMode.Vanish);
        }
        tempMote = null;
    }
    public override void CompExposeData()
    {
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
        Scribe_Values.Look(ref humanlike, "humanlike", defaultValue: false);
    }

    protected static string GenerateGrammarRequest(RulePackDef rulePack, int speechSection)
    {
        GrammarRequest grammarRequest = default;
        grammarRequest.Includes.Add(rulePack);
        grammarRequest.Constants.Add("speechSection", speechSection.ToString());
        return GenText.CapitalizeAsTitle(GrammarResolver.Resolve("speech_root", grammarRequest));
    }
}
