using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public TMP_InputField hostIPInputField; // IPアドレスを入力するフィールド
    public TextMeshProUGUI connectionStatusText; // 接続状況を表示するフィールド

    public UnityEngine.UI.Button hostButton; // Hostボタン
    public UnityEngine.UI.Button clientButton; // Clientボタン
    public UnityEngine.UI.Button serverButton; // Serverボタン

    private string connectionStatus = ""; // 接続状況のメッセージ
    public int port = 7777; // 使用するポート番号

    public GameObject playerPrefab; // プレイヤープレハブ

    private void Start()
    {
        // 初期メッセージを設定
        UpdateConnectionStatus("Waiting for input...");

        // ボタンイベントの登録
        if (hostButton != null)
            hostButton.onClick.AddListener(StartHost);

        if (clientButton != null)
            clientButton.onClick.AddListener(() => StartCoroutine(DelayedStartClient()));

        if (serverButton != null)
            serverButton.onClick.AddListener(StartServer);
    }

    private void UpdateConnectionStatus(string status)
    {
        connectionStatus = status;
        if (connectionStatusText != null)
        {
            connectionStatusText.text = connectionStatus;
        }
    }

    public void StartHost()
    {
        if (hostIPInputField == null)
        {
            Debug.LogError("Host IP Input Field is not assigned!");
            UpdateConnectionStatus("Host IP Input Field is missing!");
            return;
        }

        string hostIp = hostIPInputField.text; // InputFieldからIPを取得
        if (string.IsNullOrEmpty(hostIp))
        {
            Debug.LogError("Host IP is empty!");
            UpdateConnectionStatus("Please enter a valid Host IP.");
            return;
        }

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (unityTransport == null)
        {
            Debug.LogError("UnityTransport component is missing from NetworkManager!");
            UpdateConnectionStatus("UnityTransport component is missing!");
            return;
        }

        unityTransport.SetConnectionData(hostIp, (ushort)port);

        if (NetworkManager.Singleton.StartHost())
        {
            UpdateConnectionStatus($"Host started at {hostIp}:{port}");
            Debug.Log(connectionStatus);

            // ホスト用プレイヤーをスポーン
            SpawnPlayer();
        } else
        {
            UpdateConnectionStatus("Failed to start host.");
            Debug.LogError(connectionStatus);
        }
    }

    public void StartServer()
    {
        if (hostIPInputField == null)
        {
            Debug.LogError("Host IP Input Field is not assigned!");
            UpdateConnectionStatus("Host IP Input Field is missing!");
            return;
        }

        string hostIp = hostIPInputField.text; // InputFieldからIPを取得
        if (string.IsNullOrEmpty(hostIp))
        {
            Debug.LogError("Host IP is empty!");
            UpdateConnectionStatus("Please enter a valid Host IP.");
            return;
        }

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (unityTransport == null)
        {
            Debug.LogError("UnityTransport component is missing from NetworkManager!");
            UpdateConnectionStatus("UnityTransport component is missing!");
            return;
        }

        unityTransport.SetConnectionData(hostIp, (ushort)port);

        if (NetworkManager.Singleton.StartServer())
        {
            UpdateConnectionStatus($"Server started at {hostIp}:{port}");
            Debug.Log(connectionStatus);
        } else
        {
            UpdateConnectionStatus("Failed to start server.");
            Debug.LogError(connectionStatus);
        }
    }

    public IEnumerator DelayedStartClient()
    {
        if (hostIPInputField == null)
        {
            Debug.LogError("Host IP Input Field is not assigned!");
            UpdateConnectionStatus("Host IP Input Field is missing!");
            yield break;
        }

        string hostIp = hostIPInputField.text; // InputFieldからIPを取得
        if (string.IsNullOrEmpty(hostIp))
        {
            Debug.LogError("Host IP is empty!");
            UpdateConnectionStatus("Please enter a valid Host IP.");
            yield break;
        }

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (unityTransport == null)
        {
            Debug.LogError("UnityTransport component is missing from NetworkManager!");
            UpdateConnectionStatus("UnityTransport component is missing!");
            yield break;
        }

        unityTransport.SetConnectionData(hostIp, (ushort)port);

        UpdateConnectionStatus($"Connecting to Host at {hostIp}:{port}...");
        Debug.Log($"Connecting to Host at {hostIp}:{port}");

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        NetworkManager.Singleton.StartClient();

        yield return new WaitForSeconds(1f); // 1秒待機
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdateConnectionStatus("Connection failed. Please check the host address and try again.");
        Debug.LogError("Client disconnected. Connection failed.");
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned!");
            UpdateConnectionStatus("Player prefab is not assigned!");
            return;
        }

        // ネットワークプレイヤーをスポーン
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().Spawn();
    }
}
