using System;
using System.ComponentModel;
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

    public class Config
    {
        //当前版本
        public const string currVersion = "1.9.0";
        static bool isDown, needUpdata;
        static string pluginPath, latestVersion, latestDownloadURL;
        static int autoOpenAbout;
        //场景配置文件数据
        static SceneConfig sceneConfig;

        static string[] groupStr;

        //菜单路径
        public struct MenuPath
        {
            public const string quickOpenSceneWindow = "Tools/Quick Open Scene/快速打开场景 %&X";
            public const string aboutWindow = "Tools/Quick Open Scene/关于";
            public const string addCurrScene = "Assets/Tools/Quick Open Scene/添加当前目录场景或此场景到当前分组";
            public const string addAllScene = "Assets/Tools/Quick Open Scene/添加所有场景到当前分组";
        }

        //关于界面的数据
        public struct URL
        {
            public const string github = "https://github.com/BoyceLig/QuickOpenScene";
            public const string githubIssues = "https://github.com/BoyceLig/QuickOpenScene/issues";
            public const string githubReleases = "https://github.com/BoyceLig/QuickOpenScene/releases";
            public const string githubLatestAPI = "https://api.github.com/repos/BoyceLig/QuickOpenScene/releases/latest";
            public const string qqGroup = "https://jq.qq.com/?_wv=1027&k=7ap29Woh";
        }

        /// <summary>
        /// 获取插件的路径
        /// </summary>
        public static string PluginPath
        {
            get
            {
                if (!Directory.Exists(GetValue(ref pluginPath, "PluginPath", SceneConfigManage.GetPluginPath())))
                {
                    SetValue(ref pluginPath, "PluginPath", SceneConfigManage.GetPluginPath());
                }
                return GetValue(ref pluginPath, "PluginPath", SceneConfigManage.GetPluginPath());
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

        static int sortbyIndex;
        /// <summary>
        /// 排序选项
        /// </summary>
        public static int SortbyIndex
        {
            get => SessionState.GetInt("SortbyIndex", 0);
            set => SessionState.SetInt("SortbyIndex", value);
        }


        public static int GroupIndexPanel
        {
            get
            {
                int temp = SessionState.GetInt("GroupIndexPanel", 0);
                if (temp > SceneConfigData.groupConfigs.Count)
                {
                    temp = SceneConfigData.groupConfigs.Count;
                }
                return temp;
            }
            set
            {
                if (value != SessionState.GetInt("GroupIndexPanel", 0))
                {
                    SessionState.SetInt("GroupIndexPanel", value);
                    EditorWindow.GetWindow<QOSWindow>().SendEvent(EditorGUIUtility.CommandEvent("GroupIndexPanelChange"));
                }
            }
        }

        /// <summary>
        /// 需要更新时是否自动打开关于面板
        /// </summary>
        public static int AutoOpenAbout
        {
            get => GetValue(ref autoOpenAbout, "AutoOpenAbout", 0);
            set => SetValue(ref autoOpenAbout, "AutoOpenAbout", value);
        }

        public static DateTime UpdateTime
        {
            get
            {
                return DateTime.Parse(SessionState.GetString("UpdateTime", new DateTime(1971, 1, 1).ToString()));
            }
            set
            {
                SessionState.SetString("UpdateTime", value.ToString());
            }
        }

        public static string[] GroupStr
        {
            get
            {
                if (groupStr == null || groupStr.Length != SceneConfigData.groupConfigs.Count + 1)
                {
                    groupStr = new string[SceneConfigData.groupConfigs.Count + 1];
                    groupStr[0] = ("所有");
                    for (int i = 0; i < SceneConfigData.groupConfigs.Count; i++)
                    {
                        groupStr[i + 1] = SceneConfigData.groupConfigs[i].groupName;
                    }
                }
                return groupStr;
            }
            set { groupStr = value; }
        }



        /// <summary>
        /// 属性中的获取数据，如果为空则创建一个默认的并储存到 EditorUserSettings
        /// </summary>
        /// <typeparam name="T">数据的类型</typeparam>
        /// <param name="name">储存命名</param>
        /// <param name="t">私有函数</param>
        /// <param name="defaultValue">默认数值</param>
        /// <returns>最终的数值</returns>
        static T GetValue<T>(ref T t, string name, T defaultValue)
        {
            if (t == null)
            {
                if (EditorUserSettings.GetConfigValue(name) == null || EditorUserSettings.GetConfigValue(name) == string.Empty)
                {
                    EditorUserSettings.SetConfigValue(name, defaultValue.ToString());
                    t = defaultValue;
                    return t;
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    t = (T)converter.ConvertFrom(EditorUserSettings.GetConfigValue(name));
                    return t;
                }
            }
            else
            {
                return t;
            }
        }

        /// <summary>
        /// 赋值并保存到 EditorUserSettings
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="name">储存命名</param>
        /// <param name="t">私有函数</param>
        /// <param name="value">值</param>
        /// <returns>最终的数值</returns>
        static void SetValue<T>(ref T t, string name, T value)
        {
            EditorUserSettings.SetConfigValue(name, value.ToString());
            t = value;
        }
    }
}