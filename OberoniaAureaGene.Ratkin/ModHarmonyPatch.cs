using HarmonyLib;
using System.Reflection;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public static class ModHarmonyPatch
{
    public static Harmony harmonyInstance;

    public static Harmony HarmonyInstance
    {
        get
        {
            harmonyInstance ??= new Harmony("OberoniaAureaGene.Ratkin.Hramony");
            return harmonyInstance;
        }
    }

    static ModHarmonyPatch()
    {
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }
}