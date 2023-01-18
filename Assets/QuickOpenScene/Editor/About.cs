using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class AboutWindow : EditorWindow
    {
        GUIStyle titleStyle, linkStyle,nameStyle;
        Vector2 logscrollPosition;
        string logText;
        string latestVersion, latestDownloadURL;

        [MenuItem(Config.MenuPath.aboutWindow, priority = 2001)]
        static void OpenAbout()
        {
            Vector2 defaultSize = new Vector2(400, 500); ;
            AboutWindow window = GetWindow<AboutWindow>(true, "关于");

            window.minSize = defaultSize;
            window.maxSize = defaultSize;
            window.Show();
        }
        private void OnEnable()
        {
            logText = SceneConfigManage.GetLogText();
            latestVersion = SceneConfigManage.GetJsonData("Assets/QuickOpenScene/Editor/test.json",SceneConfigManage.GetJsonType.LatestTag);
            latestDownloadURL = SceneConfigManage.GetJsonData("Assets/QuickOpenScene/Editor/test.json", SceneConfigManage.GetJsonType.LatestDownloadURL);
        }
        private void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 18;
                titleStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (nameStyle ==null)
            {
                nameStyle = new GUIStyle(EditorStyles.label);
                nameStyle.alignment= TextAnchor.MiddleRight;
            }

            if (linkStyle == null)
            {
                linkStyle = new GUIStyle(EditorStyles.label);
                linkStyle.normal.textColor = new Color(0.2980392f, 0.4901961f, 1f);
                linkStyle.hover.textColor = Color.white;
                linkStyle.active.textColor = Color.gray;
                linkStyle.alignment = TextAnchor.MiddleLeft;
                linkStyle.margin.left = 8;
            }

            EditorGUILayout.Separator();
            //标题
            GUILayout.Label("快速打开场景（Quick Open Scene）", titleStyle);
            GUILayout.Label("By:Boyce Lig", nameStyle);
            EditorGUILayout.Separator();
            //升级日志部分
            GUILayout.Label("升级日志：", EditorStyles.boldLabel);
            logscrollPosition = GUILayout.BeginScrollView(logscrollPosition, "ProgressBarBack", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            GUILayout.Label(logText, "WordWrappedMiniLabel", GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            EditorGUILayout.Separator();

            //相关信息部分
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("相关链接：");
            if (GUILayout.Button("Github", linkStyle))
            {
                Application.OpenURL(Config.About.githubURL);
            }
            GUILayout.Label("-");
            if (GUILayout.Button("Github Releases", linkStyle))
            {
                Application.OpenURL(Config.About.githubReleasesURL);
            }
            GUILayout.Label("-");
            if (GUILayout.Button("Github Issues", linkStyle))
            {
                Application.OpenURL(Config.About.githubIssuesURL);
            }            
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("当前版本：" + Config.version);
            GUILayout.Label("最新版本：" + latestVersion);
        }
    }
}