using HarmonyLib;
using JetBrains.Annotations;
using System.Reflection;
using Verse;

namespace OberoniaAureaGene;


[UsedImplicitly]
[StaticConstructorOnStartup]
public static class ModHarmonyPatch
{
    private static Harmony harmonyInstance;

    internal static Harmony HarmonyInstance
    {
        get
        {
            harmonyInstance ??= new Harmony("OberoniaAureaGene_Hramony");
            return harmonyInstance;
        }
    }

    static ModHarmonyPatch()
    {
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }
}