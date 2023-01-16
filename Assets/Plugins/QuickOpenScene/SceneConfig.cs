using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    [CreateAssetMenu(menuName = "Quick Open Scene/配置文件")]
    public class SceneConfig : ScriptableObject
    {
        public List<SceneConfigInfo> sceneInfos = new List<SceneConfigInfo>();

        private void OnValidate()
        {
            foreach (var scene in sceneInfos)
            {
                try
                {
                    scene.SceneName = scene.Scene.name;
                    scene.ScenePath = AssetDatabase.GetAssetPath(scene.Scene);
                    scene.SceneGUID = AssetDatabase.AssetPathToGUID(scene.ScenePath);
                }
                catch { }
            }
        }
    }

    [Serializable]
    public class SceneConfigInfo
    {
        [SerializeField]
        SceneAsset scene;
        string sceneName;
        [SerializeField]
        string scenePath;
        [SerializeField]
        string sceneGUID;

        public enum SceneConfigInfoType
        {
            scenePath, sceneGUID
        }

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
            scenePath = AssetDatabase.GUIDToAssetPath(SceneGUID);
            scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            sceneName = scene.name;
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