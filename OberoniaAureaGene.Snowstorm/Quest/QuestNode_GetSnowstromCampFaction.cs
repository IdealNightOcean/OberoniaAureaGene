using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.QuestGen;
using System.Linq;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class QuestNode_GetSnowstormCampFaction : QuestNode
{
    [NoTranslate]
    public SlateRef<string> storeAs;

    public SlateRef<FactionDef> factionDef;

    protected override bool TestRunInt(Slate slate)
    {
        Map map = slate.Get<Map>("map");
        if (map is null)
        {
            return false;
        }
        Faction faction = GetFaction(slate);
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
        Faction faction = GetFaction(slate);
        if (faction is not null)
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
        Faction faction = OAFrame_FactionUtility.ValidTempFactionsOfDef(factionDef).Where(f => !f.HostileTo(Faction.OfPlayer)).RandomElementWithFallback();
        faction ??= OAFrame_FactionUtility.GenerateTempFaction(factionDef);
        return faction;
    }
}
