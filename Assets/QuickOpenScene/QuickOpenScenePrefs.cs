using System;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class QuickOpenScenePrefs
    {
        static KeyCode key;

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
            if (EditorUserSettings.GetConfigValue("sceneConfigInfoPath") != null && EditorUserSettings.GetConfigValue("sceneConfigInfoPath") != string.Empty)
            {
                StaticConfig.sceneConfigInfoPath = EditorUserSettings.GetConfigValue("sceneConfigInfoPath");
            }
            else
            {
                EditorUserSettings.SetConfigValue("sceneConfigInfoPath", StaticConfig.defaultSceneConfigInfoPath);
                StaticConfig.sceneConfigInfoPath = EditorUserSettings.GetConfigValue("sceneConfigInfoPath");
            }
            StaticConfig.sceneConfigInfoPath = GUILayout.TextField(StaticConfig.sceneConfigInfoPath);
            EditorUserSettings.SetConfigValue("sceneConfigInfoPath", StaticConfig.sceneConfigInfoPath);
            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Label("打开面板快捷键：", GUILayout.Width(140f));
            //GetKey(StaticConfig.key1, StaticConfig.key2, StaticConfig.key3, StaticConfig.key4);

            //StaticConfig.key1 = (StaticConfig.Key)EditorGUILayout.EnumPopup(StaticConfig.key1, GUILayout.Width(60));
            //StaticConfig.key2 = (StaticConfig.Key)EditorGUILayout.EnumPopup(StaticConfig.key2, GUILayout.Width(60));
            //StaticConfig.key3 = (StaticConfig.Key)EditorGUILayout.EnumPopup(StaticConfig.key3, GUILayout.Width(60));
            //StaticConfig.key4 = GUILayout.TextField(StaticConfig.key4);
            //if (StaticConfig.key4 == null || StaticConfig.key4 == string.Empty)
            //{
            //    StaticConfig.key4 = string.Empty;
            //}
            //else
            //{
            //    StaticConfig.key4 = StaticConfig.key4.ToUpper()[0].ToString();
            //}

            //SetKey(StaticConfig.key1, StaticConfig.key2, StaticConfig.key3, StaticConfig.key4);
            //EditorGUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Version: " + StaticConfig.VERSION, EditorStyles.centeredGreyMiniLabel);
        }

        public static string GetKey(StaticConfig.Key key1, StaticConfig.Key key2, StaticConfig.Key key3, string key4)
        {
            if (EditorUserSettings.GetConfigValue("key1") != null)
            {
                StaticConfig.key1 = (StaticConfig.Key)Enum.Parse(typeof(StaticConfig.Key), EditorUserSettings.GetConfigValue("key1"));
            }
            else
            {
                StaticConfig.key1 = StaticConfig.defaultKey1;
                EditorUserSettings.SetConfigValue("key1", StaticConfig.defaultKey1.ToString());
            }
            if (EditorUserSettings.GetConfigValue("key2") != null)
            {
                StaticConfig.key2 = (StaticConfig.Key)Enum.Parse(typeof(StaticConfig.Key), EditorUserSettings.GetConfigValue("key2"));
            }
            else
            {
                StaticConfig.key2 = StaticConfig.defaultKey2;
                EditorUserSettings.SetConfigValue("key2", StaticConfig.defaultKey2.ToString());
            }
            if (EditorUserSettings.GetConfigValue("key3") != null)
            {
                StaticConfig.key3 = (StaticConfig.Key)Enum.Parse(typeof(StaticConfig.Key), EditorUserSettings.GetConfigValue("key3"));
            }
            else
            {
                StaticConfig.key3 = StaticConfig.defaultKey3;
                EditorUserSettings.SetConfigValue("key3", StaticConfig.defaultKey3.ToString());
            }
            if (EditorUserSettings.GetConfigValue("key4") != null)
            {
                StaticConfig.key4 = EditorUserSettings.GetConfigValue("key4");
            }
            else
            {
                StaticConfig.key4 = StaticConfig.defaultKey4;
                EditorUserSettings.SetConfigValue("key4", StaticConfig.defaultKey4.ToString());
            }

            string key01, key02, key03, keyall;
            switch (key1)
            {
                case StaticConfig.Key.None:
                    key01 = string.Empty;
                    break;
                case StaticConfig.Key.Ctrl:
                    key01 = "%";
                    break;
                case StaticConfig.Key.Shift:
                    key01 = "#";
                    break;
                case StaticConfig.Key.Alt:
                    key01 = "&";
                    break;
                default:
                    goto case StaticConfig.Key.None;
            }
            switch (key2)
            {
                case StaticConfig.Key.None:
                    key02 = string.Empty;
                    break;
                case StaticConfig.Key.Ctrl:
                    key02 = "%";
                    break;
                case StaticConfig.Key.Shift:
                    key02 = "#";
                    break;
                case StaticConfig.Key.Alt:
                    key02 = "&";
                    break;
                default:
                    goto case StaticConfig.Key.None;
            }
            switch (key3)
            {
                case StaticConfig.Key.None:
                    key03 = string.Empty;
                    break;
                case StaticConfig.Key.Ctrl:
                    key03 = "%";
                    break;
                case StaticConfig.Key.Shift:
                    key03 = "#";
                    break;
                case StaticConfig.Key.Alt:
                    key03 = "&";
                    break;
                default:
                    goto case StaticConfig.Key.None;
            }

            keyall = key01 + key02 + key03 + key4;
            return keyall;
        }
        public static void SetKey(StaticConfig.Key key1, StaticConfig.Key key2, StaticConfig.Key key3, string key4)
        {
            EditorUserSettings.SetConfigValue("key1", StaticConfig.key1.ToString());
            EditorUserSettings.SetConfigValue("key2", StaticConfig.key2.ToString());
            EditorUserSettings.SetConfigValue("key3", StaticConfig.key3.ToString());
            EditorUserSettings.SetConfigValue("key4", StaticConfig.key4);
        }

    }
}
