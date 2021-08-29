using ElementEngine;
using ElementEngine.UI;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class EditorUtility
    {
        public static void ImGuiImage(string assetName, Vector2 size)
        {
            if (!AssetManager.Contains(assetName))
                return;

            if (!EditorGlobals.TexturePtrs.ContainsKey(assetName))
                EditorGlobals.TexturePtrs.Add(assetName, IMGUIManager.AddTexture(AssetManager.LoadTexture2D(assetName)));

            ImGui.Image(EditorGlobals.TexturePtrs[assetName], size);
        }

        public static Vector2 ImGuiImageFromAtlas(string sprite, float scale = 1f, Vector4? tint = null)
        {
            if (tint == null)
                tint = Vector4.One;

            if (!EditorGlobals.WorldAssetsAtlas.Sprites.ContainsKey(sprite))
                return Vector2.Zero;

            var sourceRect = EditorGlobals.WorldAssetsAtlas.GetSpriteRect(sprite);

            ImGui.Image(
                EditorGlobals.WorldAssetsAtlasPtr,
                sourceRect.SizeF * scale,
                sourceRect.LocationF * EditorGlobals.WorldAssetsAtlas.Texture.TexelSize,
                sourceRect.BottomRightF * EditorGlobals.WorldAssetsAtlas.Texture.TexelSize,
                tint.Value);

            return ImGui.GetItemRectMin();
        }
    }
}
