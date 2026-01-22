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
        [SerializeField] List<GroupConfigInfo> m_groupConfigs;

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
            set { m_groupConfigs = value; }
        }
    }

    // 场景组配置信息
    [Serializable]
    public class GroupConfigInfo
    {
        public enum Type
        {
            Add,
            Sync
        }

        [SerializeField] string m_groupName;
        [SerializeField] bool m_UseBindPath;
        [SerializeField] Type m_RefreshType;
        [SerializeField] string m_Path;
        [SerializeField] List<SceneConfigInfo> m_sceneInfos;

        #region 属性

        /// <summary>
        /// 分组命名
        /// </summary>
        public string groupName
        {
            get => m_groupName;
            set => m_groupName = value;
        }

        /// <summary>
        /// 是否使用同步
        /// </summary>
        public bool UseBindPath
        {
            get => m_UseBindPath;
            set => m_UseBindPath = value;
        }

        /// <summary>
        /// 同步路径
        /// </summary>
        public string Path
        {
            get => m_Path;
            set => m_Path = value;
        }

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

        /// <summary>
        /// 0为增加，1为同步
        /// </summary>
        public Type RefreshType
        {
            get => m_RefreshType;
            set => m_RefreshType = value;
        }

        #endregion

        #region 构造函数

        public GroupConfigInfo(string groupName, List<SceneConfigInfo> sceneInfos)
        {
            m_groupName = groupName;
            m_UseBindPath = false;
            m_Path = string.Empty;
            m_sceneInfos = sceneInfos;
        }

        public GroupConfigInfo(string groupName, bool useBindPath, Type refreshType, string path,
            List<SceneConfigInfo> sceneInfos)
        {
            m_groupName = groupName;
            m_UseBindPath = useBindPath;
            m_RefreshType = refreshType;
            m_Path = path;
            m_sceneInfos = sceneInfos;
        }

        #endregion
    }


    // 场景配置信息
    [Serializable]
    public class SceneConfigInfo : IComparable<SceneConfigInfo>
    {
        [SerializeField] string m_sceneName; // 场景名称
        [SerializeField] string m_scenePath; // 场景路径
        [SerializeField] string m_sceneGUID; // 场景GUID

        public string sceneName
        {
            get => m_sceneName;
            set => m_sceneName = value;
        }

        public string scenePath
        {
            get => m_scenePath;
            set => m_scenePath = value;
        }

        public string sceneGUID
        {
            get => m_sceneGUID;
            set => m_sceneGUID = value;
        }


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
                    if (scenePath != newPath || sceneName !=
                        Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(newPath)))
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