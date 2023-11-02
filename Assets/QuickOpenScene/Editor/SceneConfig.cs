using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    [Serializable]
    public class SceneConfig
    {
        [SerializeField]
        List<GroupConfigInfo> m_groupConfigs;

        public List<GroupConfigInfo> groupConfigs
        {
            get
            {
                if (m_groupConfigs == null)
                {
                    m_groupConfigs = new List<GroupConfigInfo>();
                }
                return m_groupConfigs;
            }
            set
            {
                m_groupConfigs = value;
            }
        }
    }

    // 场景组配置信息
    [Serializable]
    public class GroupConfigInfo
    {
        [SerializeField]
        string m_groupName;
        [SerializeField]
        List<SceneConfigInfo> m_sceneInfos;

        public string groupName { get => m_groupName; set => m_groupName = value; }
        public List<SceneConfigInfo> sceneInfos
        {
            get
            {
                if (m_sceneInfos == null)
                {
                    m_sceneInfos = new List<SceneConfigInfo>();
                }
                return m_sceneInfos;
            }
            set => m_sceneInfos = value;
        }



        public GroupConfigInfo(string groupName, List<SceneConfigInfo> sceneInfos)
        {
            m_groupName = groupName;
            m_sceneInfos = sceneInfos;
        }
    }



    // 场景配置信息
    [Serializable]
    public class SceneConfigInfo : IComparable<SceneConfigInfo>
    {
        [SerializeField]
        string m_sceneName; // 场景名称
        [SerializeField]
        string m_scenePath; // 场景路径
        [SerializeField]
        string m_sceneGUID; // 场景GUID

        public string sceneName { get => m_sceneName; set => m_sceneName = value; }
        public string scenePath { get => m_scenePath; set => m_scenePath = value; }
        public string sceneGUID { get => m_sceneGUID; set => m_sceneGUID = value; }


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
        public bool Refresh()
        {
            string fullPath = Path.Combine(Application.dataPath, scenePath);
            if (File.Exists(fullPath))
            {
                if (sceneGUID != AssetDatabase.AssetPathToGUID(scenePath))
                {
                    sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
                    return true;
                }

            }
            else
            {
                string newPath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                string extension = Path.GetExtension(newPath).ToLower();
                if (extension == ".unity")
                {
                    if (scenePath != newPath || sceneName != Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(newPath)))
                    {
                        scenePath = newPath;
                        sceneName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(newPath));
                        return true;
                    }

                }
            }
            return false;
        }

        // 比较场景信息，用于排序
        public int CompareTo(SceneConfigInfo other)
        {
            return sceneName.CompareTo(other.sceneName);
        }
    }
}
