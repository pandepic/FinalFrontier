using ElementEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public abstract class EditorWindow
    {
        public EditorWindowType Type;
        public bool IsActive;

        public EditorWindow(EditorWindowType type)
        {
            Type = type;
        }

        public abstract void Update(GameTimer gameTimer);
        public abstract void Draw(GameTimer gameTimer);

        public virtual void OnShow() { }
    }
}
