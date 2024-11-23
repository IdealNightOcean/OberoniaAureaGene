using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_GetSnowstromCampFaction : QuestNode
{
    [NoTranslate]
    public SlateRef<string> storeAs;

    public SlateRef<FactionDef> factionDef;

    protected override bool TestRunInt(Slate slate)
    {
        Map map = slate.Get<Map>("map");
        if (map == null)
        {
            return false;
        }
        Faction faction = GetFaction(slate);
        if (faction != null)
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
        if (map == null)
        {
            return;
        }
        Faction faction = GetFaction(slate);
        if (faction != null)
        {
            slate.Set(storeAs.GetValue(slate), faction);
            QuestPart_InvolvedFactions questPart_InvolvedFactions = new();
            questPart_InvolvedFactions.factions.Add(faction);
            QuestGen.quest.AddPart(questPart_InvolvedFactions);
        }
    }

    protected Faction GetFaction(Slate slate)
    {
        FactionDef factionDef = this.factionDef.GetValue(slate);
        Faction faction = OAFrame_FactionUtility.RandomTempFactionOfDef(factionDef);
        faction ??= OAFrame_FactionUtility.GenerateTempFaction(factionDef);
        return faction;
    }
}
