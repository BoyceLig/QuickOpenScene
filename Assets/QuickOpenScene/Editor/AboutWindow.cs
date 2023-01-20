using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class AboutWindow : EditorWindow
    {
        GUIStyle titleStyle, linkStyle, nameStyle, logTextStyle;
        Vector2 logscrollPosition;
        string logText;

        [MenuItem(Config.MenuPath.aboutWindow, priority = 2001)]
        public static void OpenAbout()
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
        }
        public void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 18;
                titleStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (nameStyle == null)
            {
                nameStyle = new GUIStyle(EditorStyles.label);
                nameStyle.alignment = TextAnchor.MiddleRight;
            }

            if (linkStyle == null)
            {
                linkStyle = new GUIStyle(EditorStyles.label);
                linkStyle.fontSize = 12;
                linkStyle.normal.textColor = new Color(0.2980392f, 0.4901961f, 1f);
                linkStyle.hover.textColor = Color.white;
                linkStyle.active.textColor = Color.gray;
                linkStyle.margin.top = 3;
            }

            if (logTextStyle == null)
            {
                logTextStyle = new GUIStyle("WordWrappedMiniLabel");
                logTextStyle.wordWrap = false;
            }

            EditorGUILayout.Separator();
            //标题
            GUILayout.Label("快速打开场景（Quick Open Scene）", titleStyle);
            GUILayout.Label("By:Boyce Lig", nameStyle);
            EditorGUILayout.Separator();
            //升级日志部分
            GUILayout.Label("升级日志：", EditorStyles.boldLabel);
            logscrollPosition = GUILayout.BeginScrollView(logscrollPosition, "ProgressBarBack", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            GUILayout.Label(logText, logTextStyle);
            GUILayout.EndScrollView();
            EditorGUILayout.Separator();

            //相关信息部分
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("相关链接：", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Github", linkStyle, GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(Config.URL.github);
            }
            GUILayout.Label("-", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Github Releases", linkStyle, GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(Config.URL.githubReleases);
            }
            GUILayout.Label("-", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Github Issues", linkStyle, GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(Config.URL.githubIssues);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("当前版本：", GUILayout.ExpandWidth(false));
            GUILayout.Label(Config.currVersion, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("最新版本：", GUILayout.ExpandWidth(false));
            if (Config.LatestVersion != string.Empty)
            {
                if (Config.NeedUpdate)
                {
                    Color cache = GUI.color;
                    GUI.color = Color.red;
                    GUILayout.Label(Config.LatestVersion, GUILayout.ExpandWidth(false));
                    GUI.color = cache;
                    GUILayout.Label("-", GUILayout.ExpandWidth(false));
                    if (GUILayout.Button("下载最新版", linkStyle, GUILayout.ExpandWidth(false)))
                    {
                        Application.OpenURL(Config.LatestDownloadURL);
                    }
                }
                else
                {
                    var cache = GUI.color;
                    GUI.color = Color.green;
                    GUILayout.Label(Config.LatestVersion, GUILayout.ExpandWidth(false));
                    GUILayout.Label("-", GUILayout.ExpandWidth(false));
                    GUILayout.Label("最新版本", GUILayout.ExpandWidth(false));
                    GUI.color = cache;
                }
            }
            if (GUILayout.Button("手动检查更新", GUILayout.ExpandWidth(false)))
            {
                new GetVersionInformation().GetJson(true);
            }

            EditorGUILayout.EndHorizontal();
            Repaint();
        }
    }
}