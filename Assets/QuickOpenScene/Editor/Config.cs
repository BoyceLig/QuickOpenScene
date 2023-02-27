using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
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

        public const string sceneConfigDatePath = "UserSettings/QuickOpenSceneConfigData.json";

        //菜单路径
        public struct MenuPath
        {
            public const string quickOpenSceneWindow = "Tools/Quick Open Scene/快速打开场景 %&X";
            public const string aboutWindow = "Tools/Quick Open Scene/关于";
            public const string refreshQOSWindow = "Tools/Quick Open Scene/刷新主界面";
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
            public const string githubChangeLog = "https://raw.githubusercontent.com/BoyceLig/QuickOpenScene/main/Assets/QuickOpenScene/ChangeLog.txt";
            public const string qqGroup = "https://jq.qq.com/?_wv=1027&k=7ap29Woh";
        }

        static string pluginPath;
        /// <summary>
        /// 获取插件的路径
        /// </summary>
        public static string PluginPath
        {
            get
            {
                //PluginPath有数据，但是挪位置了，从新查找位置
                if (!Directory.Exists(GetValue(ref pluginPath, "PluginPath", SceneConfigManage.GetPluginPath())))
                {
                    SetValue(ref pluginPath, "PluginPath", SceneConfigManage.GetPluginPath());
                }
                return pluginPath;
            }
        }

        //场景配置文件数据
        static SceneConfig sceneConfig;
        /// <summary>
        /// 获取SceneConfig配置文件
        /// </summary>
        public static SceneConfig SceneConfigData
        {
            get
            {
                if (sceneConfig == null)
                {
                    if (File.Exists(sceneConfigDatePath))
                    {
                        sceneConfig = SceneConfigManage.ReadSceneConfig();
                    }
                    else
                    {
                        sceneConfig = SceneConfigManage.CreateSceneConfig();
                    }
                }
                return sceneConfig;
            }
        }

        static string latestVersion;
        /// <summary>
        /// 最新版本
        /// </summary>
        public static string LatestVersion
        {
            get
            {
                if (latestVersion == null)
                {
                    latestVersion = SessionState.GetString("QuickOpenSceneLatestVersion", currVersion);
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

        static string latestDownloadURL;
        /// <summary>
        /// 最新版本下载地址
        /// </summary>
        public static string LatestDownloadURL
        {
            get
            {
                if (latestDownloadURL == null)
                {
                    latestDownloadURL = SessionState.GetString("QuickOpenSceneLatestDownloadURL", URL.githubReleases);
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

        static bool versionIsDown;
        /// <summary>
        /// 是否下载完成
        /// </summary>
        public static bool VersionIsDown
        {
            get
            {
                versionIsDown = SessionState.GetBool("VersionIsDown", false);
                return versionIsDown;
            }
            set
            {
                SessionState.SetBool("VersionIsDown", value);
                versionIsDown = value;
            }
        }

        static bool changeLogIsDown;
        public static bool ChangeLogIsDown
        {
            get
            {
                changeLogIsDown = SessionState.GetBool("ChangeLogIsDown", false);
                return changeLogIsDown;
            }
            set
            {
                if (changeLogIsDown != value)
                {
                    SessionState.SetBool("ChangeLogIsDown", value);
                    changeLogIsDown = value;
                }
            }
        }

        static Version currVersionV, latestVersionV;
        static bool needUpdata;
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
            get
            {
                if (sortbyIndex == 0)
                {
                    sortbyIndex = SessionState.GetInt("SortbyIndex", 0);
                }
                return sortbyIndex;
            }

            set
            {
                if (sortbyIndex != value)
                {
                    SessionState.SetInt("SortbyIndex", value);
                    sortbyIndex = value;
                }
            }
        }

        static int groupIndexPanel;
        /// <summary>
        /// 当前分组的面板序号
        /// </summary>
        public static int GroupIndexPanel
        {
            get
            {
                if (groupIndexPanel == 0)
                {
                    groupIndexPanel = SessionState.GetInt("GroupIndexPanel", 0);
                }
                else if (groupIndexPanel > SceneConfigData.groupConfigs.Count)
                {
                    groupIndexPanel = SceneConfigData.groupConfigs.Count;
                }
                return groupIndexPanel;
            }
            set
            {
                if (value != groupIndexPanel)
                {
                    SessionState.SetInt("GroupIndexPanel", value);
                    groupIndexPanel = value;
                    EditorWindow.GetWindow<QOSWindow>().SendEvent(EditorGUIUtility.CommandEvent("GroupIndexPanelChange"));
                }
            }
        }

        /// <summary>
        /// 当前面板分组所对应的数据分组index数
        /// </summary>
        public static int CurrGroupIndex
        {
            get => GroupIndexPanel > 0 ? GroupIndexPanel - 1 : 0;
        }

        static int autoOpenAbout;
        /// <summary>
        /// 需要更新时是否自动打开关于面板
        /// </summary>
        public static int AutoOpenAbout
        {
            get => GetValue(ref autoOpenAbout, "AutoOpenAbout", 0);
            set => SetValue(ref autoOpenAbout, "AutoOpenAbout", value);
        }

        static DateTime updateTime;
        public static DateTime UpdateTime
        {
            get
            {
                if (updateTime == null)
                {
                    updateTime = DateTime.Parse(SessionState.GetString("UpdateTime", new DateTime(1971, 1, 1).ToString()));
                }
                return updateTime;
            }
            set
            {
                if (updateTime != value)
                {
                    updateTime = value;
                    SessionState.SetString("UpdateTime", value.ToString());
                }
            }
        }

        static string[] groupStr;
        /// <summary>
        /// 面板分组
        /// </summary>
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
            set
            {
                if (groupStr != value)
                {
                    groupStr = value;
                }
            }
        }

        static string logText;
        public static string LogText
        {
            get
            {
                if (logText == null)
                {
                    logText = SessionState.GetString("QOSLogText", AssetDatabase.LoadAssetAtPath<TextAsset>(PluginPath + "/ChangeLog.txt").text);
                }
                return logText;
            }
            set
            {
                if (logText != value)
                {
                    SessionState.SetString("QOSLogText", value);
                    logText = value;
                }
            }
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
                string temp = EditorUserSettings.GetConfigValue(name);
                if (temp == null)
                {
                    SetValue(ref t, name, defaultValue);
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    t = (T)converter.ConvertFrom(temp);
                }
            }
            return t;
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
            if (t == null || !t.Equals(value))
            {
                EditorUserSettings.SetConfigValue(name, value.ToString());
                t = value;
            }

        }
    }
}