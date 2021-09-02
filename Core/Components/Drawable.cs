using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncDrawable);
            packet.Writer.Write(entity.ID);

            ref var drawable = ref entity.GetComponent<Drawable>();

            packet.Writer.Write(ref drawable.AtlasRect);
            packet.Writer.Write(drawable.Texture);
            packet.Writer.Write(drawable.Layer);
            packet.Writer.Write(ref drawable.Scale);
            packet.Writer.Write(ref drawable.Color);
            packet.Writer.Write(ref drawable.MinSize);
            packet.Writer.Write(drawable.MaxZoomLevel);

            packet.DataCount += 1;
        }

        public static int read = 0;

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var drawable = new Drawable();

            drawable.AtlasRect = reader.ReadRectangle();
            drawable.Texture = reader.ReadString();
            drawable.Layer = reader.ReadInt32();
            drawable.Scale = reader.ReadVector2();
            drawable.Color = reader.ReadRgbaByte();
            drawable.MinSize = reader.ReadVector2();
            drawable.MaxZoomLevel = reader.ReadInt32();

            drawable.TextureReference = AssetManager.LoadTexture2D(drawable.Texture);

            if (drawable.AtlasRect.IsZero)
                drawable.AtlasRect = new Rectangle(new Vector2I(0), drawable.TextureReference.Size);

            drawable.Origin = drawable.AtlasRect.SizeF / 2;

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(drawable);
        }
    }

    public struct WorldIcon
    {
        public Rectangle AtlasRect;
        public Vector2 Origin;
        public Vector2 Scale;
        public string Texture;
        public int Layer;
        
        [JsonIgnore] public Texture2D TextureReference;

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncWorldIcon);
            packet.Writer.Write(entity.ID);

            ref var worldIcon = ref entity.GetComponent<WorldIcon>();

            packet.Writer.Write(ref worldIcon.AtlasRect);
            packet.Writer.Write(worldIcon.Texture);
            packet.Writer.Write(worldIcon.Layer);
            packet.Writer.Write(ref worldIcon.Scale);

            packet.DataCount += 1;
        }

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var worldIcon = new WorldIcon();

            worldIcon.AtlasRect = reader.ReadRectangle();
            worldIcon.Texture = reader.ReadString();
            worldIcon.Layer = reader.ReadInt32();
            worldIcon.Scale = reader.ReadVector2();

            worldIcon.TextureReference = AssetManager.LoadTexture2D(worldIcon.Texture);

            if (worldIcon.AtlasRect.IsZero)
                worldIcon.AtlasRect = new Rectangle(new Vector2I(0), worldIcon.TextureReference.Size);

            worldIcon.Origin = worldIcon.AtlasRect.SizeF / 2;

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(worldIcon);
        }
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
