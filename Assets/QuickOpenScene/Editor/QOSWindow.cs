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
    public class QOSWindow : EditorWindow, IHasCustomMenu
    {
        private readonly string[] _sortOptions = { "默认排序", "命名排序" };
        private ReorderableList _sceneBtn;
        private Vector2 _scrollViewPos;
        private readonly Vector2 _createWindowSize = new Vector2(260, 150);
        private static readonly Vector2 QOSWindowMinSize = new Vector2(260, 200);
        private GUIStyle _versionStyle, _btnStyle, _rightLabelStyle;
        private static string _searchText = string.Empty;

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("添加当前场景到当前分组"), false, () =>
            {
                var scene = SceneManager.GetActiveScene();
                if (!scene.IsValid() || string.IsNullOrEmpty(scene.path))
                {
                    Debug.LogWarning("当前场景未保存");
                    return;
                }

                SceneConfigManage.AddScene(Config.CurrGroupIndex, scene.path, SceneConfigInfoType.scenePath);
            });
        }

        [MenuItem(Config.MenuPath.quickOpenSceneWindow)]
        static void Open()
        {
            var mainWindow = GetWindow<QOSWindow>();
            mainWindow.titleContent = new GUIContent("快速打开场景");
            mainWindow.minSize = QOSWindowMinSize;
            mainWindow.Show();
        }

        void ScenesPanelRefresh()
        {
            if (_sceneBtn == null || _sceneBtn.count != GetSceneConfigInfos.Length ||
                Event.current.commandName == "GroupIndexPanelChange")
            {
                _sceneBtn = Config.GroupIndexPanel == 0
                    ? new ReorderableList(GetSceneConfigInfos, typeof(SceneConfigInfo))
                    : new ReorderableList(SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].sceneInfos,
                        typeof(SceneConfigInfo));

                _sceneBtn.displayAdd = false;
                _sceneBtn.displayRemove = false;
                _sceneBtn.drawHeaderCallback = DrawHeaderCallback;
                _sceneBtn.drawElementCallback = DrawElementCallback;
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

            SceneConfigManage.CheckSceneConfig();
        }

        static void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, new GUIContent(Config.GroupStr[Config.GroupIndexPanel] + "场景"));
        }

        private Rect _sceneRect, _removeRect;

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height -= 4;
            rect.y += 2;

            //GUIStyle初始化
            _btnStyle = new GUIStyle("button");
            _btnStyle.alignment = TextAnchor.MiddleLeft;

            _sceneRect = new Rect(rect);
            _removeRect = new Rect(rect);

            _removeRect.width = 30;
            _sceneRect.width = rect.width - _removeRect.width - 4;
            _removeRect.position = new Vector2(_sceneRect.position.x + _sceneRect.width + 4, rect.y);


            //场景按钮
            if (SceneConfigData.sceneConfig.groupConfigs != null && SceneConfigData.sceneConfig.groupConfigs.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();

                //名称检索(搜索框为空或者搜索名称包含)
                isActive = !CheckSearchResults(GetSceneConfigInfos[index]);
                EditorGUI.BeginDisabledGroup(isActive);
                {
                    if (!isActive && _searchText.Trim() != string.Empty)
                    {
                        _btnStyle.normal.textColor = Color.green;
                    }

                    if (GUI.Button(_sceneRect,
                            new GUIContent("  " + GetSceneConfigInfos[index].sceneName,
                                EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image), _btnStyle))
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
                                menu.AddItem(new GUIContent("复制场景名字"), false,
                                    () => { GUIUtility.systemCopyBuffer = GetSceneConfigInfos[currIndex].sceneName; });
                                menu.AddItem(new GUIContent("复制场景路径"), false,
                                    () => { GUIUtility.systemCopyBuffer = GetSceneConfigInfos[currIndex].scenePath; });
                                menu.AddItem(new GUIContent("添加到当前场景"), false, () =>
                                {
                                    if (File.Exists(GetSceneConfigInfos[currIndex].scenePath))
                                    {
                                        EditorSceneManager.OpenScene(GetSceneConfigInfos[currIndex].scenePath,
                                            OpenSceneMode.Additive);
                                    }
                                    else
                                    {
                                        SceneLostDisplayDialog(currIndex);
                                    }
                                });
                                menu.AddItem(new GUIContent("跳转到当前场景"), false, () =>
                                {
                                    var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
                                        GetSceneConfigInfos[currIndex].scenePath);
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
                                        SceneConfigManage.RemoveSceneInfo(Config.CurrGroupIndex,
                                            GetSceneConfigInfos[currIndex]);
                                    }
                                    else
                                    {
                                        if (EditorUtility.DisplayDialog("删除警告",
                                                $"当前显示的为所有分组，将删除所有分组内的{GetSceneConfigInfos[currIndex].sceneName}场景信息，是否删除？",
                                                "删除数据", "取消"))
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
                                        if (SceneConfigData.sceneConfig.groupConfigs[i].groupName != SceneConfigData
                                                .sceneConfig.groupConfigs[Config.CurrGroupIndex].groupName)
                                        {
                                            menu.AddItem(
                                                new GUIContent(
                                                    $"复制场景到/{SceneConfigData.sceneConfig.groupConfigs[i].groupName} 分组"),
                                                false,
                                                () =>
                                                {
                                                    SceneConfigManage.AddScene(currGroupIndex,
                                                        SceneConfigData.sceneConfig
                                                            .groupConfigs[Config.CurrGroupIndex]
                                                            .sceneInfos[currIndex]);
                                                });
                                        }
                                    }
                                }

                                menu.ShowAsContext();
                                break;
                            }
                        }
                    }

                    //删除场景按钮
                    if (GUI.Button(_removeRect, EditorGUIUtility.IconContent("TreeEditor.Trash")) &&
                        Event.current.button == 0)
                    {
                        if (Config.GroupIndexPanel == 0)
                        {
                            if (EditorUtility.DisplayDialog("删除警告",
                                    $"当前显示的为所有分组，将删除所有分组内的{GetSceneConfigInfos[index].sceneName}场景信息，是否删除？", "删除数据",
                                    "取消"))
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

                EditorGUILayout.EndHorizontal();
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
                if (EditorUtility.DisplayDialog("场景丢失", $"场景{GetSceneConfigInfos[currIndex].sceneName}已丢失，是否删除？",
                        "删除数据", "取消"))
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
            _sceneBtn.draggable = Config.SortbyIndex == 1 || Config.GroupIndexPanel == 0 ? false : true;
            _sceneBtn.onChangedCallback = OnChangedCallback;
            //GUIStyle初始化
            if (_versionStyle == null)
            {
                _versionStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                _versionStyle.normal.textColor = Color.red;
                _versionStyle.hover.textColor = Color.white;
                _versionStyle.active.textColor = Color.gray;
            }


            _rightLabelStyle = new GUIStyle(EditorStyles.label);
            _rightLabelStyle.border.top = 3;


            //名称检索
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("名称搜索：", GUILayout.ExpandWidth(false));
            _searchText = EditorGUILayout.TextField(_searchText);
            if (GUILayout.Button("清空", GUILayout.ExpandWidth(false)))
            {
                _searchText = string.Empty;
            }

            EditorGUILayout.EndHorizontal();


            //场景计数，排序方式
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("当前场景数量：" + GetSceneConfigInfos.Length, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("排序方式：", _rightLabelStyle, GUILayout.ExpandWidth(false));
            Config.SortbyIndex = EditorGUILayout.Popup(Config.SortbyIndex, _sortOptions, GUILayout.ExpandWidth(false),
                GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("分组：", GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            {
                Config.GroupIndexPanel =
                    EditorGUILayout.Popup(Config.GroupIndexPanel, Config.GroupStr, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    GUIUtility.ExitGUI();
                }
            }
            //新建分组按钮
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus").image, "新建一个新的分组并跳转"),
                    GUILayout.ExpandWidth(false)) && Event.current.button == 0)
            {
                CreateGroupWindow window = GetWindow<CreateGroupWindow>(true);
                window.SendEvent(EditorGUIUtility.CommandEvent("Create"));
                window.titleContent = new GUIContent("创建分组");
                window.minSize = _createWindowSize;
                window.maxSize = _createWindowSize;
                Rect mainRect = GetWindow<QOSWindow>().position;
                window.position =
                    new Rect(mainRect.position + mainRect.size / 2 - new Vector2(window.position.size.x / 2, 0),
                        window.minSize);
                window.Show();
                window.Focus();
            }

            //根据路径刷新场景信息
            EditorGUI.BeginDisabledGroup(!SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].UseBindPath);
            {
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "更新分组内绑定路径的场景"),
                        GUILayout.ExpandWidth(false)) && Event.current.button == 0)
                {
                    if (Config.GroupIndexPanel == 0)
                    {
                        for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
                        {
                            if (EditorUtility.DisplayCancelableProgressBar("刷新场景信息",
                                    SceneConfigData.sceneConfig.groupConfigs[i].groupName,
                                    i / SceneConfigData.sceneConfig.groupConfigs.Count - 1))
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
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("editicon.sml").image, "修改分组属性"),
                        GUILayout.ExpandWidth(false)) && Event.current.button == 0)
                {
                    CreateGroupWindow window = GetWindow<CreateGroupWindow>(true);
                    window.titleContent = new GUIContent(Config.GroupStr[Config.GroupIndexPanel] + "分组属性");
                    window.minSize = _createWindowSize;
                    window.maxSize = _createWindowSize;
                    Rect mainRect = GetWindow<QOSWindow>().position;
                    window.position =
                        new Rect(mainRect.position + mainRect.size / 2 - new Vector2(window.position.size.x / 2, 0),
                            window.minSize);
                    window.Show();
                    window.Focus();
                    window.SendEvent(EditorGUIUtility.CommandEvent("Rename"));
                }

                EditorGUI.BeginDisabledGroup(SceneConfigData.sceneConfig.groupConfigs.Count == 1);
                {
                    //删除分组
                    if (GUILayout.Button(
                            new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Trash").image, "删除当前分组和当前分组内的场景数据"),
                            GUILayout.ExpandWidth(false)) && Event.current.button == 0)
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


            _scrollViewPos = GUILayout.BeginScrollView(_scrollViewPos);
            _sceneBtn.DoLayoutList();
            GUILayout.EndScrollView();


            //如果鼠标正在拖拽中，并且为unity文件
            if (Event.current.type == EventType.DragUpdated &&
                Path.GetExtension(DragAndDrop.paths[0]).Contains(".unity"))
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
                if (GUILayout.Button($"Version: {Config.currVersion}（需要更新）", _versionStyle))
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
            if (_searchText.Contains(" "))
            {
                allSearchText = _searchText.Split(' ');
            }
            else
            {
                allSearchText = new[] { _searchText };
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
        private static void OpenScene(string scenePath)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning(Config.SceneIsPlayingWaring);
                GetWindow<QOSWindow>().ShowNotification(new GUIContent(Config.SceneIsPlayingWaring));
                return;
            }

            //判断场景是否需要保存
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }

        private static SceneConfigInfo[] _tempSceneConfigInfos;
        private static bool _isGet;
        private static bool _isSort;

        /// <summary>
        /// 场景排序
        /// </summary>
        /// <returns>排序后结果</returns>
        SceneConfigInfo[] GetSceneConfigInfos
        {
            get
            {
                var tempList = new List<SceneConfigInfo>();
                if (!_isGet)
                {
                    if (Config.GroupIndexPanel == 0)
                    {
                        for (int i = 0; i < SceneConfigData.sceneConfig.groupConfigs.Count; i++)
                        {
                            for (int j = 0; j < SceneConfigData.sceneConfig.groupConfigs[i].sceneInfos.Count; j++)
                            {
                                tempList.Add(SceneConfigData.sceneConfig.groupConfigs[i].sceneInfos[j]);
                            }
                        }

                        _tempSceneConfigInfos = tempList.ToArray();
                    }
                    else
                    {
                        if (_tempSceneConfigInfos != null && _tempSceneConfigInfos != SceneConfigData.sceneConfig
                                .groupConfigs[Config.CurrGroupIndex]
                                .sceneInfos.ToArray())
                        {
                            _tempSceneConfigInfos = SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex]
                                .sceneInfos.ToArray();
                        }
                    }

                    _isGet = true;
                }

                switch (Config.SortbyIndex)
                {
                    //默认排序
                    case 0:
                        _isSort = false;
                        if (Config.GroupIndexPanel > 0)
                        {
                            return SceneConfigData.sceneConfig.groupConfigs[Config.CurrGroupIndex].sceneInfos.ToArray();
                        }
                        else
                        {
                            return _tempSceneConfigInfos;
                        }
                    case 1:
                        if (!_isSort)
                        {
                            if (_tempSceneConfigInfos != null)
                            {
                                Array.Sort(_tempSceneConfigInfos);
                            }
                        }

                        _isSort = true;
                        return _tempSceneConfigInfos;
                    default:
                        goto case 0;
                }
            }

            set => _tempSceneConfigInfos = value;
        }


        /// <summary>
        /// 标记重新获取数据
        /// </summary>
        public static void RefreshGetSceneConfigInfos()
        {
            _isGet = false; // 标记需要重新获取数据
            _isSort = false;
        }
    }
}