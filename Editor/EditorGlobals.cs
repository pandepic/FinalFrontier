using ElementEngine.TexturePacker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class EditorGlobals
    {
        public static string AssetsRoot = "";
        public static Dictionary<string, IntPtr> TexturePtrs = new Dictionary<string, IntPtr>();
        public static TexturePackerAtlas WorldAssetsAtlas;
        public static IntPtr WorldAssetsAtlasPtr;
    }
}
