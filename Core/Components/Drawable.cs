using ElementEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct Drawable
    {
        public Rectangle AtlasRect;
        public Vector2 Origin;
        public Vector2 Scale;
        public string Texture;
        public int Layer;

        public Vector2 MinSize;
        public int MaxZoomLevel;

        //[JsonIgnore] public Texture2D TextureReference;
    }
}
