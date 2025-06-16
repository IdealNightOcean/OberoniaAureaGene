using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class WeatherOverlay_SnowExtremeFog : WeatherOverlayDualPanner
{
    private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/BloodFogOverlayWorld");

    public WeatherOverlay_SnowExtremeFog()
    {
        worldOverlayMat = FogOverlayWorld;
        worldOverlayPanSpeed1 = 0.0005f;
        worldOverlayPanSpeed2 = 0.0004f;
        worldPanDir1 = new Vector2(1f, 1f);
        worldPanDir1.Normalize();
        worldPanDir2 = new Vector2(0.5f, -0.1f);
        worldPanDir2.Normalize();
        ForcedOverlayColor = new Color(0.6f, 1f, 1f);
    }
}