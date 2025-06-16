using HarmonyLib;
using RimWorld;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public static class GetDodgeChance_Patch
{
    static GetDodgeChance_Patch()
    {
        if (OberoniaAureaGene_Settings.DodgeChancePatch)
        {
            ModHarmonyPatch.HarmonyInstance.Patch(AccessTools.Method(typeof(Verb_MeleeAttack), "GetDodgeChance"), null, new HarmonyMethod(typeof(GetDodgeChance_Patch), "GetDodgeChance_Postfix"));
        }
    }

    public static void GetDodgeChance_Postfix(ref float __result, LocalTargetInfo target)
    {
        if (__result == 0f || target.Thing is not Pawn pawn)
        {
            return;
        }
        bool flag = Rand.Chance(__result);
        if (flag)
        {
            __result = 2f;
            return;
        }
        if (pawn.genes is not null && pawn.genes.HasActiveGene(OAGene_GeneDefOf.OAGene_MeleeIntouchable))
        {
            float dodgeChance = __result > 0.5f ? 0.5f : __result;
            flag = Rand.Chance(dodgeChance);
            __result = flag ? 2f : (Rand.Chance(dodgeChance) ? 2f : dodgeChance);
        }
    }
}
