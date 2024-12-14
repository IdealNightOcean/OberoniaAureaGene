using UnityEngine;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class PlaceWorker_RoofProtectRadius : PlaceWorker
{
    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
    {
        if (OAGene_SnowstormSettings.ShowColumnProtectRadius)
        {
            GenDraw.DrawRadiusRing(center, OberoniaAureaGene_Settings.ColumnProtectRadius, Color.green);
        }
    }
}
