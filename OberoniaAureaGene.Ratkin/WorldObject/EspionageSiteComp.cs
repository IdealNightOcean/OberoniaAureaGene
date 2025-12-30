using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
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
    public bool activeEspionage = false;
    private int allowEspionageTick = -1;
    public bool espionageSuccess;

    public int CoolingTicksLeft => allowEspionageTick - Find.TickManager.TicksGame;
    public bool AllowEspionage => activeEspionage && !IsWorking && Find.TickManager.TicksGame > allowEspionageTick;
    public Site Site => parent as Site;

    private EspionageHandler espionageHandler;
    public bool IsWorking => espionageHandler?.IsWorking ?? false;
    public EspionageHandler EspionageHandler => espionageHandler;

    public void InitEspionage(int allowEspionageTick = -1)
    {
        activeEspionage = true;
        this.allowEspionageTick = allowEspionageTick;
    }

    public void Disable()
    {
        activeEspionage = false;
        allowEspionageTick = -1;
        espionageHandler?.CancelWork();
        espionageHandler = null;
    }

    public override void CompTick()
    {
        base.CompTick();
        espionageHandler?.WorkTick();
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

    public void Notify_EspionageEnd(bool destory, bool succeeded, int cooldownTicks = -1)
    {
        espionageSuccess = succeeded;
        espionageHandler = null;
        if (destory)
        {
            Disable();
            if (!parent.Destroyed && !Site.HasMap)
            {
                parent.Destroy();
            }
        }
        else
        {
            allowEspionageTick = Find.TickManager.TicksGame + cooldownTicks;
        }
    }

    public void StartEspionage(Caravan caravan)
    {
        espionageHandler = new EspionageHandler(Site);
        espionageHandler.StartWork(caravan);
    }
    public override void PostDestroy()
    {
        if (activeEspionage && !espionageSuccess)
        {
            Site site = Site;
            bool allEnemiesDefeated = OAFrame_ReflectionUtility.GetFieldValue(site, "allEnemiesDefeatedSignalSent", fallback: false);
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
        Scribe_Deep.Look(ref espionageHandler, "espionageHandler", ctorArgs: Site);
    }
}

public class CaravanArrivalAction_EspionageSiteComp : CaravanArrivalAction
{
    private WorldObject site;
    public override string Label => "OAGene_EspionageSite".Translate(site.Label);
    public override string ReportString => "CaravanVisiting".Translate(site.Label);

    public CaravanArrivalAction_EspionageSiteComp() { }
    public CaravanArrivalAction_EspionageSiteComp(WorldObject site)
    {
        this.site = site;
    }

    public override void Arrived(Caravan caravan)
    {
        EspionageSiteComp espionageSiteComp = site.GetComponent<EspionageSiteComp>();
        if (espionageSiteComp.AllowEspionage)
        {
            espionageSiteComp.StartEspionage(caravan);
        }
    }

    public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
    {
        if (site is null || site.Tile != destinationTile)
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
        if (site is null || !site.Spawned)
        {
            return false;
        }
        EspionageSiteComp espionageSiteComp = site.GetComponent<EspionageSiteComp>();
        if (espionageSiteComp is null || !espionageSiteComp.activeEspionage || espionageSiteComp.IsWorking)
        {
            return false;
        }
        int coolingTicksLeft = espionageSiteComp.CoolingTicksLeft;
        if (coolingTicksLeft > 0)
        {
            return FloatMenuAcceptanceReport.WithFailMessage("OAGene_MessageEspionageCooldown".Translate(coolingTicksLeft.ToStringTicksToPeriod()));
        }
        return true;
    }

    public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, WorldObject site)
    {
        return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CanVisit(site), () => new CaravanArrivalAction_EspionageSiteComp(site), "OAGene_EspionageSite".Translate(site.Label), caravan, site.Tile, site);
    }
}
