using Verse;

namespace OberoniaAureaGene;

public interface IGeneIncomingDamageProcessor
{
    void PreApplyDamage(ref DamageInfo dinfo);
}
