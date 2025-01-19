using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameServerClient : MonoBehaviour
{
    [SerializeField] private string serverUrl = "https://fcc123.xsrv.jp/test_higashiyama/Unity_Server/"; // PHP�T�[�o��URL

    private void Start()
    {
        SendDataToServer();
    }

    // �{�^������Ăяo���Ȃǂ��ĒʐM�����s
    public void SendDataToServer()
    {
        StartCoroutine(SendRequest());
    }

    private IEnumerator SendRequest()
    {
        // ���M�f�[�^
        var postData = new
        {
            playerName = "UnityPlayer",
            score = 100
        };

        string jsonData = JsonUtility.ToJson(postData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(serverUrl, jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();

            // �T�[�o�֑��M
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            } else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
