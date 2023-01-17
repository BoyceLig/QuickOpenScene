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
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("打开面板快捷键：", GUILayout.Width(140f));
            StaticConfig.key1 = (StaticConfig.Key)EditorGUILayout.EnumPopup(StaticConfig.key1, GUILayout.Width(60));
            StaticConfig.key2 = (StaticConfig.Key)EditorGUILayout.EnumPopup(StaticConfig.key2, GUILayout.Width(60));
            StaticConfig.key3 = (StaticConfig.Key)EditorGUILayout.EnumPopup(StaticConfig.key3, GUILayout.Width(60));
            StaticConfig.key4 = GUILayout.TextField(StaticConfig.key4);

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Version: " + StaticConfig.VERSION, EditorStyles.centeredGreyMiniLabel);
        }

        public static string Key(StaticConfig.Key key1, StaticConfig.Key key2, StaticConfig.Key key3, string key4)
        {
            string key01, key02, key03;
            switch (key1)
            {
                case StaticConfig.Key.None:
                    key01 = string.Empty;
                    break;
                case StaticConfig.Key.Ctrl:
                    key01 = string.Empty;
                    break;
                case StaticConfig.Key.Shift:
                    key01 = string.Empty;
                    break;
                case StaticConfig.Key.Alt:
                    key01 = string.Empty;
                    break;
                default:
                    break;
            }

            return string.Empty;
        }

    }
}
