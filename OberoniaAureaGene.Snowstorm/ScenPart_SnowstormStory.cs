using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public class ScenPart_SnowstormStory : ScenPart
{
    public override void PostGameStart()
    {
        base.PostGameStart();
        Current.Game.GetComponent<GameComponent_SnowstormStory>()?.Notify_StoryActive();
    }
}
