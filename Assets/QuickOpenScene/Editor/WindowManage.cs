using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickOpenScene
{
    public class WindowManage : EditorWindow
    {
        SceneConfig sceneConfig;
        Vector2 scrollViewPos;

        [MenuItem(Config.MenuPath.quickOpenSceneWindow)]
        // Start is called before the first frame update
        static void Init()
        {
            EditorWindow editorWindow = GetWindow<WindowManage>();
            editorWindow.titleContent = new GUIContent("快速打开场景");
            editorWindow.Show();
        }
        void OnEnable()
        {
            sceneConfig = SceneConfigManage.GetSceneConfigObject();
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("配置文件：", GUILayout.Width(60f));
            sceneConfig = (SceneConfig)EditorGUILayout.ObjectField(sceneConfig, typeof(SceneConfig), false);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            GUIStyle buttonStyle = new GUIStyle("Button");
            buttonStyle.alignment = TextAnchor.MiddleLeft;

            if (sceneConfig.sceneInfos != null && sceneConfig.sceneInfos.Count > 0)
            {
                scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);
                for (int i = 0; i < sceneConfig.sceneInfos.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    //判断当前是否有数据
                    if (sceneConfig.sceneInfos[i].SceneGUID != string.Empty)
                    {
                        if (GUILayout.Button(new GUIContent("  " + sceneConfig.sceneInfos[i].SceneName, EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image), buttonStyle))
                        {
                            //左键点击打开场景
                            if (Event.current.button == 0)
                            {
                                sceneConfig.sceneInfos[i].Refresh();
                                //判断场景是否丢失
                                if (sceneConfig.sceneInfos[i].Scene != null)
                                {
                                    //判断场景是否需要保存
                                    if (SceneManager.GetActiveScene().isDirty)
                                    {
                                        int b = EditorUtility.DisplayDialogComplex("打开场景", $"确定要打开场景{sceneConfig.sceneInfos[i].SceneName}吗, \n请提前保存上一个场景!", "打开(保存场景)", "取消", "打开(不保存)");
                                        switch (b)
                                        {
                                            //打开(保存场景)
                                            case 0:
                                                EditorSceneManager.SaveOpenScenes();
                                                EditorSceneManager.OpenScene(sceneConfig.sceneInfos[i].ScenePath);
                                                break;

                                            //打开(不保存)
                                            case 2:
                                                EditorSceneManager.OpenScene(sceneConfig.sceneInfos[i].ScenePath);
                                                break;

                                            //取消
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        EditorSceneManager.OpenScene(sceneConfig.sceneInfos[i].ScenePath);
                                    }
                                }
                                else
                                {
                                    if (EditorUtility.DisplayDialog("场景丢失", $"场景{sceneConfig.sceneInfos[i].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                    {
                                        Debug.Log("删除 " + sceneConfig.sceneInfos[i].SceneName + " 场景成功！");
                                        sceneConfig.sceneInfos.Remove(sceneConfig.sceneInfos[i]);
                                    }

                                }
                            }
                            //右键点击弹出菜单
                            else if (Event.current.button == 1)
                            {
                                int currIndex = i;
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("删除当前场景"), false, () =>
                                {
                                    SceneConfigManage.RemoveScene(sceneConfig, sceneConfig.sceneInfos[currIndex]);
                                });
                                menu.AddItem(new GUIContent("跳转到当前场景"), false, () =>
                                {
                                    var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneConfig.sceneInfos[currIndex].ScenePath);
                                    if (scene != null)
                                    {
                                        EditorUtility.FocusProjectWindow();
                                        EditorGUIUtility.PingObject(scene);
                                    }
                                    else
                                    {
                                        if (EditorUtility.DisplayDialog("场景丢失", $"场景{sceneConfig.sceneInfos[currIndex].SceneName}已丢失，是否删除？", "删除数据", "取消"))
                                        {
                                            SceneConfigManage.RemoveScene(sceneConfig, sceneConfig.sceneInfos[currIndex]);
                                        }
                                    }
                                });
                                menu.AddItem(new GUIContent("复制场景名字"), false, () =>
                                {
                                    sceneConfig.sceneInfos[currIndex].Refresh();
                                    GUIUtility.systemCopyBuffer = sceneConfig.sceneInfos[currIndex].SceneName;
                                });
                                menu.AddItem(new GUIContent("复制场景路径"), false, () =>
                                {
                                    sceneConfig.sceneInfos[currIndex].Refresh();
                                    GUIUtility.systemCopyBuffer = sceneConfig.sceneInfos[currIndex].ScenePath;
                                });
                                menu.ShowAsContext();
                            }
                        }

                        //删除场景按钮
                        if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.Width(30f)) && Event.current.button == 0)
                        {
                            SceneConfigManage.RemoveScene(sceneConfig, sceneConfig.sceneInfos[i]);
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



            //如果鼠标正在拖拽中或拖拽结束时
            if (Event.current.type == EventType.DragUpdated)
            {
                if (DragAndDrop.paths[0].ToLower().Contains(".unity"))
                {
                    //改变鼠标的外表
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    foreach (var path in DragAndDrop.paths)
                    {
                        SceneConfigManage.AddScene(sceneConfig, path, SceneConfigInfo.SceneConfigInfoType.scenePath);
                    }

                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Version: " + Config.version, EditorStyles.centeredGreyMiniLabel);
        }
    }
}
