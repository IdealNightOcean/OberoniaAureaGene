using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WorldObjectCompProperties_SnowstormCamp : WorldObjectCompProperties
{
    public WorldObjectCompProperties_SnowstormCamp()
    {
        compClass = typeof(SnowstormCampComp);
    }
}

public class SnowstormCampComp : WorldObjectComp
{
    private bool active = false;
    protected SiteTrader innerTrader;

    protected const string Postfix = "_SnowstormCamp";

    protected enum SnowstormCampType
    {
        None,
        Firendly,
        Hostile
    }

    protected SnowstormCampType curType = SnowstormCampType.None;
    public void InitInnerTrader()
    {
        innerTrader = new(Snowstrom_MiscDefOf.OAGene_Trader_SnowstormCamp, parent);
        innerTrader.GenerateThings(parent.Tile);
    }
    public void VisitCamp(Caravan caravan)
    {
        if (curType == SnowstormCampType.None)
        {
            TryInitType(caravan);
        }
        else if (curType == SnowstormCampType.Firendly)
        {
            DiaNode diaNode = OAFrame_DiaUtility.ConfirmDiaNode("OAGene_SnowstormCamp_Firendly".Translate(), "OAGene_SnowstormCamp_Trade".Translate(), delegate
            {
                TradeWithCamp(caravan);
            }, "GoBack".Translate(), null);
            Dialog_NodeTree nodeTree = new(diaNode);
            Find.WindowStack.Add(nodeTree);
        }
        else if (curType == SnowstormCampType.Hostile)
        {
            DiaNode diaNode = OAFrame_DiaUtility.ConfirmDiaNode("OAGene_SnowstormCamp_Hostile".Translate(), "Attack".Translate(), delegate
            {
                new CaravanArrivalAction_VisitSite(parent as RimWorld.Planet.Site).Arrived(caravan);
            }, "GoBack".Translate(), null);
            Dialog_NodeTree nodeTree = new(diaNode);
            Find.WindowStack.Add(nodeTree);
        }
    }

    protected void TryInitType(Caravan caravan)
    {
        TaggedString text;
        DiaNode diaNode;

        float randFlag = Rand.Value;
        if (randFlag < 0.05f)
        {
            curType = SnowstormCampType.Firendly;
            InitInnerTrader();
            text = "OAGene_SnowstormCamp_FirendlyAndGift".Translate();
            diaNode = OAFrame_DiaUtility.ConfirmDiaNode(text, "OAGene_SnowstormCamp_Trade".Translate(), delegate
            {
                GiveGifts(caravan);
                TradeWithCamp(caravan);
            }, "GoBack".Translate(), null);
        }
        else if (randFlag < 0.4f)
        {
            curType = SnowstormCampType.Firendly;
            InitInnerTrader();
            text = "OAGene_SnowstormCamp_FirendlyFirst".Translate();
            diaNode = OAFrame_DiaUtility.ConfirmDiaNode(text, "OAGene_SnowstormCamp_Trade".Translate(), delegate
            {
                TradeWithCamp(caravan);
            }, "GoBack".Translate(), null);

        }
        else
        {
            curType = SnowstormCampType.Hostile;
            innerTrader = null;
            text = "OAGene_SnowstormCamp_HostileFirst".Translate();
            diaNode = OAFrame_DiaUtility.ConfirmDiaNode(text, "Attack".Translate(), delegate
            {
                new CaravanArrivalAction_VisitSite(parent as RimWorld.Planet.Site).Arrived(caravan);
            }, "GoBack".Translate(), null);
        }
        Dialog_NodeTree nodeTree = new(diaNode);
        Find.WindowStack.Add(nodeTree);
    }
    protected static void GiveGifts(Caravan caravan)
    {
        Thing torch = ThingMaker.MakeThing(Snowstrom_MiscDefOf.OAGene_AntiSnowTorch);
        Thing pemmican = ThingMaker.MakeThing(ThingDefOf.Pemmican);
        pemmican.stackCount = Rand.RangeInclusive(35, 45);

        CaravanInventoryUtility.GiveThing(caravan, torch);
        CaravanInventoryUtility.GiveThing(caravan, pemmican);
    }
    protected void TradeWithCamp(Caravan caravan)
    {
        if (innerTrader == null)
        {
            return;
        }
        Pawn pawn = BestCaravanPawnUtility.FindBestNegotiator(caravan);
        if (pawn == null)
        {
            Messages.Message("OAFrame_MessageNoTrader".Translate(), caravan, MessageTypeDefOf.NegativeEvent, historical: false);
            return;
        }
        Find.WindowStack.Add(new Dialog_Trade(pawn, innerTrader));
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
    {
        if (active)
        {
            foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_VisitSnowstormCamp.GetFloatMenuOptions(caravan, parent))
            {
                yield return floatMenuOption;
            }
        }
    }
    public void ActiveComp()
    {
        active = true;
        curType = SnowstormCampType.None;
    }

    public void InactiveComp()
    {
        active = false;
        curType = SnowstormCampType.None;
        innerTrader?.Destory();
        innerTrader = null;
    }

    public override void PostDestroy()
    {
        innerTrader?.Destory();
        base.PostDestroy();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref active, "active" + Postfix, defaultValue: false);
        Scribe_Values.Look(ref curType, "curType" + Postfix, defaultValue: SnowstormCampType.None);
        Scribe_Deep.Look(ref innerTrader, "innerTrader" + Postfix);
    }
}

public class CaravanArrivalAction_VisitSnowstormCamp : CaravanArrivalAction
{
    private WorldObject site;
    public override string Label => "OAFrame_Visit".Translate(site.Label);
    public override string ReportString => "CaravanVisiting".Translate(site.Label);
    public CaravanArrivalAction_VisitSnowstormCamp()
    { }

    public CaravanArrivalAction_VisitSnowstormCamp(WorldObject site)
    {
        this.site = site;
    }
    public override void Arrived(Caravan caravan)
    {
        site.GetComponent<SnowstormCampComp>()?.VisitCamp(caravan);
    }
    public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
    {
        FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(caravan, destinationTile);
        if (!floatMenuAcceptanceReport)
        {
            return floatMenuAcceptanceReport;
        }
        if (site == null || site.Tile != destinationTile)
        {
            return false;
        }
        return site.Spawned;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref site, "target");
    }

    public static FloatMenuAcceptanceReport CanVisit(WorldObject site)
    {
        if (site == null || !site.Spawned)
        {
            return false;
        }
        return true;
    }

    public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, WorldObject site)
    {
        return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CanVisit(site), () => new CaravanArrivalAction_VisitSnowstormCamp(site), "OAFrame_Visit".Translate(site.Label), caravan, site.Tile, site);
    }
}