using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

[StaticConstructorOnStartup]
public class Thought_LostInMemory : Thought_Memory
{
    private static readonly IntRange RandomStage = new(0, 7);
    public override void Init()
    {
        base.Init();
        SetForcedStage(RandomStage.RandomInRange);
    }
}
