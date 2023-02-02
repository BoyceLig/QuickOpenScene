using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    //[CreateAssetMenu(menuName = "Quick Open Scene/创建配置文件")]
    public class SceneConfig : ScriptableObject
    {
        public List<GroupConfigInfo> groupConfigs = new List<GroupConfigInfo>();

        void OnValidate()
        {
            for (int i = 0; i < groupConfigs.Count - 1; i++)
            {
                if (groupConfigs.Last().groupName == groupConfigs[i].groupName)
                {
                    groupConfigs.Last().groupName += 1;
                }
            }

            foreach (var group in groupConfigs)
            {
                foreach (var scene in group.sceneInfos)
                {
                    scene.Refresh();
                }
            }
            EditorUtility.SetDirty(Config.SceneConfigData);
        }
    }
    [Serializable]
    public class GroupConfigInfo : IComparable<GroupConfigInfo>
    {
        public string groupName;
        public List<SceneConfigInfo> sceneInfos = new List<SceneConfigInfo>();
        public GroupConfigInfo(string groupName, List<SceneConfigInfo> sceneInfos)
        {
            this.groupName = groupName;
            this.sceneInfos = sceneInfos;
        }

        //排序
        public int CompareTo(GroupConfigInfo other)
        {
            int a = groupName.CompareTo(other.groupName);
            return a;
        }
    }

    [Serializable]
    public class SceneConfigInfo : IComparable<SceneConfigInfo>
    {
        [SerializeField]
        SceneAsset scene;
        string sceneName;
        [SerializeField]
        string scenePath;
        [SerializeField]
        string sceneGUID;

        public SceneConfigInfo(string sceneInfo, SceneConfigInfoType sceneConfigInfoType)
        {
            switch (sceneConfigInfoType)
            {
                case SceneConfigInfoType.scenePath:
                    ScenePath = sceneInfo;
                    break;
                case SceneConfigInfoType.sceneGUID:
                    SceneGUID = sceneInfo;
                    break;
                default:
                    break;
            }
        }

        public SceneConfigInfo(SceneAsset scene)
        {
            Scene = scene;
        }

        public void Refresh()
        {
            if (scene != null)
            {
                scenePath = AssetDatabase.GetAssetPath(scene);
                sceneName = scene.name;
                sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
            }
            else if (sceneGUID != string.Empty)
            {
                scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (scene != null)
                {
                    sceneName = scene.name;
                }
            }
            else if (ScenePath != string.Empty)
            {
                scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (scene != null)
                {
                    sceneName = scene.name;
                    sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
                }
            }
        }

        //排序
        public int CompareTo(SceneConfigInfo other)
        {
            int a = SceneName.CompareTo(other.SceneName);
            return a;
        }

        public string SceneName
        {
            get { return sceneName; }
            set { sceneName = value; }
        }
        public SceneAsset Scene
        {
            get
            {
                return scene;
            }
            set
            {
                scene = value;
                sceneName = scene.name;
                scenePath = AssetDatabase.GetAssetPath(value);
                sceneGUID = AssetDatabase.AssetPathToGUID(scenePath);
            }
        }

        public string ScenePath
        {
            get
            {
                return scenePath;
            }
            set
            {
                scenePath = value;
                scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(value);
                sceneName = scene.name;
                sceneGUID = AssetDatabase.AssetPathToGUID(value);
            }
        }

        public string SceneGUID
        {
            get
            {
                return sceneGUID;
            }
            set
            {
                sceneGUID = value;
                scenePath = AssetDatabase.GUIDToAssetPath(value);
                scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }
        }
    }
}