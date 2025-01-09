using OberoniaAurea_Frame;
using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_SnowstormPrecursor : GameCondition_TemperatureChange
{
    public override void Init()
    {
        base.Init();
        PostInit();
    }

    protected virtual void PostInit()
    {
        Map mainMap = GetMainMap();
        if (mainMap != null)
        {
            IncidentParms parms = new()
            {
                target = mainMap,
            };
            try
            {
                OAFrame_MiscUtility.TryFireIncidentNow(Snowstorm_IncidentDefOf.OAGene_SnowstormPrecursor_AnimalFlee, parms);
            }
            catch
            {
                Log.Error("Attempt to initialize snowstorm precursor failed.");
            }
        }
    }
    public void EndSlience()
    {
        suppressEndMessage = true;
        base.End();
    }

    public override void End()
    {
        base.End();
        PostEnd();
    }
    protected virtual void PostEnd()
    {
        IncidentParms parms = new()
        {
            target = Find.World,
        };
        try
        {
            OAFrame_MiscUtility.TryFireIncidentNow(Snowstorm_IncidentDefOf.OAGene_ExtremeSnowstorm, parms);
        }
        catch
        {
            Log.Error("Attempt to trigger extreme snowstorm failed.");
        }

    }

    protected virtual Map GetMainMap()
    {
        Map mainMap = gameConditionManager.ownerMap;

        mainMap ??= AffectedMaps.Where(m => m.IsPlayerHome).RandomElementWithFallback(null);

        mainMap ??= Find.AnyPlayerHomeMap;

        return mainMap;
    }

}
