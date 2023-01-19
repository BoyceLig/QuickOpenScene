using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace QuickOpenScene
{
    public class HttpUitls
    {
        UnityWebRequest www;
        public void GetJson()
        {
            StartBackgroundTask(StartRequest(Config.About.githubLatestAPI, () =>
            {
                var temp = JsonUtility.FromJson<GithubJsonData>(www.downloadHandler.text);
                if (temp != null)
                {
                    Config.LatestVersion = temp.tag_name;
                    Config.latestDownloadURL = temp.assets[0].browser_download_url;
                }
            }));
        }

        IEnumerator StartRequest(string url, Action success = null)
        {
            using (www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                while (www.isDone == false)
                    yield return null;

                if (success != null)
                    success();
            }
        }

        public static void StartBackgroundTask(IEnumerator update, Action end = null)
        {
            EditorApplication.CallbackFunction closureCallback = null;

            closureCallback = () =>
            {
                try
                {
                    if (update.MoveNext() == false)
                    {
                        if (end != null)
                            end();
                        EditorApplication.update -= closureCallback;
                    }
                }
                catch (Exception ex)
                {
                    if (end != null)
                        end();
                    Debug.LogException(ex);
                    EditorApplication.update -= closureCallback;
                }
            };
            EditorApplication.update += closureCallback;
        }
    }
}
