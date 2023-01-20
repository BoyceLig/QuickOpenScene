using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace QuickOpenScene
{
    public class GetVersionInformation : Config
    {
        UnityWebRequest request;
        public void GetJson(bool displayUpdateMessage = false)
        {
            StartBackgroundTask(StartRequest(URL.githubLatestAPI, () =>
            {
                if (!request.downloadHandler.text.Contains("message"))
                {
                    var temp = JsonUtility.FromJson<GithubJsonData>(request.downloadHandler.text);
                    if (temp != null)
                    {
                        LatestVersion = temp.tag_name;
                        LatestDownloadURL = temp.assets[0].browser_download_url;
                        IsDown = true;
                        if (displayUpdateMessage)
                        {
                            Debug.Log($"检查完成，当前版本：{currVersion} ,最新版本：{LatestVersion} ，{(NeedUpdate ? "需要更新,可自行选择更新。" : "不需要更新。")}");
                            //Debug.Log($"预计{UnixToDateTime(long.Parse(request.GetResponseHeader("x-ratelimit-reset")))} 重置请求次数为5000次。当前请求次数剩余：{request.GetResponseHeader("x-ratelimit-remaining")}次");
                        }
                    }
                }
            }));
        }

        IEnumerator StartRequest(string url, Action action = null)
        {
            var uri = new Uri(url);
            request = UnityWebRequest.Get(uri);
            request.SetRequestHeader("Authorization", "token " + GetHead()); ;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                while (!request.isDone)
                {
                    yield return null;
                }

                if (action != null)
                {
                    action();
                }
            }
        }

        public static void StartBackgroundTask(IEnumerator update, Action end = null)
        {
            EditorApplication.CallbackFunction closureCallback = null;

            closureCallback = () =>
            {
                try
                {
                    if (!update.MoveNext())
                    {
                        if (end != null)
                        {
                            end();
                        }
                        EditorApplication.update -= closureCallback;
                    }
                }
                catch (Exception ex)
                {
                    if (end != null)
                    {
                        end();
                    }
                    Debug.LogException(ex);
                    EditorApplication.update -= closureCallback;
                }
            };
            EditorApplication.update += closureCallback;
        }

        public string UnixToDateTime(long unixTimeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt.ToString("HH:mm:ss");
        }
    }
}
