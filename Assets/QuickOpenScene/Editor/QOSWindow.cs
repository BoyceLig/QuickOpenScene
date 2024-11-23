using System;
using System.Collections.Generic;
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
        readonly Vector2 createWindowSize = new Vector2(260, 150);
        static Vector2 qosWindowMinSize = new Vector2(260, 200);
        GUIStyle versionStyle, buttonStyle, rightLableStyle;
        static string searchText = string.Empty;

        [MenuItem(Config.MenuPath.quickOpenSceneWindow)]
        static void Open()
        {
            QOSWindow mainWindow = GetWindow<QOSWindow>();
            mainWindow.titleContent = new GUIContent("快速打开场景");
            mainWindow.minSize = qosWindowMinSize;
            mainWindow.Show();
            SceneConfigManage.CheckSceneConfig();
        }

        void ScenesPanelRefresh()
        {
            if (sceneButtons == null || sceneButtons.count != GetSceneConfigInfos.Length || Event.current.commandName == "GroupIndexPanelChange")
            {
                sceneButtons = Config.GroupIndexPanel == 0 ? new ReorderableList(GetSceneConfigInfos, typeof(SceneConfigInfo)) : new ReorderableList(SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].sceneInfos, typeof(SceneConfigInfo));

                sceneButtons.displayAdd = false;
                sceneButtons.displayRemove = false;
                sceneButtons.drawHeaderCallback = DrawHeaderCallback;
                sceneButtons.drawElementCallback = DrawElementCallback;
            }
        }

        void OnDestroy()
        {
            SceneConfigData.instance.SaveDate();
        }

        private void OnEnable()
        {
            if (Config.GroupIndexPanel == 0)
            {
                Config.GroupIndexPanel = 1;
            }
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
            if (SceneConfigData.sceneConfig.groupConfigs != null && SceneConfigData.sceneConfig.groupConfigs.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                try
                {
                    //判断当前是否有数据
                    if (GetSceneConfigInfos[index].sceneGUID == string.Empty) return;
                    //名称检索(搜索框为空或者搜索名称包含)
                    isActive = !CheckSearchResults(GetSceneConfigInfos[index]);
                    EditorGUI.BeginDisabledGroup(isActive);
                    {
                        if (!isActive && searchText.Trim() != string.Empty)
                        {
                            buttonStyle.normal.textColor = Color.green;
                        }

                        if (GUI.Button(sceneRect, new GUIContent("  " + GetSceneConfigInfos[index].sceneName, EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image), buttonStyle))
                        {
                            if (GetSceneConfigInfos[index].Refresh())
                            {
                                SceneConfigData.instance.SaveDate();
                            }

                            switch (Event.current.button)
                            {
                                //左键点击打开场景
                                case 0:
                                {
                                    var sceneInfo = GetSceneConfigInfos[index];
                                    if (IsSceneValid(sceneInfo) || TryRefreshSceneInfo(sceneInfo))
                                    {
                                        //要打开的场景是否是当前已经打开的场景
                                        var s = false;
                                        for (var i = 0; i < SceneManager.sceneCount; i++)
                                        {
                                            if (SceneManager.GetSceneAt(i).path == sceneInfo.scenePath)
                                            {
                                                s = true;
                                                break;
                                            }
                                        }

                                        if (!s)
                                        {
                                            OpenScene(sceneInfo.scenePath);
                                        }
                                    }
                                    else
                                    {
                                        SceneLostDisplayDialog(index);
                                    }

                                    break;
                                }
                                //右键点击弹出菜单
                                case 1:
                                {
                                    var currIndex = index;
                                    var menu = new GenericMenu();
                                    menu.AddItem(new GUIContent("复制场景名字"), false, () => { GUIUtility.systemCopyBuffer = GetSceneConfigInfos[currIndex].sceneName; });
                                    menu.AddItem(new GUIContent("复制场景路径"), false, () => { GUIUtility.systemCopyBuffer = GetSceneConfigInfos[currIndex].scenePath; });
                                    menu.AddItem(new GUIContent("添加到当前场景"), false, () =>
                                    {
                                        if (File.Exists(GetSceneConfigInfos[currIndex].scenePath))
                                        {
                                            EditorSceneManager.OpenScene(GetSceneConfigInfos[currIndex].scenePath, OpenSceneMode.Additive);
                                        }
                                        else
                                        {
                                            SceneLostDisplayDialog(currIndex);
                                        }
                                    });
                                    menu.AddItem(new GUIContent("跳转到当前场景"), false, () =>
                                    {
                                        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(GetSceneConfigInfos[currIndex].scenePath);
                                        if (scene != null)
                                        {
                                            EditorUtility.FocusProjectWindow();
                                            EditorGUIUtility.PingObject(scene);
                                        }
                                        else
                                        {
                                            SceneLostDisplayDialog(currIndex);
                                        }
                                    });
                                    menu.AddItem(new GUIContent("删除当前场景"), false, () =>
                                    {
                                        if (Config.GroupIndexPanel > 0)
                                        {
                                            SceneConfigManage.RemoveSceneInfo(Config.CurrGroupIndex, GetSceneConfigInfos[currIndex]);
                                        }
                                        else
                                        {
                                            if (EditorUtility.DisplayDialog("删除警告", $"当前显示的为所有分组，将删除所有分组内的{GetSceneConfigInfos[currIndex].sceneName}场景信息，是否删除？", "删除数据", "取消"))
                                            {
                                                SceneConfigManage.RemoveSceneInfo(GetSceneConfigInfos[currIndex]);
                                            }
                                        }
                                    });
                                    if (Config.GroupIndexPanel > 0)
                                    {
                                        for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
                                        {
                                            int currGroupIndex = i;
                                            if (SceneConfigData.sceneConfig.groupConfigs[i].groupName != SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].groupName)
                                            {
                                                menu.AddItem(new GUIContent($"复制场景到/{SceneConfigData.sceneConfig.groupConfigs[i].groupName} 分组"), false, () => { SceneConfigManage.AddScene(currGroupIndex, SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].sceneInfos[currIndex]); });
                                            }
                                        }
                                    }

                                    menu.ShowAsContext();
                                    break;
                                }
                            }
                        }

                        //删除场景按钮
                        if (GUI.Button(removeRect, EditorGUIUtility.IconContent("TreeEditor.Trash")) && Event.current.button == 0)
                        {
                            if (Config.GroupIndexPanel == 0)
                            {
                                if (EditorUtility.DisplayDialog("删除警告", $"当前显示的为所有分组，将删除所有分组内的{GetSceneConfigInfos[index].sceneName}场景信息，是否删除？", "删除数据", "取消"))
                                {
                                    SceneConfigManage.RemoveSceneInfo(GetSceneConfigInfos[index]);
                                }
                            }
                            else
                            {
                                SceneConfigManage.RemoveSceneInfo(Config.CurrGroupIndex, GetSceneConfigInfos[index]);
                            }

                            GUIUtility.ExitGUI();
                        }

                        EditorGUI.EndDisabledGroup();
                    }
                }
                finally
                {
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("场景为空，请添加场景!");
            }
        }

        private static void OnChangedCallback(ReorderableList list)
        {
            SceneConfigData.instance.SaveDate();
        }

        private static bool TryRefreshSceneInfo(SceneConfigInfo sceneInfo)
        {
            if (sceneInfo.Refresh())
            {
                SceneConfigData.instance.SaveDate();
            }

            return IsSceneValid(sceneInfo);
        }

        private static bool IsSceneValid(SceneConfigInfo sceneInfo)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneInfo.scenePath);
            return sceneAsset != null;
        }

        private void SceneLostDisplayDialog(int currIndex)
        {
            {
                if (EditorUtility.DisplayDialog("场景丢失", $"场景{GetSceneConfigInfos[currIndex].sceneName}已丢失，是否删除？", "删除数据", "取消"))
                {
                    if (Config.GroupIndexPanel > 0)
                    {
                        SceneConfigManage.RemoveSceneInfo(Config.CurrGroupIndex, GetSceneConfigInfos[currIndex]);
                    }
                    else
                    {
                        SceneConfigManage.RemoveSceneInfo(GetSceneConfigInfos[currIndex]);
                    }
                }
            }
        }

        void OnGUI()
        {
            ScenesPanelRefresh();
            sceneButtons.draggable = Config.SortbyIndex == 1 || Config.GroupIndexPanel == 0 ? false : true;
            sceneButtons.onChangedCallback = OnChangedCallback;
            //GUIStyle初始化
            if (versionStyle == null)
            {
                versionStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                versionStyle.normal.textColor = Color.red;
                versionStyle.hover.textColor = Color.white;
                versionStyle.active.textColor = Color.gray;
            }


            rightLableStyle = new GUIStyle(EditorStyles.label);
            rightLableStyle.border.top = 3;


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
            GUILayout.Label("当前场景数量：" + GetSceneConfigInfos.Length, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("排序方式：", rightLableStyle, GUILayout.ExpandWidth(false));
            Config.SortbyIndex = EditorGUILayout.Popup(Config.SortbyIndex, sortbys, GUILayout.ExpandWidth(false), GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("分组：", GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            {
                Config.GroupIndexPanel = EditorGUILayout.Popup(Config.GroupIndexPanel, Config.GroupStr, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    GUIUtility.ExitGUI();
                }
            }
            //新建分组按钮
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus").image, "新建一个新的分组并跳转"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
            {
                CreateGroupWindow window = GetWindow<CreateGroupWindow>(true);
                window.SendEvent(EditorGUIUtility.CommandEvent("Create"));
                window.titleContent = new GUIContent("创建分组");
                window.minSize = createWindowSize;
                window.maxSize = createWindowSize;
                Rect mainRect = GetWindow<QOSWindow>().position;
                window.position = new Rect(mainRect.position + mainRect.size / 2 - new Vector2(window.position.size.x / 2, 0), window.minSize);
                window.Show();
                window.Focus();
            }

            //根据路径刷新场景信息
            EditorGUI.BeginDisabledGroup(!SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].UseBindPath);
            {
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "更新分组内绑定路径的场景"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
                {
                    if (Config.GroupIndexPanel == 0)
                    {
                        for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
                        {
                            if (EditorUtility.DisplayCancelableProgressBar("刷新场景信息", SceneConfigData.sceneConfig.groupConfigs[i].groupName, i / SceneConfigData.sceneConfig.groupConfigs.Count - 1))
                            {
                                EditorUtility.ClearProgressBar();
                                return;
                            }

                            SceneConfigManage.RefreshCurrSceneConfigForPath(i);
                        }
                    }
                    else
                    {
                        SceneConfigManage.RefreshCurrSceneConfigForPath(Config.CurrGroupIndex);
                    }

                    EditorUtility.ClearProgressBar();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(Config.GroupIndexPanel == 0);
            {
                //重命名
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("editicon.sml").image, "修改分组属性"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
                {
                    CreateGroupWindow window = GetWindow<CreateGroupWindow>(true);
                    window.titleContent = new GUIContent(Config.GroupStr[Config.GroupIndexPanel] + "分组属性");
                    window.minSize = createWindowSize;
                    window.maxSize = createWindowSize;
                    Rect mainRect = GetWindow<QOSWindow>().position;
                    window.position = new Rect(mainRect.position + mainRect.size / 2 - new Vector2(window.position.size.x / 2, 0), window.minSize);
                    window.Show();
                    window.Focus();
                    window.SendEvent(EditorGUIUtility.CommandEvent("Rename"));
                }

                EditorGUI.BeginDisabledGroup(SceneConfigData.sceneConfig.groupConfigs.Count == 1);
                {
                    //删除分组
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Trash").image, "删除当前分组和当前分组内的场景数据"), GUILayout.ExpandWidth(false)) && Event.current.button == 0)
                    {
                        Config.GroupIndexPanel -= 1;
                        SceneConfigData.sceneConfig.groupConfigs.RemoveAt(Config.GroupIndexPanel);
                        SceneConfigData.instance.SaveDate();
                        GUIUtility.ExitGUI();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup();
            }
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
        /// <returns>如果匹配则返回false，如果不匹则返回true</returns>
        private static bool CheckSearchResults(SceneConfigInfo sceneConfigInfo)
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
                if (!sceneConfigInfo.sceneName.ToLower().Contains(item.ToLower()))
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
                    void OpenScene()
                    {
                        if (EditorApplication.isPlaying == false)
                        {
                            EditorSceneManager.OpenScene(path);
                            EditorApplication.update -= OpenScene;
                        }
                    }

                    if (EditorUtility.DisplayDialog("警告", "场景正在运行，是否继续打开新场景？", "继续", "取消"))
                    {
                        EditorApplication.isPlaying = false;
                        EditorApplication.update += OpenScene;
                    }
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
        static bool isGet = false;
        static bool isSort = false;

        /// <summary>
        /// 场景排序
        /// </summary>
        /// <returns>排序后结果</returns>
        SceneConfigInfo[] GetSceneConfigInfos
        {
            get
            {
                List<SceneConfigInfo> templist = new List<SceneConfigInfo>();
                if (!isGet)
                {
                    if (Config.GroupIndexPanel == 0)
                    {
                        for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
                        {
                            for (int j = 0; j < SceneConfigData.sceneConfig.groupConfigs[i].sceneInfos.Count; j++)
                            {
                                templist.Add(SceneConfigData.sceneConfig.groupConfigs[i].sceneInfos[j]);
                            }
                        }

                        tempSceneConfigInfos = templist.ToArray();
                    }
                    else
                    {
                        if (tempSceneConfigInfos != SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].sceneInfos.ToArray())
                        {
                            tempSceneConfigInfos = SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].sceneInfos.ToArray();
                        }
                    }

                    isGet = true;
                }

                switch (Config.SortbyIndex)
                {
                    //默认排序
                    case 0:
                        isSort = false;
                        if (Config.GroupIndexPanel > 0)
                        {
                            return SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].sceneInfos.ToArray();
                        }
                        else
                        {
                            return tempSceneConfigInfos;
                        }
                    case 1:
                        if (!isSort)
                        {
                            Array.Sort(tempSceneConfigInfos);
                            //Debug.Log("排序");
                        }

                        isSort = true;
                        return tempSceneConfigInfos;
                    default:
                        goto case 0;
                }
            }

            set => tempSceneConfigInfos = value;
        }


        /// <summary>
        /// 标记重新获取数据
        /// </summary>
        public static void RefreshGetSceneConfigInfos()
        {
            isGet = false; // 标记需要重新获取数据
            isSort = false;
        }
    }
}