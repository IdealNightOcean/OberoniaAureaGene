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

    public bool activeEspionage = false;
    protected int allowEspionageTick = -1;
    public bool espionageSuccess;

    public int CoolingTicksLeft => allowEspionageTick - Find.TickManager.TicksGame;
    public bool AllowEspionage => activeEspionage && Find.TickManager.TicksGame > allowEspionageTick;
    public Site Site => parent as Site;

    public void TryGetOutCome(Caravan caravan, bool forceFail = false)
    {
        if (forceFail)
        {
            Fail(this);
            return;
        }
        tmpPossibleOutcomes.Clear();
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            Success(this);
        }, 15f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            Fail(this);
        }, 50f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            SuccessButBeFound(caravan, this);
        }, 25f));
        tmpPossibleOutcomes.Add(new Pair<Action, float>(delegate
        {
            FailAndBeBeFound(caravan, this);
        }, 10f));
        tmpPossibleOutcomes.RandomElementByWeight((Pair<Action, float> x) => x.Second).First();
    }
    public void InitEspionage(int allowEspionageTick = -1)
    {
        activeEspionage = true;
        this.allowEspionageTick = allowEspionageTick;
    }
    public void Disable()
    {
        activeEspionage = false;
        allowEspionageTick = -1;
    }

    protected static void Success(EspionageSiteComp espionageSiteComp)
    {
        Site site = espionageSiteComp.Site;

        espionageSiteComp.espionageSuccess = true;
        QuestUtility.SendQuestTargetSignals(site.questTags, "OAGene_EspionageSuccess", site.Named("SUBJECT"));
        Messages.Message("OAGene_MessageEspionageSuccess".Translate(), MessageTypeDefOf.PositiveEvent);
        site.Destroy();
    }
    public static void Fail(EspionageSiteComp espionageSiteComp)
    {
        Messages.Message("OAGene_MessageEspionageFail".Translate(), MessageTypeDefOf.NeutralEvent);
        espionageSiteComp.allowEspionageTick = Find.TickManager.TicksGame + 15000;
    }

    protected static void SuccessButBeFound(Caravan caravan, EspionageSiteComp espionageSiteComp)
    {
        Site site = espionageSiteComp.Site;

        espionageSiteComp.espionageSuccess = true;
        Faction.OfPlayer.TryAffectGoodwillWith(site.Faction, -15, reason: OAGene_RatkinDefOf.OAGene_SuspectedBehavior);
        QuestUtility.SendQuestTargetSignals(site.questTags, "OAGene_EspionageSuccess", site.Named("SUBJECT"));
        if (site.Faction.HostileTo(Faction.OfPlayer))
        {
            Messages.Message("OAGene_MessageEspionageSuccessButBeFound".Translate(), MessageTypeDefOf.ThreatBig);
            new CaravanArrivalAction_VisitSite(site).Arrived(caravan);
        }
        else
        {
            Messages.Message("OAGene_MessageEspionageSuccessButBeFound".Translate(), MessageTypeDefOf.NegativeEvent);
            site.Destroy();
        }
    }
    protected static void FailAndBeBeFound(Caravan caravan, EspionageSiteComp espionageSiteComp)
    {
        Site site = espionageSiteComp.Site;

        espionageSiteComp.allowEspionageTick = Find.TickManager.TicksGame + 15000;
        Faction.OfPlayer.TryAffectGoodwillWith(site.Faction, -15, reason: OAGene_RatkinDefOf.OAGene_SuspectedBehavior);
        if (site.Faction.HostileTo(Faction.OfPlayer))
        {
            Messages.Message("OAGene_MessageEspionageFailAndBeBeFound".Translate(), MessageTypeDefOf.ThreatBig);
            new CaravanArrivalAction_VisitSite(site).Arrived(caravan);
        }
        else
        {
            Messages.Message("OAGene_MessageEspionageFailAndBeBeFound".Translate(), MessageTypeDefOf.NegativeEvent);
        }
    }
    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
    {
        if (activeEspionage)
        {
            foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_EspionageSiteComp.GetFloatMenuOptions(caravan, parent))
            {
                yield return floatMenuOption;
            }
        }
    }

    public void Espionage(Caravan caravan)
    {
        if (!OAFrame_CaravanUtility.IsExactTypeCaravan(caravan))
        {
            return;
        }
        FixCaravan_EspionageSite fixedCaravan = (FixCaravan_EspionageSite)OAFrame_FixedCaravanUtility.CreateFixedCaravan(caravan, OAGene_RatkinDefOf.OAGene_FixedCaravan_Espionage, FixCaravan_EspionageSite.ReconnaissanceTicks);
        fixedCaravan.SetEspionageSiteComp(Site);
        Find.WorldObjects.Add(fixedCaravan);
    }
    public override void PostDestroy()
    {
        if (activeEspionage && !espionageSuccess)
        {
            Site site = parent as Site;
            bool allEnemiesDefeated = OAFrame_ReflectionUtility.GetFieldValue<bool>(site, "allEnemiesDefeatedSignalSent", fallback: false);
            if (allEnemiesDefeated)
            {
                QuestUtility.SendQuestTargetSignals(site.questTags, "OAGene_EspionageSuccess", site.Named("SUBJECT"));
            }
            else
            {
                QuestUtility.SendQuestTargetSignals(site.questTags, "OAGene_EspionageFail", site.Named("SUBJECT"));
            }
        }
        Disable();
        base.PostDestroy();
    }
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref activeEspionage, "activeEspionage", defaultValue: false);
        Scribe_Values.Look(ref allowEspionageTick, "allowEspionageTick", -1);
        Scribe_Values.Look(ref espionageSuccess, "espionageSuccess", defaultValue: false);
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
        site.GetComponent<EspionageSiteComp>()?.Espionage(caravan);
    }
    public override FloatMenuAcceptanceReport StillValid(Caravan caravan, PlanetTile destinationTile)
    {
        if (site == null)
        {
            return false;
        }
        if (site.Tile != destinationTile)
        {
            return false;
        }
        return CanVisit(site);
    }

    public override void ExposeData()
    {
        Scribe_References.Look(ref site, "site");
    }

    public static FloatMenuAcceptanceReport CanVisit(WorldObject site)
    {
        if (site == null || !site.Spawned)
        {
            return false;
        }
        EspionageSiteComp espionageSiteComp = site.GetComponent<EspionageSiteComp>();
        if (espionageSiteComp == null)
        {
            return false;
        }
        if (!espionageSiteComp.AllowEspionage)
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
