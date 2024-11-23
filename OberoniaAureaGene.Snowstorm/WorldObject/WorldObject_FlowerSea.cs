using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class WorldObject_IceCrystalFlowerSea : WorldObject_InteractiveBase
{
    public bool workStart;
    protected const int WeedingTicks = 30000;
    public override void Notify_CaravanArrived(Caravan caravan)
    {
        if (!workStart)
        {
            DiaNode startNode = OAFrame_DiaUtility.ConfirmDiaNode
            (
                "OAGene_IceCrystalFlowerSea_Text".Translate(),
                "OAGene_IceCrystalFlowerSea_Confirm".Translate(WeedingTicks.ToStringTicksToPeriod(shortForm: true)),
                delegate { StartWeeding(caravan); },
                "GoBack".Translate()
            );
            Dialog_NodeTree nodeTree = new(startNode);
            Find.WindowStack.Add(nodeTree);
        }
        else
        {
            Messages.Message("OAGene_Message_IceCrystalFlowerSeaWeeding".Translate(), MessageTypeDefOf.RejectInput, historical: false);
        }
    }
    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
    {
        foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(caravan))
        {
            yield return floatMenuOption;
        }
        yield return new FloatMenuOption("OAGene_WeedingIceCrystalFlowerSea", delegate { Notify_CaravanArrived(caravan); });
    }
    protected void StartWeeding(Caravan caravan)
    {
        if (!FixedCaravanUtility.IsExactTypeCaravan(caravan))
        {
            return;
        }
        FixedCaravan_IceCrystalFlowerSea fixedCaravan = (FixedCaravan_IceCrystalFlowerSea)FixedCaravanUtility.CreateFixedCaravan(caravan, Snowstrom_MiscDefOf.OAGene_FixedCaravan_IceCrystalFlowerSea, WeedingTicks);
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
