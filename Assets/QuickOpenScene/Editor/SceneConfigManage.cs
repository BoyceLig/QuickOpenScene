using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [MenuItem(Config.MenuPath.addGroupFromScene)]
        static void AddGroupFromScene()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids == null || guids.Length == 0)
            {
                return;
            }

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                string name = Path.GetFileNameWithoutExtension(path);
                if (EditorUtility.DisplayCancelableProgressBar("通过路径添加分组", "添加分组 " + name, i / guids.Length - 1))
                {
                    return;
                }

                if (Directory.Exists(path))
                {
                    for (int j = 0; j < SceneConfigData.sceneConfig.groupConfigs.Count; j++)
                    {
                        //检查所选目录是否已经有了当前分组
                        if (SceneConfigData.sceneConfig.groupConfigs[j].groupName == name)
                        {
                            switch (EditorUtility.DisplayDialogComplex("重名", "当前目录分组命名重复，覆盖还是新建？", "覆盖", "跳过", "新建"))
                            {
                                //覆盖
                                case 0:
                                    SceneConfigData.sceneConfig.groupConfigs[j].UseBindPath = true;
                                    SceneConfigData.sceneConfig.groupConfigs[j].Path = path;
                                    SceneConfigData.sceneConfig.groupConfigs[j].RefreshType = GroupConfigInfo.Type.Sync;
                                    Config.GroupIndexPanel = j + 1;
                                    RefreshCurrSceneConfigForPath(j);
                                    break;
                                //跳过
                                case 1:
                                    Config.GroupIndexPanel = j + 1;
                                    break;
                                //新建
                                case 2:
                                    SceneConfigData.sceneConfig.groupConfigs.Add(
                                        new GroupConfigInfo(CreateGroupWindow.NameAdd(name), true,
                                            GroupConfigInfo.Type.Sync, path, new List<SceneConfigInfo>()));
                                    Config.GroupIndexPanel = SceneConfigData.sceneConfig.groupConfigs.Count;
                                    RefreshCurrSceneConfigForPath(SceneConfigData.sceneConfig.groupConfigs.Count - 1);
                                    break;
                            }

                            break;
                        }
                    }

                    //新建
                    SceneConfigData.sceneConfig.groupConfigs.Add(new GroupConfigInfo(name, true,
                        GroupConfigInfo.Type.Sync, path, new List<SceneConfigInfo>()));
                    Config.GroupIndexPanel = SceneConfigData.sceneConfig.groupConfigs.Count;
                    RefreshCurrSceneConfigForPath(SceneConfigData.sceneConfig.groupConfigs.Count - 1);
                }
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("通过路径创建完成");
        }

        /// <summary>
        /// 根据当前path刷新路径的场景,自动判断刷新类型
        /// </summary>
        /// <param name="currGroupIndex"></param>
        public static void RefreshCurrSceneConfigForPath(int currGroupIndex)
        {
            GroupConfigInfo currGroupConfig = SceneConfigData.sceneConfig.groupConfigs[currGroupIndex];
            string path = currGroupConfig.Path;

            //增加路径判断，防止路径变更后路径不匹配
            if (path == string.Empty)
            {
                Debug.LogError("路径为空");
                return;
            }
            else if (!Directory.Exists(path))
            {
                Debug.LogError("路径不存在，请重新设置");
                return;
            }

            //查找路径内的所有场景文件
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new string[] { path });

            if (sceneGuids == null || sceneGuids.Length == 0)
            {
                return;
            }

            if (currGroupConfig.RefreshType == GroupConfigInfo.Type.Sync)
            {
                currGroupConfig.sceneInfos.Clear();
            }

            foreach (var sceneGuid in sceneGuids)
            {
                AddScene(currGroupIndex, sceneGuid, SceneConfigInfoType.sceneGUID);
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
            if (SceneConfigData.sceneConfig.groupConfigs.Count <= groupIndex || string.IsNullOrEmpty(info)) return;

            var path = sceneConfigInfoType == SceneConfigInfoType.sceneGUID
                ? AssetDatabase.GUIDToAssetPath(info)
                : info;
            if (File.Exists(path) && Path.GetExtension(path).Contains(".unity") && path.StartsWith("Assets"))
            {
                var temp = new SceneConfigInfo(path, SceneConfigInfoType.scenePath);
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
            if (SceneConfigData.sceneConfig.groupConfigs.Count <= groupIndex || sceneConfigInfo == null ||
                string.IsNullOrEmpty(sceneConfigInfo.scenePath) || string.IsNullOrEmpty(sceneConfigInfo.sceneGUID) ||
                string.IsNullOrEmpty(sceneConfigInfo.sceneName))
            {
                return;
            }

            if (SceneConfigData.sceneConfig.groupConfigs[groupIndex].sceneInfos.Count > 0)
            {
                if (SceneConfigData.sceneConfig.groupConfigs[groupIndex].sceneInfos
                    .Any(item => item.sceneGUID == sceneConfigInfo.sceneGUID)) return;
            }

            SceneConfigData.sceneConfig.groupConfigs[groupIndex].sceneInfos.Add(sceneConfigInfo);
            QOSWindow.RefreshGetSceneConfigInfos();

            SceneConfigData.instance.SaveDate();
            Debug.Log(
                "添加 " + sceneConfigInfo.sceneName + " 场景到分组" +
                SceneConfigData.sceneConfig.groupConfigs[groupIndex].groupName + " 成功！",
                AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneConfigInfo.scenePath));
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
                        Debug.Log("删除 " + SceneConfigData.sceneConfig.groupConfigs[i].groupName + " 分组内的 " +
                                  sceneConfigInfo.sceneName + " 场景成功！");
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
                    if (tempPath.Contains("QuickOpenScene/Editor/GithubJsonData.cs") ||
                        tempPath.Contains("com.boycelig.quickopenscene/Editor/GithubJsonData.cs"))
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
                SceneConfigData.sceneConfig.groupConfigs.Add(
                    new GroupConfigInfo("Default", new List<SceneConfigInfo>()));
            }
        }
    }
}