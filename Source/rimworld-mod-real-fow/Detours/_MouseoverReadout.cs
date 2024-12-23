using UnityEngine;
using Verse;

namespace RimWorldRealFoW.Detours;

public static class _MouseoverReadout
{
    private static readonly Vector2 BotLeft = new Vector2(15f, 65f);

    public static bool MouseoverReadoutOnGUI_Prefix(MouseoverReadout __instance)
    {
        bool result;
        if (Event.current.type != EventType.Repaint)
        {
            result = true;
        }
        else
        {
            if (Find.MainTabsRoot.OpenTab != null)
            {
                result = true;
            }
            else
            {
                var c = UI.MouseCell();
                if (!c.InBounds(Find.CurrentMap))
                {
                    result = true;
                }
                else
                {
                    var mapComponentSeenFog = Find.CurrentMap.getMapComponentSeenFog();
                    if (!c.Fogged(Find.CurrentMap) && mapComponentSeenFog != null &&
                        !mapComponentSeenFog.knownCells[Find.CurrentMap.cellIndices.CellToIndex(c)])
                    {
                        GenUI.DrawTextWinterShadow(new Rect(256f, UI.screenHeight - 256, -256f, 256f));
                        Text.Font = GameFont.Small;
                        GUI.color = new Color(1f, 1f, 1f, 0.8f);
                        var rect = new Rect(BotLeft.x, UI.screenHeight - BotLeft.y, 999f, 999f);
                        Widgets.Label(rect, "NotVisible".Translate());
                        GUI.color = Color.white;
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
        }

        return result;
    }
}