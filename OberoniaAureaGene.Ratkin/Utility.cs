using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public static class OAGene_RatkinUtility
{
    public static bool IsRatkin(this Pawn pawn)
    {
        return pawn.def == OAGene_RatkinDefOf.Ratkin || pawn.def == OAGene_RatkinDefOf.Ratkin_Su;
    }
}