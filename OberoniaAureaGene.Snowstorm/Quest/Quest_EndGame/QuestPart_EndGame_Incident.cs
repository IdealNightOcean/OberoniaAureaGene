using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestPart_EndGame_Incident : QuestPart
{
    public string inSignal;

    public IncidentDef incident;
    protected IncidentParms incidentParms;
    public float currentThreatPointsFactor = 1f;
    protected MapParent mapParent;


    public override IEnumerable<GlobalTargetInfo> QuestLookTargets
    {
        get
        {
            foreach (GlobalTargetInfo questLookTarget in base.QuestLookTargets)
            {
                yield return questLookTarget;
            }
            if (mapParent != null)
            {
                yield return mapParent;
            }
        }
    }

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        if (signal.tag != inSignal || incidentParms == null)
        {
            return;
        }
        incidentParms.forced = true;
        incidentParms.quest = quest;
        Log.Message("mapParent");
        Log.Message(mapParent != null);
        Log.Message("map");
        Log.Message(mapParent.HasMap);
        if (mapParent != null && mapParent.HasMap)
        {
            Log.Message("yesMapP");
            Map targetMap = mapParent.Map;
            incidentParms.target = targetMap;
            if (incidentParms.points < 0f)
            {
                incidentParms.points = StorytellerUtility.DefaultThreatPointsNow(targetMap) * currentThreatPointsFactor;
            }
            if (incident.Worker.CanFireNow(incidentParms))
            {
                Log.Message("f");
                incident.Worker.TryExecute(incidentParms);
            }
            Log.Message("nf");
            incidentParms.target = null;
        }
    }

    public void SetIncidentParmsAndRemoveTarget(IncidentParms parms, MapParent mapParent = null)
    {
        incidentParms = parms;
        if (incidentParms.target is Map map && map.Parent != null)
        {
            this.mapParent = map.Parent;
            incidentParms.target = null;
        }
        else
        {
            this.mapParent = mapParent;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
        Scribe_Defs.Look(ref incident, "incident");
        Scribe_Deep.Look(ref incidentParms, "incidentParms");
        Scribe_References.Look(ref mapParent, "mapParent");
        Scribe_Values.Look(ref currentThreatPointsFactor, "currentThreatPointsFactor", 1f);
    }

    public override void AssignDebugData()
    {
        base.AssignDebugData();
        inSignal = "DebugSignal" + Rand.Int;
        if (Find.AnyPlayerHomeMap != null)
        {
            incident = IncidentDefOf.RaidEnemy;
            IncidentParms incidentParms = new()
            {
                target = Find.RandomPlayerHomeMap,
                points = 500f
            };
            SetIncidentParmsAndRemoveTarget(incidentParms);
        }
    }
}

