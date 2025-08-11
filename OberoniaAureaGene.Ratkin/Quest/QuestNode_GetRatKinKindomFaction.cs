using RimWorld;
using RimWorld.QuestGen;

namespace OberoniaAureaGene.Ratkin;

public class QuestNode_GetRatkinKindomFaction : OberoniaAurea_Frame.QuestNode_GetFaction
{
    protected override bool IsGoodFaction(Faction faction, Slate slate)
    {
        if (!faction.IsRatkinKindomFaction())
        {
            return false;
        }
        return base.IsGoodFaction(faction, slate);
    }
}
