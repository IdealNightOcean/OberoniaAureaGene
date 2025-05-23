﻿using OberoniaAurea_Frame;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class IncidentWorker_SnowstormFanaticalCultis : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        Map map = (Map)parms.target;
        map ??= Snowstorm_StoryUtility.GetHometownMap();
        return map != null;
    }

    protected bool ResolveParms(IncidentParms parms)
    {
        if (!Snowstorm_StoryUtility.StoryGameComp.storyInProgress)
        {
            return false;
        }
        Map map = (Map)parms.target;
        map ??= Snowstorm_StoryUtility.GetHometownMap();
        if (map == null)
        {
            return false;
        }
        return true;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!ResolveParms(parms))
        {
            return false;
        }
        IncidentCategoryDef raidCategory = Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist ? Snowstorm_RimWorldDefOf.AllyAssistance : IncidentCategoryDefOf.ThreatBig;


        IncidentParms raidParms1 = StorytellerUtility.DefaultParmsNow(raidCategory, parms.target);
        raidParms1.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
        raidParms1.forced = true;

        IncidentParms raidParms2 = StorytellerUtility.DefaultParmsNow(raidCategory, parms.target);
        raidParms1.forced = true;

        bool flag1 = OAFrame_MiscUtility.TryFireIncidentNow(Snowstorm_IncidentDefOf.OAGene_SnowstormCultistRaid, raidParms1);
        bool flag2 = OAFrame_MiscUtility.TryFireIncidentNow(Snowstorm_IncidentDefOf.OAGene_SnowstormCultistRaid, raidParms2);

        return flag1 || flag2;
    }

}
