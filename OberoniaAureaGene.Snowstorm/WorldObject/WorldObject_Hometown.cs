using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WorldObject_Hometown : MapParent
{
    protected bool mapGenerated;
    public override void SpawnSetup()
    {
        base.SpawnSetup();
        Snowstorm_StoryUtility.StoryGameComp?.Notify_HometownSpawned(this, this.Tile);

    }
    public override void PostMapGenerate()
    {
        base.PostMapGenerate();
        mapGenerated = true;
        Snowstorm_StoryUtility.StoryGameComp.hometownMap = this.Map;
    }

    public override void Notify_MyMapRemoved(Map map)
    {
        base.Notify_MyMapRemoved(map);
        mapGenerated = false;
        Snowstorm_StoryUtility.StoryGameComp.hometownMap = null;
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
    {
        foreach (FloatMenuOption option in base.GetFloatMenuOptions(caravan))
        {
            yield return option;
        }
        if (mapGenerated)
        {
            yield break;
        }

        bool hasProtagonist = false;
        if (Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist))
        {
            foreach (Pawn p in caravan.PawnsListForReading)
            {
                if (p == protagonist)
                {
                    hasProtagonist = true;
                    break;
                }
            }
        }

        if (hasProtagonist)
        {
            foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_VisitSnowstormHometown.GetFloatMenuOptions(caravan, this))
            {
                yield return floatMenuOption;
            }
        }
        else
        {
            yield return new FloatMenuOption("OAGene_FloatMenuOption_NoProtagonist".Translate(), null);
        }
    }

    public override IEnumerable<IncidentTargetTagDef> IncidentTargetTags()
    {
        foreach (IncidentTargetTagDef item in base.IncidentTargetTags())
        {
            yield return item;
        }
        if (base.Faction == Faction.OfPlayer)
        {
            yield return IncidentTargetTagDefOf.Map_PlayerHome;
        }
        else
        {
            yield return IncidentTargetTagDefOf.Map_Misc;
        }
    }
    public override void Destroy()
    {
        int tile = this.Tile;
        base.Destroy();
        Snowstorm_StoryUtility.StoryGameComp?.Notify_HometownDestory();
        WorldObject hometown_sealed = WorldObjectMaker.MakeWorldObject(Snowstorm_MiscDefOf.OAGene_Hometown_Sealed);
        hometown_sealed.Tile = tile;
        Find.WorldObjects.Add(hometown_sealed);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref mapGenerated, "mapGenerated", defaultValue: false);
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
        if (hometown == null)
        {
            return;
        }
        bool hasProtagonist = false;
        if (Snowstorm_StoryUtility.TryGetStoryProtagonist(out Pawn protagonist))
        {
            foreach (Pawn p in caravan.PawnsListForReading)
            {
                if (p == protagonist)
                {
                    hasProtagonist = true;
                    break;
                }
            }
        }

        if (hasProtagonist)
        {
            hometown.SetFaction(Faction.OfPlayer);
            Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(hometown.Tile, hometown.def);
            CaravanEnterMapUtility.Enter(caravan, orGenerateMap, CaravanEnterMode.Edge, CaravanDropInventoryMode.DoNotDrop, draftColonists: true);
        }
        else
        {
            Messages.Message("OAGene_Message_NoProtagonist".Translate(), hometown, MessageTypeDefOf.RejectInput, historical: false);
        }
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
