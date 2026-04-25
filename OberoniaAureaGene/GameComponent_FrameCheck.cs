using RimWorld;
using Verse;

namespace OberoniaAureaGene;

public class GameComponent_FrameCheck : GameComponent
{
    public GameComponent_FrameCheck(Game game) { }

    public override void StartedNewGame()
    {
        base.StartedNewGame();
        GameStart();
    }

    public override void LoadedGame()
    {
        base.LoadedGame();
        GameStart();
    }

    private void GameStart()
    {
        if (!ModsConfig.IsActive("OARK.OberoniaAurea.Framework") && !ModsConfig.IsActive("OARK.OberoniaAurea.Framework_Steam"))
        {
            Find.LetterStack.ReceiveLetter(
                label: "OARO_LetterLabel_FrameMiss".Translate(),
                text: "OARO_LetterText_FrameMiss".Translate(),
                textLetterDef: LetterDefOf.NegativeEvent,
                delayTicks: 300);
        }
    }
}
