using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static QuickOpenScene.MainWindow;

namespace QuickOpenScene
{
    public class MainWindow : EditorWindow
    {
        public enum Sortby
        {
            默认, 命名
        }

        Sortby sortby;

        Vector2 scrollViewPos;
        GUIStyle versionStyle, buttonStyle;

        [MenuItem(Config.MenuPath.quickOpenSceneWindow)]
        static void Init()
        {
            EditorWindow editorWindow = GetWindow<MainWindow>();
            editorWindow.titleContent = new GUIContent("快速打开场景");
            editorWindow.Show();
        }

        private void OnGUI()
        {
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

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("配置文件：", GUILayout.Width(60f));
            EditorGUILayout.ObjectField(Config.SceneConfigData, typeof(SceneConfig), false);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("当前场景数量：" + Config.SceneConfigData.sceneInfos.Count);
            GUILayout.Label("排序方式：");
            sortby = (Sortby)EditorGUILayout.EnumPopup(sortby, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            if (Config.SceneConfigData.sceneInfos != null && Config.SceneConfigData.sceneInfos.Count > 0)
            {
                scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);
                for (int i = 0; i < Config.SceneConfigData.sceneInfos.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    //判断当前是否有数据
                    if (Config.SceneConfigData.sceneInfos[i].SceneGUID != string.Empty)
                    {
                        if (GUILayout.Button(new GUIContent("  " + Config.SceneConfigData.sceneInfos[i].SceneName, EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image), buttonStyle))
                        {
                            Config.SceneConfigData.sceneInfos[i].Refresh();
                            //左键点击打开场景
                            if (Event.current.button == 0)
                            {
                                //判断场景是否丢失
                                if (Config.SceneConfigData.sceneInfos[i].Scene != null)
                                {
                                    //判断场景是否需要保存
                                    if (SceneManager.GetActiveScene().isDirty)
                                    {
                                        int b = EditorUtility.DisplayDialogComplex("打开场景", $"确定要打开场景{Config.SceneConfigData.sceneInfos[i].SceneName}吗, \n请提前保存上一个场景!", "打开(保存场景)", "取消", "打开(不保存)");
                                        switch (b)
                                        {
                                            //打开(保存场景)
                                            case 0:
                                                EditorSceneManager.SaveOpenScenes();
                                                EditorSceneManager.OpenScene(Config.SceneConfigData.sceneInfos[i].ScenePath);
                                                break;

                                            //打开(不保存)
                                            case 2:
                                                EditorSceneManager.OpenScene(Config.SceneConfigData.sceneInfos[i].ScenePath);
                                                break;

                                            //取消
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        EditorSceneManager.OpenScene(Config.SceneConfigData.sceneInfos[i].ScenePath);
                                    }
                                }
                                else
                                {
                                    if (EditorUtility.DisplayDialog("场景丢失", $"场景{Config.SceneConfigData.sceneInfos[i].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                    {
                                        Debug.Log("删除 " + Config.SceneConfigData.sceneInfos[i].SceneName + " 场景成功！");
                                        Config.SceneConfigData.sceneInfos.Remove(Config.SceneConfigData.sceneInfos[i]);
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
                                        if (EditorUtility.DisplayDialog("场景丢失", $"场景{Config.SceneConfigData.sceneInfos[currIndex].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                        {
                                            SceneConfigManage.RemoveSceneInfo(Config.SceneConfigData.sceneInfos[currIndex]);
                                        }
                                    }
                                });
                                menu.AddItem(new GUIContent("删除当前场景"), false, () =>
                                {
                                    SceneConfigManage.RemoveSceneInfo(Config.SceneConfigData.sceneInfos[currIndex]);
                                });
                                menu.ShowAsContext();
                            }
                        }

                        //删除场景按钮
                        if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(30f)) && Event.current.button == 0)
                        {
                            SceneConfigManage.RemoveSceneInfo(Config.SceneConfigData.sceneInfos[i]);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
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
