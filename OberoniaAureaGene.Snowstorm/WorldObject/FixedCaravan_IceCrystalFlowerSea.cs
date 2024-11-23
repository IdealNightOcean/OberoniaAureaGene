using OberoniaAurea_Frame;
using RimWorld;
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
        foreach (Pawn pawn in PawnsListForReading)
        {
            pawn.needs.mood?.thoughts.memories.TryGainMemory(Snowstrom_ThoughtDefOf.OAGene_Thought_IceCrystalFlowerSea);
        }
        FixedCaravanUtility.ConvertToCaravan(this);
        assoicateFlowerSea?.EndWeeding();
    }

    protected override void PreConvertToCaravanByPlayer()
    {
        assoicateFlowerSea?.InterruptWeeding();
        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("OAGene_IceCrystalFlowerSeaLeaveHalfway".Translate(), null));
    }


    public override void Notify_ConvertToCaravan() { }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref assoicateFlowerSea, "assoicateFlowerSea");
    }
}
