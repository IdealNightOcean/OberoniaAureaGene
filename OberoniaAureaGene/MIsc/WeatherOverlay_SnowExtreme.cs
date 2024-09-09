using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public class WeatherOverlay_SnowExtreme : SkyOverlay
{
    private static readonly Material SnowOverlayWorld = MatLoader.LoadMat("Weather/SnowOverlayWorld");

    public WeatherOverlay_SnowExtreme()
    {
        worldOverlayMat = SnowOverlayWorld;
        worldOverlayPanSpeed1 = 0.016f;
        worldPanDir1 = new Vector2(-0.75f, -1f);
        worldPanDir1.Normalize();
        worldOverlayPanSpeed2 = 0.018f;
        worldPanDir2 = new Vector2(-0.72f, -1f);
        worldPanDir2.Normalize();
        forceOverlayColor = true;
        forcedColor = new Color(0.6f, 1f, 1f);
    }
}
