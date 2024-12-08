using OberoniaAurea_Frame;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
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
        if (parms.faction == null)
        {
            Faction tempFaction = null;
            if (Snowstorm_StoryUtility.StoryGameComp.satisfySnowstormCultist)
            {
                tempFaction ??= OAFrame_FactionUtility.ValidTempFactionsOfDef(FactionDefOf.OutlanderCivil).Where(f => !f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback(null);
                tempFaction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil, FactionRelationKind.Ally);
                tempFaction ??= Find.FactionManager.RandomNonHostileFaction(allowNonHumanlike: false);
            }
            else
            {
                tempFaction ??= OAFrame_FactionUtility.ValidTempFactionsOfDef(FactionDefOf.OutlanderCivil).Where(f => f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback(null);
                tempFaction ??= OAFrame_FactionUtility.GenerateTempFaction(FactionDefOf.OutlanderCivil, FactionRelationKind.Hostile);
                tempFaction ??= Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
            }
            if (tempFaction == null)
            {
                return false;
            }
            AdjuestFactionRelation(tempFaction);
            parms.faction = tempFaction;
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
        raidParms1.faction = parms.faction;

        IncidentParms raidParms2 = StorytellerUtility.DefaultParmsNow(raidCategory, parms.target);
        raidParms2.faction = parms.faction;

        bool flag1 = OAFrame_MiscUtility.TryFireIncidentNow(Snowstorm_IncidentDefOf.OAGene_SnowstormCultistRaid, raidParms1);
        bool flag2 = OAFrame_MiscUtility.TryFireIncidentNow(Snowstorm_IncidentDefOf.OAGene_SnowstormCultistRaid, raidParms2);

        return flag1 || flag2;
    }

    protected static void AdjuestFactionRelation(Faction faction)
    {
        Faction ofPlayer = Faction.OfPlayer;
        List<FactionRelation> relations = [];
        foreach (Faction otherF in Find.FactionManager.AllFactionsListForReading)
        {
            if (otherF == faction || otherF == ofPlayer)
            {
                continue;
            }
            if (!otherF.def.PermanentlyHostileTo(faction.def))
            {
                relations.Add(new FactionRelation
                {
                    other = otherF,
                    kind = FactionRelationKind.Hostile
                });
            }
        }
        for (int i = 0; i < relations.Count; i++)
        {
            faction.SetRelation(relations[i]);
        }
    }
}
