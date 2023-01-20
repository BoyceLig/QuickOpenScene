using System;
using UnityEditor;
using UnityEngine.Windows;

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

    /// <summary>
    /// 排序
    /// </summary>
    public enum Sortby
    {
        Default, SceneName
    }

    public class Config
    {
        //当前版本
        public const string currVersion = "1.5";
        static bool isDown;
        static bool needUpdata;
        static string latestVersion;
        static string latestDownloadURL;
        const string token = "Z2hwX0puTTd2clM4QUZuWlcxWmx4TjUyR01IT3lWWmdEbTBTc1p6MQ==";
        //插件路径
        static string pluginPath;
        //场景配置文件数据
        static SceneConfig sceneConfig;
        //排序选项
        static int sortbyIndex;


        //菜单路径
        public struct MenuPath
        {
            public const string quickOpenSceneWindow = "Tools/Quick Open Scene/快速打开场景 %&X";
            public const string aboutWindow = "Tools/Quick Open Scene/关于";
            public const string addCurrScene = "Assets/Tools/Quick Open Scene/添加当前目录场景或此场景到配置文件";
            public const string addAllScene = "Assets/Tools/Quick Open Scene/添加所有场景到配置文件";
        }

        //关于界面的数据
        public struct URL
        {
            public const string github = "https://github.com/BoyceLig/QuickOpenScene";
            public const string githubIssues = "https://github.com/BoyceLig/QuickOpenScene/issues";
            public const string githubReleases = "https://github.com/BoyceLig/QuickOpenScene/releases";
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
                        pluginPath = SceneConfigManage.GetPluginPath();
                        EditorUserSettings.SetConfigValue("pluginPath", pluginPath);
                    }
                    else
                    {
                        pluginPath = EditorUserSettings.GetConfigValue("pluginPath");
                        if (!Directory.Exists(pluginPath))
                        {
                            pluginPath = SceneConfigManage.GetPluginPath();
                        }
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
                    string path = PluginPath + "/Data/QuickOpenSceneConfigData.asset";
                    if (File.Exists(path))
                    {
                        sceneConfig = AssetDatabase.LoadAssetAtPath<SceneConfig>(path);
                    }
                    else
                    {
                        sceneConfig = SceneConfigManage.CreateSceneConfig();
                    }
                }
                return sceneConfig;
            }
        }

        /// <summary>
        /// 最新版本
        /// </summary>
        public static string LatestVersion
        {
            get
            {
                latestVersion = SessionState.GetString("QuickOpenSceneLatestVersion", currVersion);
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

        /// <summary>
        /// 最新版本下载地址
        /// </summary>
        public static string LatestDownloadURL
        {
            get
            {
                if (latestDownloadURL == null)
                {
                    latestDownloadURL = URL.githubReleases;
                }
                else
                {
                    if (latestDownloadURL != SessionState.GetString("QuickOpenSceneLatestDownloadURL", URL.githubReleases))
                    {
                        latestDownloadURL = SessionState.GetString("QuickOpenSceneLatestDownloadURL", URL.githubReleases);
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

        /// <summary>
        /// 是否下载完成
        /// </summary>
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

        public static string GetHead()
        {
            byte[] temp = Convert.FromBase64String(token);
            return System.Text.Encoding.UTF8.GetString(temp);
        }


        static Version currVersionV, latestVersionV;
        /// <summary>
        /// 是否需要更新
        /// </summary>
        public static bool NeedUpdate
        {
            get
            {
                if (currVersionV == null)
                {
                    currVersionV = new Version(currVersion);
                }
                if (latestVersionV == null)
                {
                    latestVersionV = new Version(LatestVersion);
                }

                if (currVersionV < latestVersionV)
                {
                    needUpdata = true;
                };
                return needUpdata;
            }
        }

        /// <summary>
        /// 排序选项
        /// </summary>
        public static int SortbyIndex
        {
            get
            {
                if (EditorUserSettings.GetConfigValue("sortbyIndex") == null || EditorUserSettings.GetConfigValue("sortbyIndex") == string.Empty)
                {
                    sortbyIndex = 0;
                    EditorUserSettings.SetConfigValue("sortbyIndex", sortbyIndex.ToString());
                }
                else
                {
                    sortbyIndex = int.Parse(EditorUserSettings.GetConfigValue("sortbyIndex"));
                }
                return sortbyIndex;
            }
            set
            {
                EditorUserSettings.SetConfigValue("sortbyIndex", value.ToString());
                sortbyIndex = value;
            }
        }
    }
}