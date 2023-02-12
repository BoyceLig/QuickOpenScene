using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class RefreshQOSWindow : MonoBehaviour
    {
        [MenuItem(Config.MenuPath.refreshQOSWindow)]
        static void RefreshQOSWindowInit()
        {
            AssetDatabase.ImportAsset(Config.PluginPath + "/Editor/QOSWindow.cs");
        }
    }
}
