using System;
using System.Collections;
using System.Net.Http.Headers;
using System.Net.Http;
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
                    Config.LatestDownloadURL = temp.assets[0].browser_download_url;
                    Config.IsDown = true;
                    Version currVersion = new Version(Config.currVersion);
                    Version latestVersion = new Version(Config.LatestVersion);
                    if (currVersion < latestVersion)
                    {
                        Config.NeedUpdate = true;
                    }
                }
            }));
        }

        static async void DownloadJson()
        {
            Uri uri = new Uri(Config.About.githubLatestAPI);
            HttpClient request = new HttpClient();
            ProductHeaderValue userAgent = new ProductHeaderValue("QuickOpenScene", "1.0.0");
            ProductInfoHeaderValue userAgentInfoHeader = new ProductInfoHeaderValue(userAgent);
            request.DefaultRequestHeaders.UserAgent.Add(userAgentInfoHeader);
            request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "OAUTH-TOKEN"); ;
            request.Timeout = TimeSpan.FromSeconds(5);
            HttpResponseMessage response = await request.GetAsync(uri);
            string responseText = await response.Content.ReadAsStringAsync();
        }




        IEnumerator StartRequest(string url, Action success = null)
        {
            using (www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Content-Type", "application/json");
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
