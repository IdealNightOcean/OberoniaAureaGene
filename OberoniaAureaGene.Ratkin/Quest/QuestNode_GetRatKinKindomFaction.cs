using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.QuestGen;

namespace OberoniaAureaGene.Ratkin;

public class QuestNode_GetRatKinKindomFaction : QuestNode_GetFactionBase
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
