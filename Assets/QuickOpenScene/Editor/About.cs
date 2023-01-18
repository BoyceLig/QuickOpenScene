using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class AboutWindow : EditorWindow
    {
        GUIStyle titleStyle, labelStyle;

        [MenuItem(Config.MenuPath.aboutWindow, priority = 2001)]
        static void OpenAbout()
        {
            AboutWindow window = GetWindow<AboutWindow>(true, "关于");
            window.Show();
        }
        private void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 18;
                titleStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle("BoldLabel");
            }
            EditorGUILayout.Separator();
            
            
            GUILayout.Label("Quick Open Scene", titleStyle);
            GUILayout.Label("Latest Update");



        }
    }
}