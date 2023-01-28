using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickOpenScene
{
    public class MainWindow : EditorWindow
    {
        string[] sortbys = new string[] { "默认排序", "命名排序" };

        Vector2 scrollViewPos, mouseStartPos, mouseCurrentPos;
        GUIStyle versionStyle, buttonStyle, rightLableStyle;
        string search = string.Empty;


        [MenuItem(Config.MenuPath.quickOpenSceneWindow)]
        static void Init()
        {
            EditorWindow editorWindow = GetWindow<MainWindow>();
            editorWindow.titleContent = new GUIContent("快速打开场景");
            editorWindow.Show();
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
            GUILayout.Label("当前场景数量：" + Config.SceneConfigData.sceneInfos.Count, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("排序方式：", rightLableStyle, GUILayout.ExpandWidth(false));
            Config.SortbyIndex = EditorGUILayout.Popup(Config.SortbyIndex, sortbys, GUILayout.ExpandWidth(false), GUILayout.Width(70));

            EditorGUILayout.EndHorizontal();

            //场景
            if (Config.SceneConfigData.sceneInfos != null && Config.SceneConfigData.sceneInfos.Count > 0)
            {
                scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);

                for (int i = 0; i < Config.SceneConfigData.sceneInfos.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    //判断当前是否有数据
                    if (SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].SceneGUID != string.Empty)
                    {
                        //名称检索(搜索框为空或者搜索名称包含)
                        if (search == string.Empty || SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].SceneName.ToLower().Contains(search.ToLower()))
                        {
                            if (GUILayout.Button(new GUIContent("  " + SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].SceneName, EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image), buttonStyle))
                            {
                                SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].Refresh();

                                //左键点击打开场景
                                if (Event.current.button == 0)
                                {
                                    //判断场景是否丢失
                                    if (SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].Scene != null)
                                    {
                                        //判断场景是否需要保存
                                        if (SceneManager.GetActiveScene().isDirty)
                                        {
                                            int b = EditorUtility.DisplayDialogComplex("打开场景", $"确定要打开场景{SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].SceneName}吗, \n请提前保存上一个场景!", "打开(保存场景)", "取消", "打开(不保存)");
                                            switch (b)
                                            {
                                                //打开(保存场景)
                                                case 0:
                                                    EditorSceneManager.SaveOpenScenes();
                                                    EditorSceneManager.OpenScene(SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].ScenePath);
                                                    break;

                                                //打开(不保存)
                                                case 2:
                                                    EditorSceneManager.OpenScene(SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].ScenePath);
                                                    break;

                                                //取消
                                                default:
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            EditorSceneManager.OpenScene(SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].ScenePath);
                                        }
                                    }
                                    else
                                    {
                                        if (EditorUtility.DisplayDialog("场景丢失", $"场景{SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                        {
                                            Debug.Log("删除 " + SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i].SceneName + " 场景成功！");
                                            Config.SceneConfigData.sceneInfos.Remove(SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i]);
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
                                        GUIUtility.systemCopyBuffer = Config.SceneConfigData.sceneInfos[currIndex].SceneName;
                                    });
                                    menu.AddItem(new GUIContent("复制场景路径"), false, () =>
                                    {
                                        GUIUtility.systemCopyBuffer = Config.SceneConfigData.sceneInfos[currIndex].ScenePath;
                                    });
                                    menu.AddItem(new GUIContent("跳转到当前场景"), false, () =>
                                    {
                                        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(Config.SceneConfigData.sceneInfos[currIndex].ScenePath);
                                        if (scene != null)
                                        {
                                            EditorUtility.FocusProjectWindow();
                                            EditorGUIUtility.PingObject(scene);
                                        }
                                        else
                                        {
                                            if (EditorUtility.DisplayDialog("场景丢失", $"场景{SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[currIndex].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                            {
                                                SceneConfigManage.RemoveSceneInfo(SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[currIndex]);
                                            }
                                        }
                                    });
                                    menu.AddItem(new GUIContent("删除当前场景"), false, () =>
                                    {
                                        SceneConfigManage.RemoveSceneInfo(SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[currIndex]);
                                    });
                                    menu.ShowAsContext();
                                }
                            }

                            //删除场景按钮
                            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(30f)) && Event.current.button == 0)
                            {
                                SceneConfigManage.RemoveSceneInfo(SceneConfigManage.SceneConfigInfosSort(Config.SortbyIndex)[i]);
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
                        SceneConfigManage.AddScene(path, SceneConfigInfoType.scenePath);
                    }
                }
            }

            //鼠标点下时记录初始位置
            //if (Event.current.type == EventType.MouseDown)
            //{
            //    mouseStartPos = Event.current.mousePosition;
            //}           

            //鼠标长按拖拽时创建Lable
            //if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            //{
            //    mouseCurrentPos = Event.current.mousePosition;
            //    GUI.Label(new Rect(mouseCurrentPos.x - 50, mouseCurrentPos.y - 10, 100, 20), "这里是一个示例", buttonStyle);               
            //    Repaint();
            //}
            
            

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
    }
}
