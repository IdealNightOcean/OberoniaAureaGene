using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestPart_EndGame_SnowstroemThreatsGenerator : QuestPartActivable, IIncidentMakerQuestPart
{
    public ThreatsGeneratorParams parms;

    public MapParent mapParent;

    public int threatStartTicks;

    private static readonly SimpleCurve ThreatScaleToCountFactorCurve =
    [
        new CurvePoint(0f, 0.1f),
        new CurvePoint(0.3f, 0.5f),
        new CurvePoint(0.6f, 0.8f),
        new CurvePoint(1f, 1f),
        new CurvePoint(1.55f, 1.1f),
        new CurvePoint(2.2f, 1.2f),
        new CurvePoint(10f, 2f)
    ];

    public IEnumerable<FiringIncident> MakeIntervalIncidents()
    {
        if (mapParent != null && mapParent.HasMap)
        {
            return MakeIntervalIncidents(parms, mapParent.Map, base.EnableTick + threatStartTicks);
        }

        return [];
    }
    public static IEnumerable<FiringIncident> MakeIntervalIncidents(ThreatsGeneratorParams parms, IIncidentTarget target, int startTick)
    {
        float threatScale = ThreatScaleToCountFactorCurve.Evaluate(Find.Storyteller.difficulty.threatScale);
        int incCount = IncidentCycleUtility.IncidentCountThisInterval(target, parms.randSeed, (float)GenDate.TickGameToSettled(startTick) / 60000f, parms.onDays, parms.offDays, parms.minSpacingDays, parms.numIncidentsRange.min * threatScale, parms.numIncidentsRange.max * threatScale);
        for (int i = 0; i < incCount; i++)
        {
            FiringIncident firingIncident = MakeThreat(parms, target);
            if (firingIncident != null)
            {
                yield return firingIncident;
            }
        }
    }
    private static FiringIncident MakeThreat(ThreatsGeneratorParams parms, IIncidentTarget target)
    {
        Faction faction = null;
        IncidentDef raidType;
        RaidStrategyDef raidStrategy;
        if (Rand.Chance(0.8f))
        {
            raidType = Snowstorm_IncidentDefOf.OAGene_SnowstormMaliceRaid_Reinforce;
            raidStrategy = Snowstorm_MiscDefOf.OAGene_SnowstormImmediateAttackBreaching;
        }
        else
        {
            raidType = IncidentDefOf.RaidEnemy;
            raidStrategy = Snowstorm_RimWorldDefOf.ImmediateAttackBreaching;
            faction = Faction.OfMechanoids;

        }
        IncidentParms incidentParms = new()
        {
            target = target,
            points = parms.threatPoints ?? (StorytellerUtility.DefaultThreatPointsNow(target) * parms.currentThreatPointsFactor),
            faction = faction,
            raidStrategy = raidStrategy,
            forced = true,
        };
        if (parms.minThreatPoints.HasValue)
        {
            incidentParms.points = Mathf.Max(incidentParms.points, parms.minThreatPoints.Value);
        }

        return new FiringIncident
        {
            def = raidType,
            parms = incidentParms
        };
    }

    public override void DoDebugWindowContents(Rect innerRect, ref float curY)
    {
        if (base.State == QuestPartState.Enabled)
        {
            Rect rect = new(innerRect.x, curY, 500f, 25f);
            if (Widgets.ButtonText(rect, "Log future incidents from " + GetType().Name))
            {
                StorytellerUtility.DebugLogTestFutureIncidents(currentMapOnly: false, null, this, 50);
            }

            curY += rect.height + 4f;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref parms, "parms");
        Scribe_References.Look(ref mapParent, "mapParent");
        Scribe_Values.Look(ref threatStartTicks, "threatStartTicks", 0);
    }
}