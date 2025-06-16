using RimWorld;
using Verse;

namespace OberoniaAureaGene;
public class Gene_AgriculturalEnthusiasm : Gene
{
    public bool activeThought;
    private int noAgriculturalTicks;
    public override void Tick()
    {
        if (pawn.IsHashIntervalTick(250))
        {
            CheckPawnWork();
        }
    }
    private void CheckPawnWork()
    {
        WorkTypeDef workType = pawn.CurJob?.workGiverDef?.workType;
        if (workType is null)
        {
            noAgriculturalTicks += 250;
        }
        else if (workType == WorkTypeDefOf.Growing || workType == WorkTypeDefOf.PlantCutting)
        {
            noAgriculturalTicks = 0;
        }
        else
        {
            noAgriculturalTicks += 250;
        }
        activeThought = noAgriculturalTicks > 420000;
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref activeThought, "activeThought", defaultValue: false);
        Scribe_Values.Look(ref noAgriculturalTicks, "noAgriculturalTicks", 0);
    }
}

