using System.IO;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class AddScenes : Editor
    {
        [MenuItem("Assets/Tools/Quick Open Scene/添加当前目录场景或此场景到配置文件")]
        static void AddCurrSceneConfig()
        {
            SceneConfig sceneConfig = StaticConfig.GetSceneConfigObject();

            string[] guids = Selection.assetGUIDs;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (Directory.Exists(path))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    FileInfo[] files = directoryInfo.GetFiles("*.unity", SearchOption.AllDirectories);
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            string unityPath = file.FullName.Substring(file.FullName.IndexOf("Assets")).Replace(@"\", "/");
                            AddScene(sceneConfig, unityPath, SceneConfigInfo.SceneConfigInfoType.scenePath);
                        }
                    }
                }
                else
                {
                    AddScene(sceneConfig, path, SceneConfigInfo.SceneConfigInfoType.scenePath);
                }
            }
        }
        [MenuItem("Assets/Tools/Quick Open Scene/添加所有场景到配置文件")]
        static void AddAllSceneConfig()
        {
            SceneConfig sceneConfig = StaticConfig.GetSceneConfigObject();
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            foreach (var guid in guids)
            {
                AddScene(sceneConfig, guid, SceneConfigInfo.SceneConfigInfoType.sceneGUID);
            }
        }

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

        public static void RemoveScene(SceneConfig sceneConfig, SceneConfigInfo sceneConfigInfo)
        {
            Debug.Log("删除 " + sceneConfigInfo.SceneName + " 场景成功！");
            sceneConfig.sceneInfos.Remove(sceneConfigInfo);
        }
    }
}
