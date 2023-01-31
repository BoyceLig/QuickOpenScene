using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickOpenScene
{
    public class QOSWindow : EditorWindow
    {
        string[] sortbys = new string[] { "默认排序", "命名排序" };

        Vector2 scrollViewPos;
        GUIStyle versionStyle, buttonStyle, rightLableStyle;
        string search = string.Empty;

        [MenuItem(Config.MenuPath.quickOpenSceneWindow)]
        static void Open()
        {
            QOSWindow mainWindow = GetWindow<QOSWindow>();
            mainWindow.titleContent = new GUIContent("快速打开场景");
            mainWindow.minSize = new Vector2(260, 200);
            mainWindow.Show();
            SceneConfigManage.CheckSceneConfig();
        }

        private void OnEnable()
        {
            //Debug.Log(Config.SceneConfigData.name);
            //Debug.Log(Config.SceneConfigData.groupConfigs);
            //Debug.Log(Config.SceneConfigData.groupConfigs.Count);
            //SceneConfigManage.CheckSceneConfig();
        }
        
        

        private void OnGUI()
        {
            //GUIStyle初始化
            if (versionStyle == null)
            {
                versionStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                versionStyle.normal.textColor = Color.red;
                versionStyle.hover.textColor = Color.white;
                versionStyle.active.textColor = Color.gray;
            }

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle("Button");
                buttonStyle.alignment = TextAnchor.MiddleLeft;
            }

            if (true)
            {
                rightLableStyle = new GUIStyle(EditorStyles.label);
                rightLableStyle.border.top = 3;
            }


            //配置文件快速跳转
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("配置文件：", GUILayout.ExpandWidth(false));
            EditorGUILayout.ObjectField(Config.SceneConfigData, typeof(SceneConfig), false);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();


            //名称检索
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("名称搜索：", GUILayout.ExpandWidth(false));
            search = EditorGUILayout.TextField(search);
            if (GUILayout.Button("清空", GUILayout.ExpandWidth(false)))
            {
                search = string.Empty;
            }
            EditorGUILayout.EndHorizontal();


            //场景计数，排序方式
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("当前场景数量：" + SceneConfigInfosSort().Length, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("排序方式：", rightLableStyle, GUILayout.ExpandWidth(false));
            Config.SortbyIndex = EditorGUILayout.Popup(Config.SortbyIndex, sortbys, GUILayout.ExpandWidth(false), GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("分组：", GUILayout.ExpandWidth(false));
            Config.GroupIndexPanel = EditorGUILayout.Popup(Config.GroupIndexPanel, Config.GroupStr, GUILayout.ExpandWidth(true));
            //新建分组按钮
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus").image, "新建一个新的分组并跳转"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
            {
                CreateGroupWindow window = GetWindow<CreateGroupWindow>(true);
                window.titleContent = new GUIContent("创建分组");
                window.minSize = new Vector2(260, 80);
                window.maxSize = new Vector2(260, 80);
                window.Show();
                window.SendEvent(EditorGUIUtility.CommandEvent("Create"));
            }
            EditorGUI.BeginDisabledGroup(Config.GroupIndexPanel == 0);
            //重命名
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("editicon.sml").image, "重命名当前分组"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
            {
                CreateGroupWindow window = GetWindow<CreateGroupWindow>(true);
                window.titleContent = new GUIContent("重命名分组");
                window.minSize = new Vector2(260, 80);
                window.maxSize = new Vector2(260, 80);
                window.Show();
                window.SendEvent(EditorGUIUtility.CommandEvent("Rename"));
            }
            //删除分组
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Trash").image, "删除当前分组和当前分组内的场景数据"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
            {
                Config.SceneConfigData.groupConfigs.RemoveAt(Config.GroupIndexPanel - 1);
                SceneConfigManage.SaveSceneConfigData();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();


            if (Config.SceneConfigData.groupConfigs != null && Config.SceneConfigData.groupConfigs.Count > 0)
            {
                scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);


                for (int i = 0; i < SceneConfigInfosSort().Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    //判断当前是否有数据
                    if (SceneConfigInfosSort()[i].SceneGUID != string.Empty)
                    {
                        //名称检索(搜索框为空或者搜索名称包含)
                        if (search == string.Empty || SceneConfigInfosSort()[i].SceneName.ToLower().Contains(search.ToLower()))
                        {
                            if (GUILayout.Button(new GUIContent("  " + SceneConfigInfosSort()[i].SceneName, EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image), buttonStyle))
                            {
                                SceneConfigInfosSort()[i].Refresh();

                                //左键点击打开场景
                                if (Event.current.button == 0)
                                {
                                    //判断场景是否丢失
                                    if (SceneConfigInfosSort()[i].Scene != null)
                                    {
                                        //判断场景是否需要保存
                                        if (SceneManager.GetActiveScene().isDirty)
                                        {
                                            int b = EditorUtility.DisplayDialogComplex("打开场景", $"确定要打开场景{SceneConfigInfosSort()[i].SceneName}吗, \n请提前保存上一个场景!", "打开(保存场景)", "取消", "打开(不保存)");
                                            switch (b)
                                            {
                                                //打开(保存场景)
                                                case 0:
                                                    EditorSceneManager.SaveOpenScenes();
                                                    EditorSceneManager.OpenScene(SceneConfigInfosSort()[i].ScenePath);
                                                    break;

                                                //打开(不保存)
                                                case 2:
                                                    EditorSceneManager.OpenScene(SceneConfigInfosSort()[i].ScenePath);
                                                    break;

                                                //取消
                                                default:
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            EditorSceneManager.OpenScene(SceneConfigInfosSort()[i].ScenePath);
                                        }
                                    }
                                    else
                                    {
                                        if (EditorUtility.DisplayDialog("场景丢失", $"场景{SceneConfigInfosSort()[i].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                        {
                                            SceneConfigManage.RemoveSceneInfo(SceneConfigInfosSort()[i]);
                                        }
                                    }
                                }
                                //右键点击弹出菜单
                                else if (Event.current.button == 1)
                                {
                                    int currIndex = i;
                                    GenericMenu menu = new GenericMenu();
                                    menu.AddItem(new GUIContent("复制场景名字"), false, () =>
                                    {
                                        GUIUtility.systemCopyBuffer = SceneConfigInfosSort()[currIndex].SceneName;
                                    });
                                    menu.AddItem(new GUIContent("复制场景路径"), false, () =>
                                    {
                                        GUIUtility.systemCopyBuffer = SceneConfigInfosSort()[currIndex].ScenePath;
                                    });
                                    menu.AddItem(new GUIContent("跳转到当前场景"), false, () =>
                                    {
                                        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneConfigInfosSort()[currIndex].ScenePath);
                                        if (scene != null)
                                        {
                                            EditorUtility.FocusProjectWindow();
                                            EditorGUIUtility.PingObject(scene);
                                        }
                                        else
                                        {
                                            if (EditorUtility.DisplayDialog("场景丢失", $"场景{SceneConfigInfosSort()[currIndex].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                            {
                                                if (Config.GroupIndexPanel > 0)
                                                {
                                                    SceneConfigManage.RemoveSceneInfo(Config.GroupIndexPanel - 1, SceneConfigInfosSort()[currIndex]);
                                                }
                                                else
                                                {
                                                    SceneConfigManage.RemoveSceneInfo(SceneConfigInfosSort()[currIndex]);
                                                }
                                            }
                                        }
                                    });
                                    menu.AddItem(new GUIContent("删除当前场景"), false, () =>
                                    {
                                        if (EditorUtility.DisplayDialog("场景丢失", $"场景{SceneConfigInfosSort()[currIndex].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                        {
                                            if (Config.GroupIndexPanel > 0)
                                            {
                                                SceneConfigManage.RemoveSceneInfo(Config.GroupIndexPanel - 1, SceneConfigInfosSort()[currIndex]);
                                            }
                                            else
                                            {
                                                if (EditorUtility.DisplayDialog("删除警告", $"当前显示的为所有分组，将删除所有分组内的{SceneConfigInfosSort()[currIndex].SceneName}场景信息，是否删除？", "删除数据", "取消"))
                                                {
                                                    SceneConfigManage.RemoveSceneInfo(SceneConfigInfosSort()[currIndex]);
                                                }
                                            }
                                        }
                                    });
                                    menu.ShowAsContext();
                                }
                            }

                            //删除场景按钮
                            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(30f)) && Event.current.button == 0)
                            {
                                if (Config.GroupIndexPanel > 0)
                                {
                                    SceneConfigManage.RemoveSceneInfo(Config.GroupIndexPanel - 1, SceneConfigInfosSort()[i]);
                                }
                                else
                                {
                                    if (EditorUtility.DisplayDialog("删除警告", $"当前显示的为所有分组，将删除所有分组内的{SceneConfigInfosSort()[i].SceneName}场景信息，是否删除？", "删除数据", "取消"))
                                    {
                                        SceneConfigManage.RemoveSceneInfo(SceneConfigInfosSort()[i]);
                                    }
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    //EditorGUILayout.Separator();
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("场景为空，请添加场景!");
            }
            //如果鼠标正在拖拽中，并且为unity文件
            if (Event.current.type == EventType.DragUpdated && Path.GetExtension(DragAndDrop.paths[0]).Contains(".unity"))
            {
                //改变鼠标的外表
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    foreach (var path in DragAndDrop.paths)
                    {
                        SceneConfigManage.AddScene(Config.GroupIndexPanel > 0 ? Config.GroupIndexPanel - 1 : 0, path, SceneConfigInfoType.scenePath);
                    }
                }
            }

            GUILayout.FlexibleSpace();

            if (Config.NeedUpdate)
            {
                if (GUILayout.Button($"Version: {Config.currVersion}（需要更新）", versionStyle))
                {
                    AboutWindow.OpenAbout();
                }
                Repaint();
            }
            else
            {
                EditorGUILayout.LabelField($"Version: {Config.currVersion}（最新版）", EditorStyles.centeredGreyMiniLabel);
            }
        }


        public static SceneConfigInfo[] SceneConfigInfosSort()
        {
            SceneConfigInfo[] tempSceneConfigInfos;
            if (Config.GroupIndexPanel == 0)
            {
                int sceneCount = 0;
                for (int i = 0; i < Config.SceneConfigData.groupConfigs.Count; i++)
                {
                    for (int j = 0; j < Config.SceneConfigData.groupConfigs[i].sceneInfos.Count; j++)
                    {
                        sceneCount++;
                    }
                }
                tempSceneConfigInfos = new SceneConfigInfo[sceneCount];
                int currIndex = 0;
                for (int i = 0; i < Config.SceneConfigData.groupConfigs.Count; i++)
                {
                    for (int j = 0; j < Config.SceneConfigData.groupConfigs[i].sceneInfos.Count; j++)
                    {
                        tempSceneConfigInfos[currIndex] = Config.SceneConfigData.groupConfigs[i].sceneInfos[j];
                        currIndex++;
                    }
                }
            }
            else
            {
                tempSceneConfigInfos = Config.SceneConfigData.groupConfigs[Config.GroupIndexPanel - 1].sceneInfos.ToArray();
            }

            switch (Config.SortbyIndex)
            {
                //默认排序
                case 0:
                    return tempSceneConfigInfos;
                case 1:
                    Array.Sort(tempSceneConfigInfos);
                    return tempSceneConfigInfos;
                default:
                    goto case 0;
            }
        }
    }
}
