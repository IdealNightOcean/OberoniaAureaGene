using HarmonyLib;
using System.Reflection;
using Verse;

namespace OberoniaAureaGene;


[StaticConstructorOnStartup]
public static class ModHarmonyPatch
{
    public static Harmony harmonyInstance;

    public static Harmony HarmonyInstance
    {
        get
        {
            harmonyInstance ??= new Harmony("OberoniaAureaGene_Harmony");
            return harmonyInstance;
        }
    }

    static ModHarmonyPatch()
    {
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }
}