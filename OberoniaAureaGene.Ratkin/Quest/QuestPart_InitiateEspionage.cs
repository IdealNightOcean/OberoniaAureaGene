using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Ratkin;

//初始化给予玩家型交易comp
public class QuestPart_InitiateEspionage : QuestPart
{
    public string inSignal;

    public Site site;
    public string customLabel;


    public override IEnumerable<GlobalTargetInfo> QuestLookTargets
    {
        get
        {
            foreach (GlobalTargetInfo questLookTarget in base.QuestLookTargets)
            {
                yield return questLookTarget;
            }
            if (site is not null)
            {
                yield return site;
            }
        }
    }

    public override IEnumerable<Faction> InvolvedFactions
    {
        get
        {
            foreach (Faction involvedFaction in base.InvolvedFactions)
            {
                yield return involvedFaction;
            }
            if (site.Faction is not null)
            {
                yield return site.Faction;
            }
        }
    }

    public override void Notify_QuestSignalReceived(Signal signal)
    {
        base.Notify_QuestSignalReceived(signal);
        if (signal.tag == inSignal)
        {
            EspionageSiteComp component = site.GetComponent<EspionageSiteComp>();
            if (component is not null)
            {
                if (component.activeEspionage)
                {
                    Log.Error("Site " + site.Label + " already has an active espionage.");
                    return;
                }
                site.customLabel = customLabel;
                component.InitEspionage();
            }
        }
    }

    public override void Cleanup()
    {
        base.Cleanup();
        EspionageSiteComp component = site.GetComponent<EspionageSiteComp>();
        component?.Disable();
        site = null;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref inSignal, "inSignal");
        Scribe_Values.Look(ref customLabel, "customLabel");
        Scribe_References.Look(ref site, "site");
    }
}
