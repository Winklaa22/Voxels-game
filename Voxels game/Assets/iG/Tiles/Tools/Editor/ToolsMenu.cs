using UnityEngine;
using UnityEditor;

namespace ig.Tiles.Tools
{
    public class ToolsMenu : EditorWindow
    {
        static string s_TileAtlasBuilderHelpfFile = "iG/Tiles/Tools/Resources/iGTileTools/iG Tile-Atlas Builder manual.pdf";

        [MenuItem("Window/iG Tile Tools/Tile-Atlas Builder")]
        static TileAtlasBuilder ShowTileAtlasBuilderWindow()
        {
            TileAtlasBuilder win = GetWindow<TileAtlasBuilder>("Atlas Editor");
            win.Setup(new Vector2(390, 580), s_TileAtlasBuilderHelpfFile);
            return win;
        }
    }
}
