using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WorldObject_IceCrystalFlowerSea : WorldObject_InteractWithFixedCaravanBase
{
    public override int TicksNeeded => 30000;
    protected override string VisitLabel => "OAGene_Visit_IceCrystalFlowerSea".Translate();

    public override string FixedCaravanWorkDesc() => "OAGene_FixedCaravanIceCrystalFlowerSea_TimeLeft".Translate(ticksRemaining.ToStringTicksToPeriod());

    public override bool StartWork(Caravan caravan)
    {
        Dialog_NodeTree nodeTree = OAFrame_DiaUtility.ConfirmDiaNodeTree
        (
            "OAGene_IceCrystalFlowerSea_Text".Translate(),
            "OAGene_IceCrystalFlowerSea_Confirm".Translate(TicksNeeded.ToStringTicksToPeriod(shortForm: true)),
            delegate { base.StartWork(caravan); },
            "GoBack".Translate()
        );
        Find.WindowStack.Add(nodeTree);
        return true;
    }

    protected override void FinishWork()
    {
        List<Thing> rewards = OAFrame_MiscUtility.TryGenerateThing(Snowstorm_ThingDefOf.OAGene_IceCrystal, Rand.RangeInclusive(550, 650));
        OAFrame_FixedCaravanUtility.GiveThings(associatedFixedCaravan, rewards);

        foreach (Pawn pawn in associatedFixedCaravan.PawnsListForReading)
        {
            pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_IceCrystalFlowerSea);
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstorm_ThoughtDefOf.OAGene_Thought_IceCrystalFlowerSea);
        }
        Dialog_NodeTree nodeTree = OAFrame_DiaUtility.DefaultConfirmDiaNodeTree("OAGene_IceCrystalFlowerSea_Finished".Translate());
        Find.WindowStack.Add(nodeTree);

        Destroy();
    }

    protected override void InterruptWork()
    {
        foreach (Pawn pawn in associatedFixedCaravan.PawnsListForReading)
        {
            pawn.health.AddHediff(Snowstorm_HediffDefOf.OAGene_Hediff_IceCrystalFlowerSea);
        }
        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_IceCrystalFlowerSea_LeaveHalfway".Translate(), null));
    }

}
