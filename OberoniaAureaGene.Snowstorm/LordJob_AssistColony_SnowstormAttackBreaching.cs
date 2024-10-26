using RimWorld;

namespace OberoniaAureaGene.Snowstorm;

public class LordJob_AssistColony_SnowstormAttackBreaching : LordJob_AssaultColony
{
    public LordJob_AssistColony_SnowstormAttackBreaching()
    { }

    public LordJob_AssistColony_SnowstormAttackBreaching(SpawnedPawnParams parms) : base(parms)
    { }
    public LordJob_AssistColony_SnowstormAttackBreaching(Faction assaulterFaction, bool canKidnap = true, bool canTimeoutOrFlee = true, bool sappers = false, bool useAvoidGridSmart = false, bool canSteal = true, bool breachers = false, bool canPickUpOpportunisticWeapons = false) : base(assaulterFaction, canKidnap, canTimeoutOrFlee, sappers, useAvoidGridSmart, canSteal, breachers, canPickUpOpportunisticWeapons)
    { }
    public override void Notify_LordDestroyed()
    {
        OnDefeat();
    }

    private void OnDefeat()
    { }
}
