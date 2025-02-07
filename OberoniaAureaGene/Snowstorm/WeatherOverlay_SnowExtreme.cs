using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class WeatherOverlay_SnowExtreme : SkyOverlay
{
    protected static readonly Material SnowOverlayWorld = MatLoader.LoadMat("Weather/SnowOverlayWorld");
    protected static readonly CachedTexture MainTex = new("Weather/OAGene_SnowExtreme");

    public WeatherOverlay_SnowExtreme()
    {
        worldOverlayMat = null;
        worldOverlayPanSpeed1 = 0.008f;
        worldPanDir1 = new Vector2(-0.5f, -1f);
        worldPanDir1.Normalize();
        worldOverlayPanSpeed2 = 0.009f;
        worldPanDir2 = new Vector2(-0.48f, -1f);
        worldPanDir2.Normalize();
        forceOverlayColor = true;
        forcedColor = new Color(0.6f, 1f, 1f);
    }

    public override void TickOverlay(Map map)
    {
        if (worldOverlayMat == null)
        {
            Log.Message("Creating new worldOverlayMat");
            Texture2D mainTex = MainTex.Texture;
            worldOverlayMat = new(MaterialPool.MatFrom(mainTex));
            worldOverlayMat.CopyPropertiesFromMaterial(SnowOverlayWorld);
            worldOverlayMat.shader = SnowOverlayWorld.shader;
            worldOverlayMat.SetTexture("_MainTex", mainTex);
            worldOverlayMat.SetTexture("_MainTex2", mainTex);
        }
        base.TickOverlay(map);
    }
}
