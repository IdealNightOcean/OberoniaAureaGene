using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_GetSnowstormRaidFaction : QuestNode
{
    [NoTranslate]
    public SlateRef<string> storeAs;

    protected override bool TestRunInt(Slate slate)
    {
        Map map = slate.Get<Map>("map");
        if (map is null)
        {
            return false;
        }
        Faction faction = SnowstormUtility.RandomSnowstormMaliceRaidableFaction(map);
        if (faction is not null)
        {
            slate.Set(storeAs.GetValue(slate), faction);
            return true;
        }
        return false;
    }
    protected override void RunInt()
    {
        Slate slate = QuestGen.slate;
        Map map = slate.Get<Map>("map");
        if (map is null)
        {
            return;
        }
        Faction faction = SnowstormUtility.RandomSnowstormMaliceRaidableFaction(map);
        if (faction is not null)
        {
            slate.Set(storeAs.GetValue(slate), faction);
            QuestPart_InvolvedFactions questPart_InvolvedFactions = new();
            questPart_InvolvedFactions.factions.Add(faction);
            QuestGen.quest.AddPart(questPart_InvolvedFactions);
        }
    }
}
