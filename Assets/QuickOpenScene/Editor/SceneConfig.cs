using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    //[CreateAssetMenu(menuName = "Quick Open Scene/创建配置文件")]
    public class SceneConfig : ScriptableObject
    {
        public List<GroupConfigInfo> groupConfigs = new List<GroupConfigInfo>();
    }

    // 场景组配置信息
    [Serializable]
    public class GroupConfigInfo
    {
        public string groupName;
        public List<SceneConfigInfo> sceneInfos = new List<SceneConfigInfo>();

        public GroupConfigInfo(string groupName, List<SceneConfigInfo> sceneInfos)
        {
            this.groupName = groupName;
            this.sceneInfos = sceneInfos;
        }
    }

    // 场景配置信息
    [Serializable]
    public class SceneConfigInfo : IComparable<SceneConfigInfo>
    {
        public string sceneName; // 场景名称
        public string scenePath; // 场景路径
        public string sceneGUID; // 场景GUID

        // 构造函数
        public SceneConfigInfo(string sceneInfo, SceneConfigInfoType sceneConfigInfoType)
        {
            switch (sceneConfigInfoType)
            {
                case SceneConfigInfoType.scenePath:
                    scenePath = sceneInfo;
                    sceneGUID = AssetDatabase.AssetPathToGUID(sceneInfo);
                    sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    break;
                case SceneConfigInfoType.sceneGUID:
                    sceneGUID = sceneInfo;
                    scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                    sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    break;
                default:
                    break;
            }
        }

        // 构造函数
        public SceneConfigInfo(SceneAsset scene)
        {
            scenePath = AssetDatabase.GetAssetPath(scene);
            sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
            sceneName = scene.name;
        }

        // 刷新场景信息
        public void Refresh()
        {
            string fullPath = Path.Combine(Application.dataPath, scenePath);
            if (File.Exists(fullPath))
            {
                sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
            }
            else
            {
                string newPath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                string extension = Path.GetExtension(newPath).ToLower();
                if (extension == ".unity")
                {
                    scenePath = newPath;
                    sceneName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(newPath));
                }
            }
            SceneConfigManage.SaveSceneConfigJS();
        }

        // 比较场景信息，用于排序
        public int CompareTo(SceneConfigInfo other)
        {
            return sceneName.CompareTo(other.sceneName);
        }
    }
}
