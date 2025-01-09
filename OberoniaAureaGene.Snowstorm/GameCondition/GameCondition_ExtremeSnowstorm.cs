using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class GameCondition_ExtremeSnowstorm : GameCondition_ExtremeSnowstormBase
{
    [Unsaved]
    protected bool endSlience;

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
        endSlience = true;
        End();
    }
    protected override void PostInit()
    {
        try
        {
            Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormStart();

            int duration = Duration;
            SnowstormUtility.InitExtremeSnowstorm_World(duration);
            Map mainMap = MainMap;
            if (mainMap != null)
            {
                causeColdSnap = TryAddColdSnap(mainMap, duration);
                SnowstormUtility.InitExtremeSnowstorm_MainMap(mainMap, duration);
            }
            for (int i = 0; i < AffectedMaps.Count; i++)
            {
                Map map = AffectedMaps[i];
                SnowstormUtility.InitExtremeSnowstorm_AllMaps(map, duration);
            }
        }
        catch
        {
            Log.Error("Attempt to initialize extreme snowstorm failed.");
        }
    }
    protected override void PreEnd()
    {
        Snowstorm_MiscUtility.SnowstormGameComp.Notify_SnowstormEnd();
        SnowstormUtility.EndExtremeSnowstorm_World();
        Map mainMap = MainMap;
        if (endSlience)
        {
            mainMap?.SnowstormMapComp()?.Notify_SnowstormEnd();
        }
        else
        {
            SnowstormUtility.EndExtremeSnowstorm_MainMap(mainMap);
        }

        for (int i = 0; i < AffectedMaps.Count; i++)
        {
            Map map = AffectedMaps[i];
            SnowstormUtility.EndExtremeSnowstorm_AllMaps(map, endSlience);
        }
    }
    protected static bool TryAddColdSnap(Map mainMap, int duration)
    {
        if (Rand.Chance(0.3f))
        {
            if (OAGene_RimWorldDefOf.ColdSnap.Worker.CanFireNow(new IncidentParms { target = mainMap }))
            {
                GameConditionManager gameConditionManager = mainMap.GameConditionManager;
                GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDefOf.ColdSnap, duration);
                gameConditionManager.RegisterCondition(gameCondition);

                Letter letter = LetterMaker.MakeLetter("OAGene_LetterLabel_ExtremeSnowstormCauseColdSnap".Translate(), "OAGene_Letter_ExtremeSnowstormCauseColdSnap".Translate(), LetterDefOf.NegativeEvent);
                Find.LetterStack.ReceiveLetter(letter, playSound: false);
                Find.MusicManagerPlay.ForceTriggerTransition(OAGene_MiscDefOf.OAGene_Transition_ClairDeLune);
                return true;
            }
        }
        return false;
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