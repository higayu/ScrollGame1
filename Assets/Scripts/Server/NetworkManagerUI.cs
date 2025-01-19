using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public TMP_InputField hostIPInputField; // IP�A�h���X����͂���t�B�[���h
    public TextMeshProUGUI connectionStatusText; // �ڑ��󋵂�\������t�B�[���h

    public UnityEngine.UI.Button hostButton; // Host�{�^��
    public UnityEngine.UI.Button clientButton; // Client�{�^��
    public UnityEngine.UI.Button serverButton; // Server�{�^��

    private string connectionStatus = ""; // �ڑ��󋵂̃��b�Z�[�W
    public int port = 7777; // �g�p����|�[�g�ԍ�

    public GameObject playerPrefab; // �v���C���[�v���n�u

    private void Start()
    {
        // �������b�Z�[�W��ݒ�
        UpdateConnectionStatus("Waiting for input...");

        // �{�^���C�x���g�̓o�^
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

        string hostIp = hostIPInputField.text; // InputField����IP���擾
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

            // �z�X�g�p�v���C���[���X�|�[��
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

        string hostIp = hostIPInputField.text; // InputField����IP���擾
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

        string hostIp = hostIPInputField.text; // InputField����IP���擾
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

        yield return new WaitForSeconds(1f); // 1�b�ҋ@
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

        // �l�b�g���[�N�v���C���[���X�|�[��
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().Spawn();
    }
}
