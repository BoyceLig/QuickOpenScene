using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class QuickOpenScenePrefs
    {
        [SettingsProvider]
        static SettingsProvider EditorPreferences()
        {
            var provider = new SettingsProvider("Preferences/Quick Open Scene", SettingsScope.User)
            {
                guiHandler = (searchContext) =>
                {
                    EditorPreferencesGUI();
                }
            };
            return provider;
        }
        static void EditorPreferencesGUI()
        {
            EditorGUILayout.HelpBox("Quick Open Scene 的配置文件位置！", MessageType.Info);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("配置文件位置：", GUILayout.Width(140f));
            StaticConfig.sceneConfigInfoPath = GUILayout.TextField(StaticConfig.sceneConfigInfoPath);
            EditorUserSettings.SetConfigValue("sceneConfigInfoPath", StaticConfig.sceneConfigInfoPath);
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Version: " + StaticConfig.VERSION, EditorStyles.centeredGreyMiniLabel);
        }
    }
}
