using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    [FilePath(Config.sceneConfigDatePath, FilePathAttribute.Location.ProjectFolder)]
    public class SceneConfigData : ScriptableSingleton<SceneConfigData>
    {
        [SerializeField]
        SceneConfig m_sceneConfig;

        public static SceneConfig sceneConfig
        {
            get
            {
                if (instance.m_sceneConfig.groupConfigs.Count == 0)
                {
                    instance.m_sceneConfig.groupConfigs.Add(new GroupConfigInfo("Default", new List<SceneConfigInfo>()));
                    instance.SaveDate();
                }
                return instance.m_sceneConfig;
            }
            set
            {
                instance.m_sceneConfig = value;
                instance.SaveDate();
            }
        }

        public void SaveDate()
        {
            Save(true);
        }
    }
}
