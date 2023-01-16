using System.IO;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    public class AddScenes : Editor
    {
        [MenuItem("Assets/Tools/Quick Open Scene/搜索当前目录场景添加到SceneConfig")]
        static void SceneConfig()
        {
            SceneConfig sceneConfig = AssetDatabase.LoadAssetAtPath<SceneConfig>(StaticConfig.sceneConfigInfoPath);

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
                            SceneConfig temsceneConfig = CreateInstance<SceneConfig>();
                            SceneConfigInfo info = new SceneConfigInfo(unityPath, SceneConfigInfo.SceneConfigInfoType.scenePath);
                            bool exist = false;
                            if (sceneConfig.sceneInfos != null)
                            {
                                foreach (var item in sceneConfig.sceneInfos)
                                {
                                    if (item.ScenePath == info.ScenePath)
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                            }
                            if (!exist)
                            {
                                sceneConfig.sceneInfos.Add(info);
                                Debug.Log(info.SceneName + "场景已添加到SceneConfig");
                            }

                        }
                    }
                }
                else if (File.Exists(path) && path.ToLower().Contains(".unity"))
                {
                    SceneConfigInfo info = new SceneConfigInfo(path, SceneConfigInfo.SceneConfigInfoType.scenePath);
                    sceneConfig.sceneInfos.Add(info);
                    Debug.Log(info.SceneName + "场景已添加到SceneConfig");
                }
            }
        }
    }
}
