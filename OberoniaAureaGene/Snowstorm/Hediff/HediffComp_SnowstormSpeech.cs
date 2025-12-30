using OberoniaAurea_Frame;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace OberoniaAureaGene;

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
    public MoteAttached_Text tempMote;

    private HediffCompProperties_SnowstormSpeech Props => props as HediffCompProperties_SnowstormSpeech;
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
                Pawn parentPawn = parent.pawn;
                if (parentPawn.Spawned && parentPawn.Awake() && parentPawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
                {
                    string speech = GenerateGrammarRequest(Props.speechRulePack, Props.speechSection.RandomInRange);
                    ThrowText(speech, Color.white);
                    PostSpeechAction();
                }
                ticksRemaining = Props.speechInterval.RandomInRange;
            }
        }
    }
    protected void ThrowText(string text, Color color, float timeBeforeStartFadeout = -1f)
    {
        if (tempMote is not null && !tempMote.Destroyed)
        {
            tempMote.Destroy(DestroyMode.Vanish);
        }
        MoteAttached_Text mote = (MoteAttached_Text)ThingMaker.MakeThing(OAFrameDefOf.OAFrame_Mote_AttachedText);
        mote.text = text;
        mote.textColor = color;
        if (timeBeforeStartFadeout >= 0f)
        {
            mote.overrideTimeBeforeStartFadeout = timeBeforeStartFadeout;
        }
        tempMote = mote;
        Pawn parentPawn = parent.pawn;
        GenSpawn.Spawn(mote, parentPawn.Position, parentPawn.Map);
        mote.Attach(parentPawn);
    }

    protected virtual void PostSpeechAction() { }
    public override void CompPostPostRemoved()
    {
        if (tempMote is not null && !tempMote.Destroyed)
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
