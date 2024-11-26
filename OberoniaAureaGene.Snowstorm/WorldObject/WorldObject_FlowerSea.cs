using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WorldObject_IceCrystalFlowerSea : WorldObject_InteractiveBase
{
    public bool workStart;
    protected const int WeedingTicks = 30000;

    protected override string VisitLabel => "OAGene_Visit_IceCrystalFlowerSea".Translate();
    public override void Notify_CaravanArrived(Caravan caravan)
    {
        if (!workStart)
        {
            Dialog_NodeTree nodeTree = OAFrame_DiaUtility.ConfirmDiaNodeTree
            (
                "OAGene_IceCrystalFlowerSea_Text".Translate(),
                "OAGene_IceCrystalFlowerSea_Confirm".Translate(WeedingTicks.ToStringTicksToPeriod(shortForm: true)),
                delegate { StartWeeding(caravan); },
                "GoBack".Translate()
            );
            Find.WindowStack.Add(nodeTree);
        }
        else
        {
            Messages.Message("OAGene_Message_IceCrystalFlowerSeaWeeding".Translate(), MessageTypeDefOf.RejectInput, historical: false);
        }
    }

    protected void StartWeeding(Caravan caravan)
    {
        if (!OAFrame_CaravanUtility.IsExactTypeCaravan(caravan))
        {
            return;
        }
        FixedCaravan_IceCrystalFlowerSea fixedCaravan = (FixedCaravan_IceCrystalFlowerSea)OAFrame_FixedCaravanUtility.CreateFixedCaravan(caravan, Snowstrom_MiscDefOf.OAGene_FixedCaravan_IceCrystalFlowerSea, WeedingTicks);
        fixedCaravan.assoicateFlowerSea = this;
        Find.WorldObjects.Add(fixedCaravan);
        Find.WorldSelector.Select(fixedCaravan);
        workStart = true;
    }
    public void InterruptWeeding()
    {
        workStart = false;
    }
    public void EndWeeding()
    {
        Destroy();
    }
}
