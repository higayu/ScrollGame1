using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NetworkManagerUI : MonoBehaviour
{
    public TMP_Dropdown hostIPDropDown;

    public TextMeshProUGUI connectionStatusText;

    public UnityEngine.UI.Button hostButton;
    public UnityEngine.UI.Button clientButton;
    public UnityEngine.UI.Button serverButton;

    private string connectionStatus = "";
    public int port = 7777;

    private void Start()
    {
        UpdateConnectionStatus("Waiting for input...");

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
        Debug.Log($"Connection Status Updated: {status}");
        if (connectionStatusText != null)
        {
            connectionStatusText.text = connectionStatus;
        }
    }

    public void StartHost()
    {
        try
        {
            if (hostIPDropDown == null)
            {
                Debug.LogError("Host IP Input Field is not assigned!");
                UpdateConnectionStatus("Host IP Input Field is missing!");
                return;
            }

            string hostIp = hostIPDropDown.options[hostIPDropDown.value].text;
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
                Debug.Log($"Host successfully started at {hostIp}:{port}");
            } else
            {
                UpdateConnectionStatus("Failed to start host.");
                Debug.LogError("Failed to start host.");
            }
        } catch (System.Exception ex)
        {
            Debug.LogError($"Exception in StartHost: {ex.Message}");
            UpdateConnectionStatus("Error occurred while starting the host.");
        }
    }

    public void StartServer()
    {
        try
        {
            if (hostIPDropDown == null)
            {
                Debug.LogError("Host IP Input Field is not assigned!");
                UpdateConnectionStatus("Host IP Input Field is missing!");
                return;
            }

            string hostIp = hostIPDropDown.options[hostIPDropDown.value].text;
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
                Debug.Log($"Server successfully started at {hostIp}:{port}");
            } else
            {
                UpdateConnectionStatus("Failed to start server.");
                Debug.LogError("Failed to start server.");
            }
        } catch (System.Exception ex)
        {
            Debug.LogError($"Exception in StartServer: {ex.Message}");
            UpdateConnectionStatus("Error occurred while starting the server.");
        }
    }

    public IEnumerator DelayedStartClient()
    {
        string hostIp = ""; // try の外で変数を宣言
        try
        {
            if (hostIPDropDown == null)
            {
                Debug.LogError("Host IP Input Field is not assigned!");
                UpdateConnectionStatus("Host IP Input Field is missing!");
                yield break;
            }

            // ドロップダウンから選択されたホストIPを取得
            hostIp = hostIPDropDown.options[hostIPDropDown.value].text;
            if (string.IsNullOrEmpty(hostIp))
            {
                Debug.LogError("Host IP is empty!");
                UpdateConnectionStatus("Please enter a valid Host IP.");
                yield break;
            }

            // UnityTransportコンポーネントを取得
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (unityTransport == null)
            {
                Debug.LogError("UnityTransport component is missing from NetworkManager!");
                UpdateConnectionStatus("UnityTransport component is missing!");
                yield break;
            }

            // 接続データを設定
            unityTransport.SetConnectionData(hostIp, (ushort)port);

            UpdateConnectionStatus($"Connecting to Host at {hostIp}:{port}...");
            Debug.Log($"Attempting to connect to Host at {hostIp}:{port}");

            // クライアント切断時のコールバックを登録
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            // クライアント接続を開始
            NetworkManager.Singleton.StartClient();



            // 接続状態を確認
            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                Debug.LogWarning("Connection attempt timed out or failed.");
                UpdateConnectionStatus($"Connection to {hostIp}:{port} timed out. Check the host address.");
            } else
            {
                Debug.Log("Client successfully connected to the server.");
                UpdateConnectionStatus("Client connected successfully!");
            }
        } catch (System.Exception ex)
        {
            // エラーメッセージの出力
            Debug.LogError($"Exception in DelayedStartClient: {ex.Message}");
            UpdateConnectionStatus($"Error occurred while connecting to the host: {hostIp}");
        } finally
        {
            // コールバックを解除してメモリリークを防止
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }


    private void OnClientDisconnected(ulong clientId)
    {
        UpdateConnectionStatus("Connection failed. Please check the host address and try again.");
        Debug.LogError("Client disconnected. Connection failed.");
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
