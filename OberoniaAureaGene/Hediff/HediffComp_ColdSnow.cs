using RimWorld;
using System.Linq;
using Verse;

namespace OberoniaAureaGene;

public class HediffCompProperties_ColdSnow : HediffCompProperties
{
    public IntRange frostbiteInterval;
    public FloatRange frostbiteDamageRange;
    public HediffCompProperties_ColdSnow()
    {
        compClass = typeof(HediffComp_ColdSnow);
    }
}

public class HediffComp_ColdSnow : HediffComp
{
    public int ticksRemainings = 250;

    HediffCompProperties_ColdSnow Props => props as HediffCompProperties_ColdSnow;
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        ticksRemainings = Props.frostbiteInterval.RandomInRange;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        ticksRemainings--;
        if (ticksRemainings <= 0)
        {
            TakeFrostbite(parent.pawn, Props.frostbiteDamageRange.RandomInRange);
            ticksRemainings = Props.frostbiteInterval.RandomInRange;
        }
    }
    protected static void TakeFrostbite(Pawn pawn, float damageAmount)
    {
        if (pawn.RaceProps.body.AllPartsVulnerableToFrostbite.Where((BodyPartRecord x) => !pawn.health.hediffSet.PartIsMissing(x)).TryRandomElementByWeight((BodyPartRecord x) => x.def.frostbiteVulnerability, out BodyPartRecord bodyPart))
        {
            DamageInfo dinfo = new(DamageDefOf.Frostbite, damageAmount, 0f, -1f, null, bodyPart);
            pawn.TakeDamage(dinfo);
            if (pawn.IsColonist)
            {
                Messages.Message("OAGene_MessageColdSnowFrostbite".Translate(pawn.Named("PAWN"), bodyPart.LabelCap), MessageTypeDefOf.NegativeEvent);
            }
        }
    }
    public override void CompExposeData()
    {
        Scribe_Values.Look(ref ticksRemainings, "ticksRemainings", 0);
    }
}
