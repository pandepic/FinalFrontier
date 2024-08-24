using ElementEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public enum UICursorType
    {
        Normal,
        Interact,
        Talk,
    }

    public static class UICursors
    {
        public static void Setup()
        {
            CursorManager.AddCursor(UICursorType.Normal, "Cursors/cursor_normal.png");
            CursorManager.AddCursor(UICursorType.Interact, "Cursors/cursor_interact.png");
            CursorManager.AddCursor(UICursorType.Talk, "Cursors/cursor_talk.png");
        }

        public static void SetCursor(UICursorType type)
        {
            CursorManager.SetCursor(type);
        }
    } // UICursors
}
