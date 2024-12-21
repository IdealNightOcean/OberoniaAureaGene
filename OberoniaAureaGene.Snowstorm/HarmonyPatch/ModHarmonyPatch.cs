using HarmonyLib;
using System.Reflection;
using Verse;

namespace OberoniaAureaGene.Snowstorm;


[StaticConstructorOnStartup]
public static class ModHarmonyPatch
{
    public static Harmony harmonyInstance;

    public static Harmony HarmonyInstance
    {
        get
        {
            harmonyInstance ??= new Harmony("OberoniaAureaGene.Snowstorm.Hramony");
            return harmonyInstance;
        }
    }

    static ModHarmonyPatch()
    {
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }
}