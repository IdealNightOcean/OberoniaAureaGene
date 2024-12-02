using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WorldObject_Hometown : MapParent
{
    public override void SpawnSetup()
    {
        base.SpawnSetup();
        Snowstorm_StoryUtility.StoryGameComp?.Notify_HometownSpawned(this.Tile);

    }
    public void Notify_CaravanArrived(Caravan caravan)
    {
        SetFaction(Faction.OfPlayer);
        Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(this.Tile, this.def);
        CaravanEnterMapUtility.Enter(caravan, orGenerateMap, CaravanEnterMode.Edge, CaravanDropInventoryMode.DoNotDrop, draftColonists: true);
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
    {
        foreach (FloatMenuOption option in base.GetFloatMenuOptions(caravan))
        {
            yield return option;
        }
        if (Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist))
        {
            foreach (Pawn p in caravan.PawnsListForReading)
            {
                if (p == protagonist)
                {
                    foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_VisitSnowstormHometown.GetFloatMenuOptions(caravan, this))
                    {
                        yield return floatMenuOption;
                    }
                    yield break;
                }
            }
        }
        yield return new FloatMenuOption("OAGene_Hometown_NoProtagonist".Translate(), null);
    }

    public override void Destroy()
    {
        int tile = this.Tile;
        base.Destroy();
        Snowstorm_StoryUtility.StoryGameComp.hometownSpawned = false;
        WorldObject hometown_sealed = WorldObjectMaker.MakeWorldObject(Snowstrom_MiscDefOf.OAGene_Hometown_Sealed);
        hometown_sealed.Tile = tile;
        Find.WorldObjects.Add(hometown_sealed);
    }
}

public class CaravanArrivalAction_VisitSnowstormHometown : CaravanArrivalAction
{
    private WorldObject_Hometown hometown;
    public override string Label => "OAGene_VisitHometown".Translate(hometown.Label);
    public override string ReportString => "CaravanVisiting".Translate(hometown.Label);
    public CaravanArrivalAction_VisitSnowstormHometown()
    { }

    public CaravanArrivalAction_VisitSnowstormHometown(WorldObject_Hometown hometown)
    {
        this.hometown = hometown;
    }
    public override void Arrived(Caravan caravan)
    {
        hometown?.Notify_CaravanArrived(caravan);
    }
    public override FloatMenuAcceptanceReport StillValid(Caravan caravan, int destinationTile)
    {
        FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(caravan, destinationTile);
        if (!floatMenuAcceptanceReport)
        {
            return floatMenuAcceptanceReport;
        }
        if (hometown == null || hometown.Tile != destinationTile)
        {
            return false;
        }
        return hometown.Spawned;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref hometown, "target");
    }

    public static FloatMenuAcceptanceReport CanVisit(WorldObject_Hometown hometown)
    {
        if (hometown == null || !hometown.Spawned)
        {
            return false;
        }
        return true;
    }

    public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, WorldObject_Hometown hometown)
    {
        return CaravanArrivalActionUtility.GetFloatMenuOptions(() => CanVisit(hometown), () => new CaravanArrivalAction_VisitSnowstormHometown(hometown), "OAGene_VisitHometown".Translate(hometown.Label), caravan, hometown.Tile, hometown);
    }
}
