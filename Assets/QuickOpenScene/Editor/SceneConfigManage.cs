using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class SceneConfigManage : Editor
    {
        [MenuItem(Config.MenuPath.addCurrScene)]
        static void AddCurrSceneConfig()
        {
            CheckSceneConfig();
            //已选择的guids
            string[] guids = Selection.assetGUIDs;
            List<string> direPaths = new List<string>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Directory.Exists(path))
                {
                    direPaths.Add(path);
                }
                else
                {
                    if (Path.GetExtension(path).Contains(".unity"))
                    {
                        AddScene(Config.CurrGroupIndex, path, SceneConfigInfoType.scenePath);
                    }
                }
            }

            //查找路径内的所有场景文件
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", direPaths.ToArray());

            foreach (var sceneGuid in sceneGuids)
            {
                AddScene(Config.CurrGroupIndex, sceneGuid, SceneConfigInfoType.sceneGUID);
            }
        }

        [MenuItem(Config.MenuPath.addAllScene)]
        static void AddAllSceneConfig()
        {
            CheckSceneConfig();
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            foreach (var guid in guids)
            {
                AddScene(Config.CurrGroupIndex, guid, SceneConfigInfoType.sceneGUID);
            }
        }

        /// <summary>
        /// 添加场景到配置文件
        /// </summary>
        /// <param name="groupIndex">分组数(不是面板的分组数)</param>
        /// <param name="info">路径或者guid</param>
        /// <param name="sceneConfigInfoType">info 的类型</param>
        public static void AddScene(int groupIndex, string info, SceneConfigInfoType sceneConfigInfoType)
        {
            if (SceneConfigData.sceneConfig.groupConfigs.Count <= groupIndex)
            {
                return;
            }
            string path;
            if (sceneConfigInfoType == SceneConfigInfoType.sceneGUID)
            {
                path = AssetDatabase.GUIDToAssetPath(info);
            }
            else
            {
                path = info;
            }
            if (File.Exists(path) && Path.GetExtension(path).Contains(".unity") && path.StartsWith("Assets"))
            {
                SceneConfigInfo temp = new SceneConfigInfo(path, SceneConfigInfoType.scenePath);
                AddScene(groupIndex, temp);
            }
        }

        /// <summary>
        /// 添加场景到配置文件
        /// </summary>
        /// <param name="groupIndex">分组数(不是面板的分组数)</param>
        /// <param name="sceneConfigInfo">场景数据</param>
        public static void AddScene(int groupIndex, SceneConfigInfo sceneConfigInfo)
        {
            if (SceneConfigData.sceneConfig.groupConfigs.Count <= groupIndex)
            {
                return;
            }

            if (SceneConfigData.sceneConfig.groupConfigs[groupIndex].sceneInfos.Count > 0)
            {
                foreach (var item in SceneConfigData.sceneConfig.groupConfigs[groupIndex].sceneInfos)
                {
                    if (item.sceneGUID == sceneConfigInfo.sceneGUID)
                    {
                        return;
                    }
                }
            }

            SceneConfigData.sceneConfig.groupConfigs[groupIndex].sceneInfos.Add(sceneConfigInfo);
            QOSWindow.RefreshGetSceneConfigInfos();

            if (Config.GroupIndexPanel == 0)
            {
                Config.GroupIndexPanel = 1;
            }

            SceneConfigData.instance.SaveDate();
            Debug.Log("添加 " + sceneConfigInfo.sceneName + " 场景到分组" + SceneConfigData.sceneConfig.groupConfigs[groupIndex].groupName + " 成功！", AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneConfigInfo.scenePath));
        }

        /// <summary>
        /// 在配置文件内删除对应分组内的场景配置信息
        /// </summary>
        /// <param name="groupIndex">分组数(不是面板的分组数)</param>
        /// <param name="sceneConfigInfo">场景的配置信息</param>
        public static void RemoveSceneInfo(int groupIndex, SceneConfigInfo sceneConfigInfo)
        {
            Debug.Log("删除 " + sceneConfigInfo.sceneName + " 场景成功！");
            SceneConfigData.sceneConfig.groupConfigs[groupIndex].sceneInfos.Remove(sceneConfigInfo);
            QOSWindow.RefreshGetSceneConfigInfos();
            SceneConfigData.instance.SaveDate();
        }

        /// <summary>
        /// 删除所有分组内的当前场景配置信息
        /// </summary>
        /// <param name="sceneConfigInfo">场景的配置信息</param>
        public static void RemoveSceneInfo(SceneConfigInfo sceneConfigInfo)
        {
            for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
            {
                for (int j = 0; j < SceneConfigData.sceneConfig.groupConfigs[i].sceneInfos.Count; j++)
                {
                    if (SceneConfigData.sceneConfig.groupConfigs[i].sceneInfos[j] == sceneConfigInfo)
                    {
                        Debug.Log("删除 " + SceneConfigData.sceneConfig.groupConfigs[i].groupName + " 分组内的 " + sceneConfigInfo.sceneName + " 场景成功！");
                        SceneConfigData.sceneConfig.groupConfigs[i].sceneInfos.Remove(sceneConfigInfo);
                        QOSWindow.RefreshGetSceneConfigInfos();
                        SceneConfigData.instance.SaveDate();
                    }
                }
            }
        }

        /// <summary>
        /// 获取升级日志的日志信息
        /// </summary>
        /// <returns></returns>
        public static void GetLogText()
        {
            if (Config.NeedUpdate && !Config.ChangeLogIsDown)
            {
                new GetVersionInformation().GetOnlineLog();
            }
            else
            {
                string path = Config.PluginPath + "/ChangeLog.txt";
                Config.LogText = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
            }

        }

        /// <summary>
        /// 获取插件路径
        /// </summary>
        /// <returns>插件根路径（最后不带/）</returns>
        public static string GetPluginPath()
        {
            string[] guids = AssetDatabase.FindAssets("GithubJsonData");
            if (guids != null && guids.Length > 0)
            {
                foreach (string guid in guids)
                {
                    string tempPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (tempPath.Contains("QuickOpenScene/Editor/GithubJsonData.cs") || tempPath.Contains("com.boycelig.quickopenscene/Editor/GithubJsonData.cs"))
                    {
                        string githubJsonDataPath = AssetDatabase.GUIDToAssetPath(guid);
                        return githubJsonDataPath.Remove(githubJsonDataPath.IndexOf("/Editor/GithubJsonData.cs"));
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 检查SceneConfig分组
        /// </summary>
        public static void CheckSceneConfig()
        {
            if (SceneConfigData.sceneConfig.groupConfigs.Count == 0)
            {
                SceneConfigData.sceneConfig.groupConfigs.Add(new GroupConfigInfo("Default", new List<SceneConfigInfo>()));
            }
        }
    }
}
