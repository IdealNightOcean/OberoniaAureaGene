using OberoniaAurea_Frame;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WorldObject_Hometown : MapParent
{
    public override void SpawnSetup()
    {
        base.SpawnSetup();
        Snowstorm_StoryUtility.StoryGameComp?.Notify_HometownSpawned(this.Tile);

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
