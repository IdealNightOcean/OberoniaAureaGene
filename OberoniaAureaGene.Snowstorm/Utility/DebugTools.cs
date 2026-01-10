using LudeonTK;
using RimWorld;
using Verse;

namespace OberoniaAureaGene.Snowstorm;

public static class DebugTools
{
    [DebugAction(category: "OberoniaAurea",
                 name: "极端暴风雪调试",
                 requiresRoyalty: false,
                 requiresIdeology: false,
                 requiresBiotech: false,
                 requiresAnomaly: false,
                 requiresOdyssey: false,
                 displayPriority: 150,
                 hideInSubMenu: false,
                 actionType = DebugActionType.Action,
                 allowedGameStates = AllowedGameStates.Playing)]
    private static void OpenSnowstormDevWindow()
    {
        if (GameComponent_Snowstorm.Instance is null)
        {
            Messages.Message("GameComponent_Snowstorm is null", MessageTypeDefOf.RejectInput, historical: false);
            return;
        }
        GameComponent_Snowstorm.OpenDevWindow();
    }

    [DebugAction(category: "OberoniaAurea",
             name: "风雪遗孤调试",
             requiresRoyalty: false,
             requiresIdeology: false,
             requiresBiotech: false,
             requiresAnomaly: false,
             requiresOdyssey: false,
             displayPriority: 140,
             hideInSubMenu: false,
             actionType = DebugActionType.Action,
             allowedGameStates = AllowedGameStates.Playing)]
    private static void OpenSnowstormStoryDevWindow()
    {
        if (GameComponent_SnowstormStory.Instance is null)
        {
            Messages.Message("GameComponent_SnowstormStory is null", MessageTypeDefOf.RejectInput, historical: false);
            return;
        }
        GameComponent_SnowstormStory.OpenDevWindow();
    }
}
