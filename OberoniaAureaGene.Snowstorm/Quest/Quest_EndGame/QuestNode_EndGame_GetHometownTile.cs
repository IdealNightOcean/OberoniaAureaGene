using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_GetHometownTile : QuestNode
{
    private const int minDist = 30;
    private const int maxDist = 200;

    [NoTranslate]
    public SlateRef<string> storeAs;
    protected override bool TestRunInt(Slate slate)
    {
        if (GetHometownTile(out PlanetTile hometownTile))
        {
            slate.Set(storeAs.GetValue(slate), hometownTile);
            return true;
        }
        else
        {
            return false;
        }
    }
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        if (GetHometownTile(out PlanetTile hometownTile))
        {
            slate.Set(storeAs.GetValue(slate), hometownTile);
        }
    }

    protected bool GetHometownTile(out PlanetTile hometownTile)
    {
        hometownTile = PlanetTile.Invalid;
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp == null)
        {
            return false;
        }
        hometownTile = storyGameComp.hometownTile;
        if (!hometownTile.Valid)
        {
            WorldObject hometown_Sealed = Find.WorldObjects.AllWorldObjects.Where(w => w.def == Snowstorm_MiscDefOf.OAGene_Hometown_Sealed).FirstOrFallback(null);
            if (hometown_Sealed != null)
            {
                hometownTile = hometown_Sealed.Tile;
            }
            else
            {
                return GetNewHometownTile(out hometownTile);
            }
        }
        return true;
    }

    protected static bool GetNewHometownTile(out PlanetTile tile)
    {
        tile = PlanetTile.Invalid;

        if (!TryFindRootTile(out PlanetTile rootTile))
        {
            return false;
        }

        return TryFindDestinationTileActual(rootTile, out tile);
    }

    private static bool TryFindRootTile(out PlanetTile rootTile)
    {
        return TileFinder.TryFindRandomPlayerTile(out rootTile, allowCaravans: false, (PlanetTile r) => TryFindDestinationTileActual(r, out PlanetTile tempTile));
    }
    private static bool TryFindDestinationTileActual(PlanetTile rootTile, out PlanetTile tile)
    {

        for (int i = 0; i < 2; i++)
        {
            bool canTraverseImpassable = i == 1;
            if (TileFinder.TryFindPassableTileWithTraversalDistance(rootTile, minDist, maxDist, out tile, TileValidator, ignoreFirstTilePassability: true, TileFinderMode.Near, canTraverseImpassable))
            {
                return true;
            }
        }
        tile = -1;
        return false;

        static bool TileValidator(PlanetTile t)
        {
            if (Find.WorldObjects.AnyWorldObjectAt(t))
            {
                return false;
            }
            BiomeDef biome = Find.WorldGrid[t].PrimaryBiome;
            if (!biome.canBuildBase || !biome.canAutoChoose)
            {
                return false;
            }
            float latitude = Find.WorldGrid.LongLatOf(t).y;
            if (latitude >= 50f || latitude <= -50f)
            {
                return true;
            }
            return false;
        }
    }
}
