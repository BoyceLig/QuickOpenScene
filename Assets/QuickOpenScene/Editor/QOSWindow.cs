using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickOpenScene
{
    public class QOSWindow : EditorWindow
    {
        string[] sortbys = new string[] { "默认排序", "命名排序" };
        ReorderableList sceneButtons;
        Vector2 scrollViewPos;
        GUIStyle versionStyle, buttonStyle, rightLableStyle;
        static string searchText = string.Empty;

        [MenuItem(Config.MenuPath.quickOpenSceneWindow)]
        static void Open()
        {
            QOSWindow mainWindow = GetWindow<QOSWindow>();
            mainWindow.titleContent = new GUIContent("快速打开场景");
            mainWindow.minSize = new Vector2(260, 200);
            mainWindow.Show();
            SceneConfigManage.CheckSceneConfig();
        }
        void ScenesPanelRefresh()
        {
            if (sceneButtons == null || sceneButtons.count != SceneConfigInfosSort().Length || Event.current.commandName == "GroupIndexPanelChange")
            {
                sceneButtons = Config.GroupIndexPanel == 0 ?
                new ReorderableList(SceneConfigInfosSort(), typeof(SceneConfigInfo)) :
                new ReorderableList(Config.SceneConfigData.groupConfigs[Config.CurrGroupIndex].sceneInfos, typeof(SceneConfigInfo));
                sceneButtons.displayAdd = false;
                sceneButtons.displayRemove = false;
                sceneButtons.drawHeaderCallback = DrawHeaderCallback;
                sceneButtons.drawElementCallback = DrawElementCallback;
            }
        }
        void OnDestroy()
        {
            SceneConfigManage.SaveSceneConfigData();
        }

        static void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, new GUIContent(Config.GroupStr[Config.GroupIndexPanel] + "场景"));
        }

        Rect sceneRect, removeRect;
        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height -= 4;
            rect.y += 2;

            //GUIStyle初始化
            buttonStyle = new GUIStyle("button");
            buttonStyle.alignment = TextAnchor.MiddleLeft;

            sceneRect = new Rect(rect);
            removeRect = new Rect(rect);

            removeRect.width = 30;
            sceneRect.width = rect.width - removeRect.width - 4;
            removeRect.position = new Vector2(sceneRect.position.x + sceneRect.width + 4, rect.y);

            //场景按钮
            if (Config.SceneConfigData.groupConfigs != null && Config.SceneConfigData.groupConfigs.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                //判断当前是否有数据
                if (SceneConfigInfosSort()[index].SceneGUID != string.Empty)
                {
                    //名称检索(搜索框为空或者搜索名称包含)                    
                    EditorGUI.BeginDisabledGroup(!CheckSearchResults(SceneConfigInfosSort()[index]));
                    if (GUI.Button(sceneRect, new GUIContent("  " + SceneConfigInfosSort()[index].SceneName, EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image), buttonStyle))
                    {
                        SceneConfigInfosSort()[index].Refresh();

                        //左键点击打开场景
                        if (Event.current.button == 0)
                        {
                            //判断场景是否丢失
                            if (SceneConfigInfosSort()[index].Scene != null)
                            {
                                OpenScene(SceneConfigInfosSort()[index].ScenePath);
                            }
                            else
                            {
                                if (EditorUtility.DisplayDialog("场景丢失", $"场景{SceneConfigInfosSort()[index].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                {
                                    SceneConfigManage.RemoveSceneInfo(SceneConfigInfosSort()[index]);
                                }
                            }
                        }
                        //右键点击弹出菜单
                        else if (Event.current.button == 1)
                        {
                            int currIndex = index;
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
                                            SceneConfigManage.RemoveSceneInfo(Config.CurrGroupIndex, SceneConfigInfosSort()[currIndex]);
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
                                        SceneConfigManage.RemoveSceneInfo(Config.CurrGroupIndex, SceneConfigInfosSort()[currIndex]);
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
                            if (Config.GroupIndexPanel > 0)
                            {
                                for (int i = 0; i < Config.SceneConfigData.groupConfigs.Count; i++)
                                {
                                    int currGroupIndex = i;
                                    if (Config.SceneConfigData.groupConfigs[i].groupName != Config.SceneConfigData.groupConfigs[Config.CurrGroupIndex].groupName)
                                    {
                                        menu.AddItem(new GUIContent($"复制场景到/{Config.SceneConfigData.groupConfigs[i].groupName} 分组"), false, () =>
                                        {
                                            SceneConfigManage.AddScene(currGroupIndex, Config.SceneConfigData.groupConfigs[Config.CurrGroupIndex].sceneInfos[currIndex]);
                                        });
                                    }
                                }
                            }
                            menu.ShowAsContext();
                        }
                    }

                    //删除场景按钮
                    if (GUI.Button(removeRect, EditorGUIUtility.IconContent("TreeEditor.Trash")) && Event.current.button == 0)
                    {
                        if (Config.GroupIndexPanel == 0)
                        {
                            if (EditorUtility.DisplayDialog("删除警告", $"当前显示的为所有分组，将删除所有分组内的{SceneConfigInfosSort()[index].SceneName}场景信息，是否删除？", "删除数据", "取消"))
                            {
                                SceneConfigManage.RemoveSceneInfo(SceneConfigInfosSort()[index]);
                            }
                        }
                        else
                        {
                            SceneConfigManage.RemoveSceneInfo(Config.CurrGroupIndex, SceneConfigInfosSort()[index]);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("场景为空，请添加场景!");
            }
        }

        void OnGUI()
        {
            ScenesPanelRefresh();
            sceneButtons.draggable = Config.SortbyIndex == 1 || Config.GroupIndexPanel == 0 ? false : true;

            //GUIStyle初始化
            if (versionStyle == null)
            {
                versionStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                versionStyle.normal.textColor = Color.red;
                versionStyle.hover.textColor = Color.white;
                versionStyle.active.textColor = Color.gray;
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
            searchText = EditorGUILayout.TextField(searchText);
            if (GUILayout.Button("清空", GUILayout.ExpandWidth(false)))
            {
                searchText = string.Empty;
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
                Rect mainRect = GetWindow<QOSWindow>().position;
                window.position = new Rect(mainRect.position + mainRect.size / 2 - new Vector2(window.position.size.x / 2, 0), window.minSize);
                window.Show();
                window.Focus();
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
                Rect mainRect = GetWindow<QOSWindow>().position;
                window.position = new Rect(mainRect.position + mainRect.size / 2 - new Vector2(window.position.size.x / 2, 0), window.minSize);
                window.Show();
                window.Focus();
                window.SendEvent(EditorGUIUtility.CommandEvent("Rename"));
            }

            EditorGUI.BeginDisabledGroup(Config.SceneConfigData.groupConfigs.Count == 1);

            //删除分组
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Trash").image, "删除当前分组和当前分组内的场景数据"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
            {
                Config.GroupIndexPanel -= 1;
                Config.SceneConfigData.groupConfigs.RemoveAt(Config.GroupIndexPanel);
                EditorUtility.SetDirty(Config.SceneConfigData);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();


            EditorGUILayout.EndHorizontal();


            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);
            sceneButtons.DoLayoutList();
            GUILayout.EndScrollView();


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
                        SceneConfigManage.AddScene(Config.CurrGroupIndex, path, SceneConfigInfoType.scenePath);
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

        /// <summary>
        /// 匹配搜索结果是否包含字符
        /// </summary>
        /// <param name="sceneConfigInfo">当前的数据</param>
        /// <returns>结果</returns>

        static bool CheckSearchResults(SceneConfigInfo sceneConfigInfo)
        {
            string[] allSearchText;
            if (searchText.Contains(" "))
            {
                allSearchText = searchText.Split(' ');
            }
            else
            {
                allSearchText = new string[1] { searchText };
            }
            foreach (var item in allSearchText)
            {
                if (!sceneConfigInfo.SceneName.ToLower().Contains(item.ToLower()))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 打开场景
        /// </summary>
        /// <param name="scenePath">场景路径</param>
        static void OpenScene(string scenePath)
        {
            void SceneIsPlayingCheck(string path)
            {
                if (EditorApplication.isPlaying)
                {
                    EditorUtility.DisplayDialog("警告", "场景正在运行，请停止后再切换场景。", "确认");
                }
                else
                {
                    EditorSceneManager.OpenScene(path);
                }

            }

            //判断场景是否需要保存
            if (SceneManager.GetActiveScene().isDirty)
            {
                int b = EditorUtility.DisplayDialogComplex("保存", $"当前场景有修改，要打开场景 {Path.GetFileNameWithoutExtension(scenePath)} 是否保存当前场景？", "打开(保存场景)", "取消", "打开(不保存)");
                switch (b)
                {
                    //打开(保存场景)
                    case 0:
                        EditorSceneManager.SaveOpenScenes();
                        SceneIsPlayingCheck(scenePath);
                        break;

                    //打开(不保存)
                    case 2:
                        SceneIsPlayingCheck(scenePath);
                        break;

                    //取消
                    default:
                        break;
                }
            }
            else
            {
                SceneIsPlayingCheck(scenePath);
            }
        }

        static SceneConfigInfo[] tempSceneConfigInfos;

        /// <summary>
        /// 场景排序
        /// </summary>
        /// <returns>排序后结果</returns>
        static SceneConfigInfo[] SceneConfigInfosSort()
        {
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
                if (tempSceneConfigInfos == null || tempSceneConfigInfos.Length != sceneCount)
                {
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
            }
            else
            {
                if (tempSceneConfigInfos != Config.SceneConfigData.groupConfigs[Config.CurrGroupIndex].sceneInfos.ToArray())
                {
                    tempSceneConfigInfos = Config.SceneConfigData.groupConfigs[Config.CurrGroupIndex].sceneInfos.ToArray();
                }
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
