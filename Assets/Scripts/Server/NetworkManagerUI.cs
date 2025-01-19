using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    private string connectionStatus = ""; // �ڑ��󋵂̃��b�Z�[�W
    public string hostIp = ""; // �����l�����[�v�o�b�N�A�h���X�ɐݒ�
    public int port = 7777; // �g�p����|�[�g�ԍ�

    private void OnGUI()
    {
        // �G���A���w��
        GUILayout.BeginArea(new Rect(10, 10, 300, 300)); // ��300�A����300�̃G���A���쐬

        // �{�^�����\����������
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            // �{�^���̕��ƍ������w��
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

        // �ڑ��󋵂�\��
        GUILayout.Label($"Status: {connectionStatus}", GUILayout.Width(250), GUILayout.Height(50));

        // �ڑ����IP�A�h���X����͂ł���t�B�[���h��ǉ�
        GUILayout.Label("Host IP Address:", GUILayout.Width(250), GUILayout.Height(30));
        hostIp = GUILayout.TextField(hostIp, GUILayout.Width(200));

        GUILayout.EndArea();
    }

    private void StartHost()
    {
        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(hostIp, (ushort)port); // �z�X�g�����[�v�o�b�N�A�h���X���g�p
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
        unityTransport.SetConnectionData(hostIp, (ushort)port); // �T�[�o�[�����[�v�o�b�N�A�h���X���g�p
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
        yield return new WaitForSeconds(1f); // 1�b�ҋ@

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(hostIp, (ushort)port); // ���͂��ꂽIP�A�h���X���g�p

        // �ڑ��󋵂�������
        connectionStatus = $"Connecting to Host at {hostIp}:{port}...";

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected; // �G���[���̃R�[���o�b�N�ݒ�

        NetworkManager.Singleton.StartClient();
        Debug.Log($"Connecting to Host at {hostIp}:{port}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // �N���C�A���g�ؒf���̏����i�G���[����\���j
        connectionStatus = "Connection failed. Please check the host address and try again.";
        Debug.LogError("Client disconnected. Connection failed.");

        // �R�[���o�b�N�������i�s�v�ȌĂяo����h�����߁j
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
