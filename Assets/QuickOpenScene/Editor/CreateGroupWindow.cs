using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    internal class CreateGroupWindow : EditorWindow
    {
        string groupName = "Default";
        int commandIndex = 0;
        bool useBindPath = false;
        string path = string.Empty;
        int refreshType = 0;
        string[] refreshTypeText = new string[] { "增加", "同步" };

        int CommandIndex
        {
            get => commandIndex;
            set
            {
                if (commandIndex != value)
                {
                    switch (value)
                    {
                        //创建
                        case 1:
                            break;
                        //修改
                        case 2:
                            var currGroup = SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex];
                            groupName = currGroup.groupName;
                            useBindPath = currGroup.UseBindPath;
                            path = currGroup.Path;
                            refreshType = (int)currGroup.RefreshType;
                            break;
                        default:
                            break;
                    }
                    commandIndex = value;
                }
            }
        }

        void OnEnable()
        {
            int tempIndex;
            if (Config.GroupIndexPanel == 0)
            {
                tempIndex = 0;
            }
            else
            {
                tempIndex = Config.GroupIndexPanel - 1;
            }

            groupName = SceneConfigData.sceneConfig.groupConfigs[tempIndex].groupName;
            groupName = NameAdd(groupName);
            bool nameRepeat = false;
            do
            {
                nameRepeat = false;
                for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
                {
                    if (SceneConfigData.sceneConfig.groupConfigs[i].groupName == groupName)
                    {
                        nameRepeat = true;
                        groupName = NameAdd(groupName);
                        break;
                    }
                }

            } while (nameRepeat);
        }

        public void OnGUI()
        {
            if (Event.current.commandName == "Create")
            {
                CommandIndex = 1;
            }
            else if (Event.current.commandName == "Rename")
            {
                CommandIndex = 2;
            }
            EditorGUILayout.LabelField("分组名：");
            groupName = EditorGUILayout.TextField(groupName);
            useBindPath = EditorGUILayout.Toggle("绑定路径（更新目录场景）：", useBindPath);
            if (useBindPath)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("同步模式：", GUILayout.Width(70));
                refreshType = GUILayout.SelectionGrid(refreshType, refreshTypeText, 2);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("路径：", GUILayout.Width(150));
                if (GUILayout.Button("拾取路径", GUILayout.ExpandWidth(true)))
                {
                    var guids = Selection.assetGUIDs;
                    if (guids.Length > 0)
                    {
                        string folderPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        if (Directory.Exists(folderPath))
                        {
                            path = folderPath;
                        }
                        else
                        {
                            string log = "请选择文件夹再拾取";
                            Debug.LogWarning(log);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                path = EditorGUILayout.TextField(path);
            }

            if (GUILayout.Button("确认"))
            {
                if (useBindPath)
                {
                    if (!Directory.Exists(path))
                    {
                        string log = "路径不存在";
                        Debug.LogWarning(log);
                    }
                }

                for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
                {
                    //当为修改模式时，重名检测不检测自己
                    if (CommandIndex == 2 && i == Config.CurrGroupIndex)
                    {
                        break;
                    }

                    //重名检测所有
                    if (SceneConfigData.sceneConfig.groupConfigs[i].groupName == groupName)
                    {
                        if (EditorUtility.DisplayDialog("命名警告", $"当前分组命名重复，点击确认命名将为：{NameAdd(groupName)},点击取消重新输入命名。", "确认", "取消"))
                        {
                            NameAdd(groupName);
                        }
                        break;
                    }
                }

                switch (CommandIndex)
                {
                    //创建分组
                    case 1:

                        SceneConfigData.sceneConfig.groupConfigs.Add(new GroupConfigInfo(groupName, useBindPath, (GroupConfigInfo.Type)refreshType, path, new List<SceneConfigInfo>()));
                        //跳转到新建的组
                        Config.GroupIndexPanel = SceneConfigData.sceneConfig.groupConfigs.Count;
                        SceneConfigManage.RefreshCurrSceneConfigForPath(Config.CurrGroupIndex);
                        break;
                    //重命名分组
                    case 2:
                        SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].groupName = groupName;
                        SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].UseBindPath = useBindPath;
                        SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].Path = path;
                        SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].RefreshType = (GroupConfigInfo.Type)refreshType;
                        Config.GroupStr[Config.GroupIndexPanel] = groupName;
                        SceneConfigManage.RefreshCurrSceneConfigForPath(Config.CurrGroupIndex);
                        break;
                    default:
                        break;
                }
                SceneConfigData.instance.SaveDate();
                Close();
            }
        }

        static void SplitTailInt(string str, out string strout, out int number, out int numCount)
        {
            string numstr = string.Empty;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] <= '9' & str[i] >= '0')
                {
                    numstr += str[i];
                }
                else
                {
                    break;
                }
            }
            char[] tempNumStr = numstr.ToCharArray();
            Array.Reverse(tempNumStr);
            number = int.Parse(new string(tempNumStr));
            strout = str.Remove(str.Length - tempNumStr.Length);
            numCount = tempNumStr.Length;
        }

        public static string NameAdd(string str)
        {
            if (int.TryParse(str.Last().ToString(), out _))
            {
                string name;
                int num, numCount;
                SplitTailInt(str, out name, out num, out numCount);
                return name + (num + 1).ToString("D" + numCount.ToString());
            }
            else
            {
                return str + 1;
            }

        }
    }
}
