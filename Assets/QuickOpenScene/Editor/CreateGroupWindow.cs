using QuickOpenScene;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal class CreateGroupWindow : EditorWindow
{
    string tempStr = "Default1";
    int commandIndex = 0;

    void OnEnable()
    {
        for (int i = 0; i < Config.SceneConfigData.groupConfigs.Count; i++)
        {
            if (Config.SceneConfigData.groupConfigs[i].groupName == tempStr)
            {
                tempStr += 1;
            }
        }
    }

    public void OnGUI()
    {
        if (Event.current.commandName == "Create")
        {
            commandIndex = 0;
        }
        else if (Event.current.commandName == "Rename")
        {
            commandIndex = 1;
        }

        GUILayout.Label("请输入新的分组命名：");
        tempStr = GUILayout.TextField(tempStr);
        if (GUILayout.Button("确认"))
        {
            for (int i = 0; i < Config.SceneConfigData.groupConfigs.Count; i++)
            {
                if (Config.SceneConfigData.groupConfigs[i].groupName == tempStr)
                {
                    if (EditorUtility.DisplayDialog("命名警告", $"当前分组命名重复，点击确认命名将为：{tempStr + 1},点击取消重新输入命名。", "确认", "取消"))
                    {
                        tempStr += 1;
                    }
                    break;
                }
            }

            switch (commandIndex)
            {
                //创建分组
                case 0:
                    Config.SceneConfigData.groupConfigs.Add(new GroupConfigInfo(tempStr, new List<SceneConfigInfo>()));
                    break;
                //重命名分组
                case 1:
                    Config.SceneConfigData.groupConfigs[Config.GroupIndexPanel - 1].groupName = tempStr;
                    Config.GroupStr[Config.GroupIndexPanel] = tempStr;
                    break;
                default:
                    break;
            }
            EditorUtility.SetDirty(Config.SceneConfigData);
            Close();
        }
    }

}
