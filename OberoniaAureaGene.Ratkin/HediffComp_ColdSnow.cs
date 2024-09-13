using NewRatkin;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene.Ratkin;

public class HediffCompProperties_ColdSnowSpeech : HediffCompProperties
{
    public RulePackDef speechRulePack;
    public HediffCompProperties_ColdSnowSpeech()
    {
        compClass = typeof(HediffComp_ColdSnowSpeech);
    }
}

public class HediffComp_ColdSnowSpeech : HediffComp
{
    protected static readonly IntRange SpeechSection = new(1, 4);
    protected static readonly IntRange SpeechInterval = new(1800, 7200);

    protected int ticksRemaining = 60;
    protected bool humanlike;

    [Unsaved]
    public Mote_CountDown tempMote;

    HediffCompProperties_ColdSnowSpeech Props => props as HediffCompProperties_ColdSnowSpeech;
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        humanlike = parent.pawn.RaceProps.Humanlike;
    }
    public override void CompPostTick(ref float severityAdjustment)
    {
        if (humanlike)
        {
            ticksRemaining--;
            if (ticksRemaining <= 0)
            {
                string speech = GenerateGrammarRequest(Props.speechRulePack);
                ThrowText(parent.pawn.DrawPos + new Vector3(0f, 0f, 0.75f), parent.pawn.Map, speech, Color.white);
                ticksRemaining = SpeechInterval.RandomInRange;
            }
        }
    }
    protected void ThrowText(Vector3 loc, Map map, string text, Color color, float timeBeforeStartFadeout = -1f)
    {
        if (tempMote != null && !tempMote.Destroyed)
        {
            tempMote.Destroy(DestroyMode.Vanish);
        }
        Mote_CountDown mote = (Mote_CountDown)ThingMaker.MakeThing(RatkinMoteDefOf.Mote_CountDown);
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

    protected static string GenerateGrammarRequest(RulePackDef rulePack)
    {
        GrammarRequest grammarRequest = default;
        grammarRequest.Includes.Add(rulePack);
        int speechSection = SpeechSection.RandomInRange;
        grammarRequest.Constants.Add("speechSection", speechSection.ToString());
        return GenText.CapitalizeAsTitle(GrammarResolver.Resolve("speech_root", grammarRequest));
    }
}
