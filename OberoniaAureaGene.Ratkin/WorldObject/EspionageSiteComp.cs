using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Ratkin;
public class WorldObjectCompProperties_EspionageSite : WorldObjectCompProperties
{
    public WorldObjectCompProperties_EspionageSite()
    {
        compClass = typeof(EspionageSiteComp);
    }
}
public class EspionageSiteComp : WorldObjectComp
{
    protected static readonly List<Pair<Action, float>> tmpPossibleOutcomes = [];

    private bool active = false;
    protected Quest associateQuest;

    public void TryGetOutCome(Caravan caravan)
    {
        tmpPossibleOutcomes.Clear();
        if (parent is not Site site)
        {
            return;
        }

        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            Success(site);
        }, 15f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {

        }, 50f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            SuccessButBeFound(site, caravan);
        }, 25f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            FailAndBeBeFound(site, caravan);
        }, 10f));

    }
    public void ForceFail()
    { }

    protected static void Success(Site site)
    {
        site.Destroy();
    }
    protected static void SuccessButBeFound(Site site, Caravan caravan)
    {
        Faction faction = site.Faction;
        Faction.OfPlayer.TryAffectGoodwillWith(faction, -15);
        if (Faction.OfPlayer.HostileTo(faction))
        {
            new CaravanArrivalAction_VisitSite(site).Arrived(caravan);
        }
        else
        {
            site.Destroy();
        }
    }
    protected static void FailAndBeBeFound(Site site, Caravan caravan)
    {
        Faction faction = site.Faction;
        Faction.OfPlayer.TryAffectGoodwillWith(faction, -15);
        if (Faction.OfPlayer.HostileTo(faction))
        {
            new CaravanArrivalAction_VisitSite(site).Arrived(caravan);
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref active, "active", defaultValue: false);
        Scribe_References.Look(ref associateQuest, "associateQuest");
    }
}
