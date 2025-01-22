using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public TMP_Dropdown hostIPDropDown; // IP�A�h���X��I������h���b�v�_�E��
    public TextMeshProUGUI connectionStatusText; // �ڑ��󋵂�\������t�B�[���h

    public Button hostButton; // Host�{�^��
    public Button clientButton; // Client�{�^��
    public Button serverButton; // Server�{�^��

    public int port = 7777; // �g�p����|�[�g�ԍ�

    private void Start()
    {
        // �������b�Z�[�W��ݒ�
        UpdateConnectionStatus("Waiting for input...");

        // �{�^���C�x���g�̓o�^
        if (hostButton != null)
            hostButton.onClick.AddListener(StartHost);

        if (clientButton != null)
            clientButton.onClick.AddListener(StartClient);

        if (serverButton != null)
            serverButton.onClick.AddListener(StartServer);
    }

    private void UpdateConnectionStatus(string status)
    {
        if (connectionStatusText != null)
        {
            connectionStatusText.text = status;
        }
       // Debug.Log(status);
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("Host or Client is already running. Cannot start a new Host.");
            return; // ���Ƀz�X�g��N���C�A���g���N�����Ă���ꍇ�A�������I��
        }

        if (!ValidateNetworkSetup()) return;

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(GetSelectedIP(), (ushort)port);

        if (NetworkManager.Singleton.StartHost())
        {
            UpdateConnectionStatus($"Host started at {GetSelectedIP()}:{port}");
            Debug.Log("Host started successfully.");
        } else
        {
            UpdateConnectionStatus("Failed to start host.");
        }
    }

    public void StartClient()
    {
        if (!ValidateNetworkSetup()) return;

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(GetSelectedIP(), (ushort)port);

        UpdateConnectionStatus($"Connecting to host at {GetSelectedIP()}:{port}...");

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        if (!NetworkManager.Singleton.StartClient())
        {
            UpdateConnectionStatus("Failed to start client.");
        }
    }

    private bool ConfigureTransport()
    {
        if (!ValidateNetworkSetup()) return false;

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(GetSelectedIP(), (ushort)port);
        return true;
    }


    public void StartServer()
    {
        if (!ConfigureTransport()) return;

        if (NetworkManager.Singleton.StartServer())
        {
            UpdateConnectionStatus($"Server started at {GetSelectedIP()}:{port}");
        } else
        {
            UpdateConnectionStatus("Failed to start server.");
        }
    }



    private void OnClientConnected(ulong clientId)
    {
        UpdateConnectionStatus($"Connected to host at {GetSelectedIP()}:{port}");
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdateConnectionStatus("Connection failed. Please check the host address and try again.");
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }



    public IEnumerator DelayedStartClient()
    {
        if (!ValidateNetworkSetup()) yield break;

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(GetSelectedIP(), (ushort)port);

        UpdateConnectionStatus($"Connecting to host at {GetSelectedIP()}:{port}...");

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        NetworkManager.Singleton.StartClient();

        yield return new WaitForSeconds(1f);
    }


    private bool ValidateNetworkSetup()
    {
        if (hostIPDropDown == null || hostIPDropDown.options.Count == 0)
        {
            Debug.LogError("Host IP Dropdown is not assigned or empty!");
            UpdateConnectionStatus("Host IP Dropdown is not assigned or empty!");
            return false;
        }

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is missing in the scene!");
            UpdateConnectionStatus("NetworkManager is missing in the scene!");
            return false;
        }

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (unityTransport == null)
        {
            Debug.LogError("UnityTransport component is missing from NetworkManager!");
            UpdateConnectionStatus("UnityTransport component is missing!");
            return false;
        }

        return true;
    }

    private string GetSelectedIP()
    {
        return hostIPDropDown.options[hostIPDropDown.value].text;
    }
}
