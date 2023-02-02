using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    internal class CreateGroupWindow : EditorWindow
    {
        string tempStr = "Default";
        int commandIndex = 0;

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

            tempStr = Config.SceneConfigData.groupConfigs[tempIndex].groupName;
            tempStr = NameAdd(tempStr);
            bool nameRepeat = false;
            do
            {
                nameRepeat = false;
                for (int i = 0; i < Config.SceneConfigData.groupConfigs.Count; i++)
                {
                    if (Config.SceneConfigData.groupConfigs[i].groupName == tempStr)
                    {
                        nameRepeat = true;
                        tempStr = NameAdd(tempStr);
                        break;
                    }
                }

            } while (nameRepeat);            
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
                        if (EditorUtility.DisplayDialog("命名警告", $"当前分组命名重复，点击确认命名将为：{NameAdd(tempStr)},点击取消重新输入命名。", "确认", "取消"))
                        {
                            NameAdd(tempStr);
                        }
                        break;
                    }
                }

                switch (commandIndex)
                {
                    //创建分组
                    case 0:
                        Config.SceneConfigData.groupConfigs.Add(new GroupConfigInfo(tempStr, new List<SceneConfigInfo>()));
                        //跳转到新建的组
                        Config.GroupIndexPanel = Config.SceneConfigData.groupConfigs.Count;
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

        void SplitTailInt(string str, out string strout, out int number, out int numCount)
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

        string NameAdd(string str)
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
