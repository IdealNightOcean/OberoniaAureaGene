using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_EndGame_GetHometownTile : QuestNode
{
    [NoTranslate]
    public SlateRef<string> storeAs;
    protected override bool TestRunInt(Slate slate)
    {
        if(GetHometownTile(out int hometownTile))
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
        if (GetHometownTile(out int hometownTile))
        {
            slate.Set(storeAs.GetValue(slate), hometownTile);
        }     
    }

    protected bool GetHometownTile(out int hometownTile)
    {
        hometownTile = Tile.Invalid;
        GameComponent_SnowstormStory storyGameComp = Snowstorm_StoryUtility.StoryGameComp;
        if (storyGameComp == null)
        {
            return false;
        }
        hometownTile = storyGameComp.hometownTile;
        if (hometownTile == Tile.Invalid)
        {
            WorldObject hometown_Sealed = Find.WorldObjects.AllWorldObjects.Where(w => w.def == Snowstrom_MiscDefOf.OAGene_Hometown_Sealed).FirstOrFallback(null);
            if (hometown_Sealed != null)
            {
                hometownTile = hometown_Sealed.Tile;
            }
            else
            {
                hometownTile = GetNewHometownTile();
            }
        }
        return true;
    }

    protected static int GetNewHometownTile()
    {
        return -1;
    }
}
