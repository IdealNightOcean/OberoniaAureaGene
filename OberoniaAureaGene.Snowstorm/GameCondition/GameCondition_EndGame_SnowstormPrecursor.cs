using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class GameCondition_EndGame_SnowstormPrecursor : GameCondition_SnowstormPrecursor
{
    protected override void PostEnd()
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
                OAFrame_MiscUtility.TryFireIncidentNow(Snowstorm_IncidentDefOf.OAGene_EndGame_ExtremeSnowstorm, parms);
            }
            catch
            {
                Log.Error("Attempt to trigger end-game extreme snowstorm failed.");
            }
        }
    }

    protected override Map GetMainMap()
    {
        Map mainMap = gameConditionManager.ownerMap;

        mainMap ??= Snowstorm_StoryUtility.GetHometownMap();

        return mainMap;
    }

}
