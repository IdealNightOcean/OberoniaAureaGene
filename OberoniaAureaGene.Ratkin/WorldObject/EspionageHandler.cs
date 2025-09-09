using OberoniaAurea_Frame;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace OberoniaAureaGene.Ratkin;

[StaticConstructorOnStartup]
public class EspionageHandler(Site site) : IExposable, IFixedCaravanAssociate
{
    private const int TicksNeeded = 10000;
    private static readonly List<(Action, float)> tmpPossibleOutcomes = [];

    public string FixedCaravanName => null;

    private readonly Site AssociatedSite = site ?? throw new ArgumentNullException(nameof(site));
    [Unsaved] private EspionageSiteComp associatedEspionageComp;

    private FixedCaravan associatedFixedCaravan;
    public FixedCaravan AssociatedFixedCaravan => associatedFixedCaravan;

    private bool isWorking;
    private int ticksRemaining = TicksNeeded;

    public bool IsWorking => isWorking;

    public void WorkTick()
    {
        if (isWorking && ticksRemaining-- == 0)
        {
            FinishWork();
        }
    }

    public void StartWork(Caravan caravan)
    {
        associatedEspionageComp = AssociatedSite.GetComponent<EspionageSiteComp>();
        if (associatedEspionageComp is null)
        {
            CancelWork();
            return;
        }

        FixCaravan_EspionageSite fixedCaravan = (FixCaravan_EspionageSite)OAFrame_FixedCaravanUtility.CreateFixedCaravan(caravan, OAGene_RatkinDefOf.OAGene_FixedCaravan_Espionage, AssociatedSite);
        if (fixedCaravan is null)
        {
            CancelWork();
            return;
        }
        associatedFixedCaravan = fixedCaravan;
        Find.WorldObjects.Add(fixedCaravan);
        Find.WorldSelector.Select(fixedCaravan);
        isWorking = true;
        ticksRemaining = TicksNeeded;
    }

    public string FixedCaravanWorkDesc() => "OAGene_FixedCaravanEspionae_TimeLeft".Translate(ticksRemaining.ToStringTicksToPeriod());

    private void EndWork(bool interrupt, bool convertToCaravan)
    {
        if (isWorking)
        {
            isWorking = false;
            if (interrupt)
            {
                Fail(associatedEspionageComp);
            }
            else
            {
                FinishWork();
                convertToCaravan = false; // FinishWork中会构造远行队
            }
        }

        if (convertToCaravan && associatedFixedCaravan is not null)
        {
            OAFrame_FixedCaravanUtility.ConvertToCaravan(associatedFixedCaravan);
        }
        Reset();
    }

    private void FinishWork()
    {
        if (associatedFixedCaravan is null)
        {
            return;
        }

        Caravan caravan = OAFrame_FixedCaravanUtility.ConvertToCaravan(associatedFixedCaravan);
        associatedFixedCaravan = null;
        TryGetOutCome(caravan);
    }

    public void PreConvertToCaravanByPlayer()
    {
        EndWork(interrupt: true, convertToCaravan: false);
    }

    public void PostConvertToCaravan(Caravan caravan) { }

    public void CancelWork()
    {
        if (isWorking)
        {
            EndWork(interrupt: true, convertToCaravan: true);
        }
        else
        {
            if (associatedFixedCaravan is not null)
            {
                OAFrame_FixedCaravanUtility.ConvertToCaravan(associatedFixedCaravan);
            }
            Reset();
        }
    }

    private void Reset()
    {
        isWorking = false;
        ticksRemaining = TicksNeeded;
        associatedFixedCaravan = null;
    }

    public void TryGetOutCome(Caravan caravan)
    {
        tmpPossibleOutcomes.Clear();
        tmpPossibleOutcomes.Add((delegate
        {
            Success(associatedEspionageComp);
        }, 15f));
        tmpPossibleOutcomes.Add((delegate
        {
            Fail(associatedEspionageComp);
        }, 50f));
        tmpPossibleOutcomes.Add((delegate
        {
            SuccessButBeFound(caravan, associatedEspionageComp);
        }, 25f));
        tmpPossibleOutcomes.Add((delegate
        {
            FailAndBeBeFound(caravan, associatedEspionageComp);
        }, 10f));
        tmpPossibleOutcomes.RandomElementByWeight(x => x.Item2).Item1();
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref isWorking, "isWorking", defaultValue: false);
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);

        Scribe_References.Look(ref associatedFixedCaravan, "associatedFixedCaravan");

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            associatedEspionageComp = AssociatedSite.GetComponent<EspionageSiteComp>();
        }
    }

    protected static void Success(EspionageSiteComp espionageSiteComp)
    {
        Site site = espionageSiteComp.Site;

        QuestUtility.SendQuestTargetSignals(site.questTags, "OAGene_EspionageSuccess", site.Named("SUBJECT"));
        Messages.Message("OAGene_MessageEspionageSuccess".Translate(), MessageTypeDefOf.PositiveEvent);
        espionageSiteComp?.Notify_EspionageEnd(destory: true, succeeded: true);
    }
    public static void Fail(EspionageSiteComp espionageSiteComp)
    {
        Messages.Message("OAGene_MessageEspionageFail".Translate(), MessageTypeDefOf.NeutralEvent);
        espionageSiteComp?.Notify_EspionageEnd(destory: false, succeeded: false, cooldownTicks: 15000);
    }

    protected static void SuccessButBeFound(Caravan caravan, EspionageSiteComp espionageSiteComp)
    {
        Site site = espionageSiteComp.Site;

        Faction.OfPlayer.TryAffectGoodwillWith(site.Faction, -15, reason: OAGene_RatkinDefOf.OAGene_SuspectedBehavior);
        QuestUtility.SendQuestTargetSignals(site.questTags, "OAGene_EspionageSuccess", site.Named("SUBJECT"));
        if (site.Faction.HostileTo(Faction.OfPlayer))
        {
            Messages.Message("OAGene_MessageEspionageSuccessButBeFound".Translate(), MessageTypeDefOf.ThreatBig);
            new CaravanArrivalAction_VisitSite(site).Arrived(caravan);
            espionageSiteComp?.Notify_EspionageEnd(destory: false, succeeded: true);
        }
        else
        {
            Messages.Message("OAGene_MessageEspionageSuccessButBeFound".Translate(), MessageTypeDefOf.NegativeEvent);
            espionageSiteComp?.Notify_EspionageEnd(destory: true, succeeded: true);
        }
    }
    protected static void FailAndBeBeFound(Caravan caravan, EspionageSiteComp espionageSiteComp)
    {
        Site site = espionageSiteComp.Site;

        Faction.OfPlayer.TryAffectGoodwillWith(site.Faction, -15, reason: OAGene_RatkinDefOf.OAGene_SuspectedBehavior);
        espionageSiteComp?.Notify_EspionageEnd(destory: false, succeeded: false, cooldownTicks: 15000);
        if (site.Faction.HostileTo(Faction.OfPlayer))
        {
            Messages.Message("OAGene_MessageEspionageFailAndBeBeFound".Translate(), MessageTypeDefOf.ThreatBig);
            new CaravanArrivalAction_VisitSite(site).Arrived(caravan);
        }
        else
        {
            Messages.Message("OAGene_MessageEspionageFailAndBeBeFound".Translate(), MessageTypeDefOf.NegativeEvent);
        }
    }

}