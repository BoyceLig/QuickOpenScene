using UnityEditor;

namespace QuickOpenScene
{
    public class RefreshQOSWindow
    {
        [MenuItem(Config.MenuPath.refreshQOSWindow)]
        static void RefreshQOSWindowInit()
        {
            AssetDatabase.ImportAsset(Config.PluginPath + "/Editor/QOSWindow.cs");
        }
    }
}
