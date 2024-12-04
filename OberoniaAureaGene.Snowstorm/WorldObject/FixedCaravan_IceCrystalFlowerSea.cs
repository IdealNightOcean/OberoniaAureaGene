using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class FixedCaravan_IceCrystalFlowerSea : FixedCaravan
{
    public WorldObject_IceCrystalFlowerSea assoicateFlowerSea;

    public override void Tick()
    {
        ticksRemaining--;
        if (ticksRemaining < 0)
        {
            FinishedWork();
        }
    }
    private void FinishedWork()
    {
        List<Thing> rewards = OAFrame_MiscUtility.TryGenerateThing(Snowstrom_ThingDefOf.OAGene_IceCrystal, 600);
        foreach (Thing reward in rewards)
        {
            OAFrame_FixedCaravanUtility.GiveThing(this, reward);
        }
        foreach (Pawn pawn in PawnsListForReading)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstrom_ThoughtDefOf.OAGene_Thought_IceCrystalFlowerSea);
        }
        Caravan caravan = OAFrame_FixedCaravanUtility.ConvertToCaravan(this);
        assoicateFlowerSea?.EndWeeding();
        Dialog_NodeTree nodeTree = OAFrame_DiaUtility.DefaultConfirmDiaNodeTree("OAGene_IceCrystalFlowerSea_Finished".Translate());
        Find.WindowStack.Add(nodeTree);
    }

    protected override void PreConvertToCaravanByPlayer()
    {
        assoicateFlowerSea?.InterruptWeeding();
        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_IceCrystalFlowerSea_LeaveHalfway".Translate(), null));
    }
    public override string GetInspectString()
    {
        StringBuilder stringBuilder = new(base.GetInspectString());
        stringBuilder.AppendInNewLine("OAGene_FixedCaravanIceCrystalFlowerSea_TimeLeft".Translate(ticksRemaining.ToStringTicksToPeriod()));
        return stringBuilder.ToString();
    }

    public override void Notify_ConvertToCaravan() { }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref assoicateFlowerSea, "assoicateFlowerSea");
    }
}
