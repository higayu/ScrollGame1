using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    private string connectionStatus = ""; // 接続状況のメッセージ
    public string hostIp = ""; // 初期値をループバックアドレスに設定
    public int port = 7777; // 使用するポート番号

    private void OnGUI()
    {
        // エリアを指定
        GUILayout.BeginArea(new Rect(10, 10, 300, 300)); // 幅300、高さ300のエリアを作成

        // ボタンが表示される条件
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            // ボタンの幅と高さを指定
            if (GUILayout.Button("Start Host", GUILayout.Width(200), GUILayout.Height(50)))
            {
                StartHost();
            }
            if (GUILayout.Button("Start Client", GUILayout.Width(200), GUILayout.Height(50)))
            {
                StartCoroutine(DelayedStartClient());
            }

            if (GUILayout.Button("Start Server", GUILayout.Width(200), GUILayout.Height(50)))
            {
                StartServer();
            }
        }

        // 接続状況を表示
        GUILayout.Label($"Status: {connectionStatus}", GUILayout.Width(250), GUILayout.Height(50));

        // 接続先のIPアドレスを入力できるフィールドを追加
        GUILayout.Label("Host IP Address:", GUILayout.Width(250), GUILayout.Height(30));
        hostIp = GUILayout.TextField(hostIp, GUILayout.Width(200));

        GUILayout.EndArea();
    }

    private void StartHost()
    {
        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(hostIp, (ushort)port); // ホストもループバックアドレスを使用
        if (NetworkManager.Singleton.StartHost())
        {
            connectionStatus = $"Host started at hostIp:{port}";
            Debug.Log(connectionStatus);
        } else
        {
            connectionStatus = "Failed to start host.";
            Debug.LogError(connectionStatus);
        }
    }

    private void StartServer()
    {
        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(hostIp, (ushort)port); // サーバーもループバックアドレスを使用
        if (NetworkManager.Singleton.StartServer())
        {
            connectionStatus = $"Server started at hostIp:{port}";
            Debug.Log(connectionStatus);
        } else
        {
            connectionStatus = "Failed to start server.";
            Debug.LogError(connectionStatus);
        }
    }

    IEnumerator DelayedStartClient()
    {
        yield return new WaitForSeconds(1f); // 1秒待機

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(hostIp, (ushort)port); // 入力されたIPアドレスを使用

        // 接続状況を初期化
        connectionStatus = $"Connecting to Host at {hostIp}:{port}...";

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected; // エラー時のコールバック設定

        NetworkManager.Singleton.StartClient();
        Debug.Log($"Connecting to Host at {hostIp}:{port}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // クライアント切断時の処理（エラー情報を表示）
        connectionStatus = "Connection failed. Please check the host address and try again.";
        Debug.LogError("Client disconnected. Connection failed.");

        // コールバックを解除（不要な呼び出しを防ぐため）
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
