using OberoniaAurea_Frame;
using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstorm : GameCondition_ExtremeSnowstormBase
{
    public bool blockCommsconsole;

    protected Map mainMap;
    public virtual Map MainMap
    {
        get
        {
            if (mainMap == null)
            {
                if (gameConditionManager.ownerMap != null)
                {
                    mainMap = gameConditionManager.ownerMap;
                }
                else
                {
                    mainMap = AffectedMaps.Where(m => m.IsPlayerHome).RandomElementWithFallback(null);
                    mainMap ??= Find.AnyPlayerHomeMap;
                }
            }
            return mainMap;
        }
    }
    public void EndSlience()
    {
        suppressEndMessage = true;
        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormEnd();
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstorm_AllMaps(map, slience: true);
        }
        base.End();
    }
    protected override void PostInit()
    {
        TryAddColdSnap();

        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormStart();

        int duration = Duration;
        SnowstormUtility.InitExtremeSnowstorm_World(duration);
        SnowstormUtility.InitExtremeSnowstorm_MainMap(MainMap, duration);
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.InitExtremeSnowstorm_AllMaps(map, duration);
        }
    }
    protected override void PreEnd()
    {
        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormEnd();
        SnowstormUtility.EndExtremeSnowstorm_MainMap(MainMap);
        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstorm_AllMaps(map);
        }
    }
    protected new void TryAddColdSnap()
    {
        if (Rand.Chance(0.3f))
        {
            if (MainMap != null)
            {
                IncidentParms parms = new()
                {
                    target = MainMap,
                };
                if (OAFrame_MiscUtility.TryFireIncidentNow(OAGene_RimWorldDefOf.ColdSnap, parms))
                {
                    Letter letter = LetterMaker.MakeLetter("OAGene_ExtremeSnowstormCauseColdSnapTitle".Translate(), "OAGene_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
                    Find.LetterStack.ReceiveLetter(letter, playSound: false);
                    Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
                    causeColdSnap = true;
                }
            }
        }
    }
    public void SetMainMap(Map map)
    {
        if (map != null)
        {
            mainMap = map;
        }
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref mainMap, "mainMap");
        Scribe_Values.Look(ref blockCommsconsole, "blockCommsconsole", defaultValue: false);
    }
}