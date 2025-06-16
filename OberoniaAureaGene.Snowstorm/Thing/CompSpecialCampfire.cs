using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class CompProperties_SpecialCampfire : CompProperties
{
    public CompProperties_SpecialCampfire()
    {
        compClass = typeof(CompSpecialCampfire);
    }
}

public class CompSpecialCampfire : ThingComp
{
    protected bool specialCampfire = false;
    protected bool firstRefueled = true;
    protected const string RefueledQuestSignal = "CampfireLighted";
    public override void ReceiveCompSignal(string signal)
    {
        base.ReceiveCompSignal(signal);
        if (specialCampfire && firstRefueled && signal == CompRefuelable.RefueledSignal)
        {
            firstRefueled = false;
            if (parent.Spawned && parent.Map.Parent is not null)
            {
                MapParent hometown = parent.Map.Parent;
                QuestUtility.SendQuestTargetSignals(hometown.questTags, RefueledQuestSignal, hometown.Named("SUBJECT"));
            }
        }
    }

    public void InitSpecialCampfire()
    {
        CompRefuelable refuelable = parent.GetComp<CompRefuelable>();
        if (refuelable is not null)
        {
            refuelable.allowAutoRefuel = false;
            OAFrame_ReflectionUtility.SetFieldValue(refuelable, "fuel", 0f);
        }
        specialCampfire = true;
        firstRefueled = true;
    }
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref specialCampfire, "specialCampfire", defaultValue: false);
        Scribe_Values.Look(ref firstRefueled, "firstRefueled", defaultValue: false);
    }
}
