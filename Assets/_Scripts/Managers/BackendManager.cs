using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BackendManager : Singleton<BackendManager>
{
    public const string GameDataURL = "https://jobfair.nordeus.com/jf24-fullstack-challenge/test";

    #region Generic Web Requests
    private IEnumerator WebRequest(string url, string json, string method, Action<string> callback = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(json))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"{method} data error: {request.error}");
                callback?.Invoke(null);
            }
            else
            {
                Debug.Log($"{method} data successfully! Response: {request.downloadHandler.text}");
                callback?.Invoke(request.downloadHandler.text);
            }
        }
    }

    private IEnumerator PostRequest(string url, string json, Action<string> callback = null)
        => WebRequest(url, json, "POST", callback);

    private IEnumerator GetRequest(string url, Action<string> callback)
        => WebRequest(url, null, "GET", callback);
    #endregion

    public void LoadGameData(Action<string> callback) => StartCoroutine(LoadGameDataCoroutine(callback));

    private IEnumerator LoadGameDataCoroutine(Action<string> callback)
    {
        yield return GetRequest(GameDataURL, callback);
    }
}
