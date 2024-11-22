using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class MapComponent_Snowstorm : MapComponent
{
    public List<Comp_SnowyCrystalTree> snowyCrystalTreeComps = [];
    public int SnowyCrystalTreeCount => snowyCrystalTreeComps.Count;
    public MapComponent_Snowstorm(Map map) : base(map) { }

}
