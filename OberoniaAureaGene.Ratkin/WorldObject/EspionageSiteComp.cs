using OberoniaAurea_Frame;
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

[StaticConstructorOnStartup]
public class EspionageSiteComp : WorldObjectComp
{
    protected static readonly List<Pair<Action, float>> tmpPossibleOutcomes = [];

    private bool activeEspionage = false;
    protected int allowEspionageTick = -1;
    public int CoolingTicksLeft => allowEspionageTick - Find.TickManager.TicksGame;
    public bool AllowEspionage => Find.TickManager.TicksGame > allowEspionageTick;
    protected Quest espionageQuest;

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
        tmpPossibleOutcomes.Add(new Pair<Action, float>(Fail, 50f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            SuccessButBeFound(site, caravan);
        }, 25f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            FailAndBeBeFound(site, caravan);
            Fail();
        }, 10f));
        tmpPossibleOutcomes.RandomElementByWeight((Pair<Action, float> x) => x.Second).First();
    }

    protected static void Success(Site site)
    {
        site.Destroy();
    }
    public void Fail()
    {
        allowEspionageTick = Find.TickManager.TicksGame + 60000;
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
    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
    {
        if (!activeEspionage)
        {
            yield break;
        }
        foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_EspionageSiteComp.GetFloatMenuOptions(caravan, parent))
        {
            yield return floatMenuOption;
        }
    }

    public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
    {
        if (!activeEspionage || parent is not Site site)
        {
            yield break;
        }
        Command_Action command_Action = new()
        {
            defaultLabel = "OAGene_CommandStartEspionage".Translate(),
            defaultDesc = "OAGene_CommandStartEspionageDesc".Translate(),
            icon = null,
            action = delegate
            {
                FixCaravan_EspionageSite fixCaravan = (FixCaravan_EspionageSite)FixedCaravanUtility.CreateFixedCaravan(caravan, OAGene_RatkinDefOf.OAGene_FixedCaravan_Espionage, FixCaravan_EspionageSite.ReconnaissanceTicks);
                fixCaravan.SetEspionageSiteComp(this);
            }
        };
        if (!AllowEspionage)
        {
            command_Action.Disable("OAGene_MessageEspionageCooldown".Translate(CoolingTicksLeft.ToStringTicksToPeriod()));
        }
        yield return command_Action;
    }


    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref activeEspionage, "activeEspionage", defaultValue: false);
        Scribe_Values.Look(ref allowEspionageTick, "allowEspionageTick", -1);
        Scribe_References.Look(ref espionageQuest, "espionageQuest");
    }
}

public class CaravanArrivalAction_EspionageSiteComp : CaravanArrivalAction
{

    private WorldObject site;
    public override string Label => "OAGene_EspionageSite".Translate(site.Label);
    public override string ReportString => "CaravanVisiting".Translate(site.Label);
    public CaravanArrivalAction_EspionageSiteComp()
    { }

    public CaravanArrivalAction_EspionageSiteComp(WorldObject site)
    {
        this.site = site;
    }
    public override void Arrived(Caravan caravan)
    {
        Espionage(caravan, site);
    }
    private static void Espionage(Caravan caravan, WorldObject site)
    {
        FixCaravan_EspionageSite fixCaravan = (FixCaravan_EspionageSite)FixedCaravanUtility.CreateFixedCaravan(caravan, OAGene_RatkinDefOf.OAGene_FixedCaravan_Espionage, FixCaravan_EspionageSite.ReconnaissanceTicks);
        fixCaravan.SetEspionageSiteComp(site.GetComponent<EspionageSiteComp>());
    }
    public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
    {
        FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(caravan, destinationTile);
        if (!floatMenuAcceptanceReport)
        {
            return floatMenuAcceptanceReport;
        }
        if (site != null && site.Tile != destinationTile)
        {
            return false;
        }
        return CanVisit(site);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref site, "site");
    }

    public static FloatMenuAcceptanceReport CanVisit(WorldObject site)
    {
        if (site == null || !site.Spawned)
        {
            return false;
        }
        EspionageSiteComp espionageSiteComp = site.GetComponent<EspionageSiteComp>();
        if (espionageSiteComp != null && !espionageSiteComp.AllowEspionage)
        {
            return FloatMenuAcceptanceReport.WithFailMessage("OAGene_MessageEspionageCooldown".Translate(espionageSiteComp.CoolingTicksLeft.ToStringTicksToPeriod()));
        }
        return true;
    }

    public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, WorldObject site)
    {
        return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CanVisit(site), () => new CaravanArrivalAction_EspionageSiteComp(site), "OAGene_EspionageSite".Translate(site.Label), caravan, site.Tile, site);
    }
}
