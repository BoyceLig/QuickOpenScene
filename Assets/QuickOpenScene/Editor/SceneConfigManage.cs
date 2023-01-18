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
            //获取配置文件
            SceneConfig sceneConfig = GetSceneConfigObject();
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
                        AddScene(sceneConfig, path, SceneConfigInfo.SceneConfigInfoType.scenePath);
                    }
                }
            }

            //查找路径内的所有场景文件
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", direPaths.ToArray());

            foreach (var sceneGuid in sceneGuids)
            {
                AddScene(sceneConfig, sceneGuid, SceneConfigInfo.SceneConfigInfoType.sceneGUID);
            }
        }

        [MenuItem(Config.MenuPath.addAllScene)]
        static void AddAllSceneConfig()
        {
            SceneConfig sceneConfig = GetSceneConfigObject();
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            foreach (var guid in guids)
            {
                AddScene(sceneConfig, guid, SceneConfigInfo.SceneConfigInfoType.sceneGUID);
            }
        }

        /// <summary>
        /// 添加场景到配置文件
        /// </summary>
        /// <param name="sceneConfig">配置文件</param>
        /// <param name="info">路径或者guid</param>
        /// <param name="sceneConfigInfoType">info 的类型</param>
        public static void AddScene(SceneConfig sceneConfig, string info, SceneConfigInfo.SceneConfigInfoType sceneConfigInfoType)
        {
            string path;
            if (sceneConfigInfoType == SceneConfigInfo.SceneConfigInfoType.sceneGUID)
            {
                path = AssetDatabase.GUIDToAssetPath(info);
            }
            else
            {
                path = info;
            }
            if (File.Exists(path) && Path.GetExtension(path).Contains(".unity") && path.StartsWith("Assets"))
            {
                bool exist = false;
                if (sceneConfig.sceneInfos.Count > 0)
                {
                    foreach (var item in sceneConfig.sceneInfos)
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
                    SceneConfigInfo temp = new SceneConfigInfo(path, SceneConfigInfo.SceneConfigInfoType.scenePath);
                    sceneConfig.sceneInfos.Add(temp);
                    Debug.Log("添加 " + temp.SceneName + " 场景成功！", temp.Scene);
                }
            }
        }

        /// <summary>
        /// 在配置文件内删除场景配置信息
        /// </summary>
        /// <param name="sceneConfig">配置文件</param>
        /// <param name="sceneConfigInfo">场景的配置信息</param>
        public static void RemoveScene(SceneConfig sceneConfig, SceneConfigInfo sceneConfigInfo)
        {
            Debug.Log("删除 " + sceneConfigInfo.SceneName + " 场景成功！");
            sceneConfig.sceneInfos.Remove(sceneConfigInfo);
        }

        public static SceneConfig GetSceneConfigObject()
        {
            string guid = AssetDatabase.FindAssets("QuickOpenSceneConfigData")[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SceneConfig sceneConfig = AssetDatabase.LoadAssetAtPath<SceneConfig>(path);
            return sceneConfig;
        }
    }
}
