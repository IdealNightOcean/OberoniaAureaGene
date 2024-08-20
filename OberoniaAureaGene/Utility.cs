using UnityEngine;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public static class OAGeneUtility
{
    public static MapComponent_OberoniaAureaGene GetOAGeneMapComp(this Map map)
    {
        return map.GetComponent<MapComponent_OberoniaAureaGene>();
    }
}