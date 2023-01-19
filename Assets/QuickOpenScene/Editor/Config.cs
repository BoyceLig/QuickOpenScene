using UnityEditor;

namespace QuickOpenScene
{
    /// <summary>
    /// Json获取的数据
    /// </summary>
    public enum GetJsonType
    {
        LatestTag, LatestDownloadURL
    }

    /// <summary>
    /// 单个场景数据类型
    /// </summary>
    public enum SceneConfigInfoType
    {
        scenePath, sceneGUID
    }

    public class Config
    {
        //当前版本
        public const string currVersion = "1.5";
        static bool isDown;
        static string latestVersion;
        static string latestDownloadURL;
        //插件路径
        static string pluginPath;
        //场景配置文件数据
        static SceneConfig sceneConfig;


        //菜单路径
        public struct MenuPath
        {
            public const string quickOpenSceneWindow = "Tools/Quick Open Scene/快速打开场景 %&X";
            public const string aboutWindow = "Tools/Quick Open Scene/关于";
            public const string addCurrScene = "Assets/Tools/Quick Open Scene/添加当前目录场景或此场景到配置文件";
            public const string addAllScene = "Assets/Tools/Quick Open Scene/添加所有场景到配置文件";
        }

        //关于界面的数据
        public struct About
        {
            public const string githubURL = "https://github.com/BoyceLig/QuickOpenScene";
            public const string githubIssuesURL = "https://github.com/BoyceLig/QuickOpenScene/issues";
            public const string githubReleasesURL = "https://github.com/BoyceLig/QuickOpenScene/releases";
            public const string githubLatestAPI = "https://api.github.com/repos/BoyceLig/QuickOpenScene/releases/latest";
        }

        /// <summary>
        /// 获取插件的路径
        /// </summary>
        public static string PluginPath
        {
            get
            {
                if (pluginPath == null || pluginPath == string.Empty)
                {
                    if (EditorUserSettings.GetConfigValue("pluginPath") == null || EditorUserSettings.GetConfigValue("pluginPath") == string.Empty)
                    {
                        string guid = AssetDatabase.FindAssets("QuickOpenSceneConfigData")[0];
                        pluginPath = AssetDatabase.GUIDToAssetPath(guid);
                        pluginPath = pluginPath.Remove(pluginPath.IndexOf("/Data/QuickOpenSceneConfigData.asset"));
                        EditorUserSettings.SetConfigValue("pluginPath", pluginPath);
                    }
                    else
                    {
                        pluginPath = EditorUserSettings.GetConfigValue("pluginPath");
                    }
                }
                return pluginPath;
            }
        }

        /// <summary>
        /// 获取SceneConfig配置文件
        /// </summary>
        public static SceneConfig SceneConfigData
        {
            get
            {
                if (sceneConfig == null)
                {
                    string guid = AssetDatabase.FindAssets("QuickOpenSceneConfigData")[0];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    sceneConfig = AssetDatabase.LoadAssetAtPath<SceneConfig>(path);
                }
                return sceneConfig;
            }
        }

        //最新版本
        public static string LatestVersion
        {
            get
            {
                if (latestVersion == null)
                {
                    latestVersion = currVersion;
                }
                else
                {
                    if (latestVersion != SessionState.GetString("QuickOpenSceneLatestVersion", currVersion))
                    {
                        latestVersion = SessionState.GetString("QuickOpenSceneLatestVersion", currVersion);
                    }
                }
                return latestVersion;
            }
            set
            {
                if (latestVersion != value)
                {
                    latestVersion = value;
                    SessionState.SetString("QuickOpenSceneLatestVersion", value);
                }

            }
        }

        public static string LatestDownloadURL
        {
            get
            {
                if (latestDownloadURL == null)
                {
                    latestDownloadURL = About.githubReleasesURL;
                }
                else
                {
                    if (latestDownloadURL != SessionState.GetString("QuickOpenSceneLatestDownloadURL", About.githubReleasesURL))
                    {
                        latestDownloadURL = SessionState.GetString("QuickOpenSceneLatestDownloadURL", About.githubReleasesURL);
                    }
                }
                return latestDownloadURL;
            }
            set
            {
                if (latestDownloadURL != value)
                {
                    latestDownloadURL = value;
                    SessionState.SetString("QuickOpenSceneLatestDownloadURL", value);
                }

            }
        }

        //是否已经下载了js
        public static bool IsDown
        {
            get
            {
                isDown = SessionState.GetBool("QuickOpenSceneIsDown", false);
                return isDown;
            }
            set
            {
                SessionState.SetBool("QuickOpenSceneIsDown", value);
                isDown = value;
            }
        }


    }
}