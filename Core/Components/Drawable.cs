using ElementEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier.Components
{
    public struct Drawable
    {
        public Rectangle AtlasRect;
        public Vector2 Origin;
        public Vector2 Scale;
        public string Texture;
        public int Layer;
        public RgbaByte Color;

        public Vector2 MinSize;
        public int MaxZoomLevel;

        [JsonIgnore] public Texture2D TextureReference;
    }

    public struct WorldIcon
    {
        public Rectangle AtlasRect;
        public Vector2 Origin;
        public Vector2 Scale;
        public string Texture;
        public int Layer;
        
        [JsonIgnore] public Texture2D TextureReference;
    }

    public struct WorldSpaceLabel
    {
        public int TextSize;
        public string Text;
        public RgbaByte Color;
        public int TextOutline;
        public int MarginBottom;
    }
}
