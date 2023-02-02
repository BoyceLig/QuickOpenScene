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
                        AddScene(Config.GroupIndexPanel > 0 ? Config.GroupIndexPanel - 1 : 0, path, SceneConfigInfoType.scenePath);
                    }
                }
            }

            //查找路径内的所有场景文件
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", direPaths.ToArray());

            foreach (var sceneGuid in sceneGuids)
            {
                AddScene(Config.GroupIndexPanel > 0 ? Config.GroupIndexPanel - 1 : 0, sceneGuid, SceneConfigInfoType.sceneGUID);
            }
        }

        [MenuItem(Config.MenuPath.addAllScene)]
        static void AddAllSceneConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            foreach (var guid in guids)
            {
                AddScene(Config.GroupIndexPanel > 0 ? Config.GroupIndexPanel - 1 : 0, guid, SceneConfigInfoType.sceneGUID);
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
                //查重
                bool exist = false;
                CheckSceneConfig();
                if (Config.SceneConfigData.groupConfigs[groupIndex].sceneInfos.Count > 0)
                {
                    foreach (var item in Config.SceneConfigData.groupConfigs[groupIndex].sceneInfos)
                    {
                        if (item.ScenePath == path)
                        {
                            exist = true;
                            break;
                        }
                    }
                }
                if (!exist)
                {
                    SceneConfigInfo temp = new SceneConfigInfo(path, SceneConfigInfoType.scenePath);
                    Config.SceneConfigData.groupConfigs[Config.GroupIndexPanel > 0 ? Config.GroupIndexPanel - 1 : 0].sceneInfos.Add(temp);
                    EditorUtility.SetDirty(Config.SceneConfigData);
                    Debug.Log("添加 " + temp.SceneName + " 场景到分组" + Config.SceneConfigData.groupConfigs[Config.GroupIndexPanel > 0 ? Config.GroupIndexPanel - 1 : 0].groupName + " 成功！", temp.Scene);
                }
            }
        }

        /// <summary>
        /// 在配置文件内删除对应分组内的场景配置信息
        /// </summary>
        /// <param name="groupIndex">分组数(不是面板的分组数)</param>
        /// <param name="sceneConfigInfo">场景的配置信息</param>
        public static void RemoveSceneInfo(int groupIndex, SceneConfigInfo sceneConfigInfo)
        {
            Debug.Log("删除 " + sceneConfigInfo.SceneName + " 场景成功！");
            Config.SceneConfigData.groupConfigs[groupIndex].sceneInfos.Remove(sceneConfigInfo);
            EditorUtility.SetDirty(Config.SceneConfigData);
        }

        /// <summary>
        /// 删除所有分组内的当前场景配置信息
        /// </summary>
        /// <param name="sceneConfigInfo">场景的配置信息</param>
        public static void RemoveSceneInfo(SceneConfigInfo sceneConfigInfo)
        {
            for (int i = 0; i < Config.SceneConfigData.groupConfigs.Count; i++)
            {
                for (int j = 0; j < Config.SceneConfigData.groupConfigs[i].sceneInfos.Count; j++)
                {
                    if (Config.SceneConfigData.groupConfigs[i].sceneInfos[j] == sceneConfigInfo)
                    {
                        Debug.Log("删除 " + Config.SceneConfigData.groupConfigs[i].groupName + " 分组内的 " + sceneConfigInfo.SceneName + " 场景成功！");
                        Config.SceneConfigData.groupConfigs[i].sceneInfos.Remove(sceneConfigInfo);
                        EditorUtility.SetDirty(Config.SceneConfigData);
                    }
                }
            }
        }

        /// <summary>
        /// 获取升级日志的日志信息
        /// </summary>
        /// <returns></returns>
        public static string GetLogText()
        {
            string path = Config.PluginPath + "/ChangeLog.txt";
            TextAsset logText = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            return logText.text;
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
                    if (AssetDatabase.GUIDToAssetPath(guid).Contains("QuickOpenScene/Editor/GithubJsonData.cs"))
                    {
                        string githubJsonDataPath = AssetDatabase.GUIDToAssetPath(guid);
                        return githubJsonDataPath.Remove(githubJsonDataPath.IndexOf("/Editor/GithubJsonData.cs"));
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 创建QuickOpenSceneConfigData文件
        /// </summary>
        /// <returns>SceneConfig</returns>
        public static SceneConfig CreateSceneConfig()
        {
            string dataFolderPath = Config.PluginPath + "/Data";
            string sceneConfigPath = dataFolderPath + "/QuickOpenSceneConfigData.asset";
            if (!Directory.Exists(dataFolderPath))
            {
                AssetDatabase.CreateFolder(Config.PluginPath, "Data");
            }
            SceneConfig sceneConfig = CreateInstance<SceneConfig>();
            AssetDatabase.CreateAsset(sceneConfig, sceneConfigPath);
            return sceneConfig;
        }

        /// <summary>
        /// 检查SceneConfig分组
        /// </summary>
        public static void CheckSceneConfig()
        {
            if (Config.SceneConfigData.groupConfigs.Count == 0)
            {
                Config.SceneConfigData.groupConfigs.Add(new GroupConfigInfo("Default", new List<SceneConfigInfo>()));
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveSceneConfigData()
        {
            AssetDatabase.SaveAssetIfDirty(Config.SceneConfigData);
        }

    }
}
