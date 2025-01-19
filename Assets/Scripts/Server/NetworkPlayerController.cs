using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkPlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // �v���C���[�̈ړ����x

    private void Update()
    {
        // �����̃v���C���[�̂ݐ���\
        if (IsOwner)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        // �L�[�{�[�h���͂ňړ�
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.position += move;

        // �T�[�o�[�Ɉʒu�𓯊�
        UpdatePositionServerRpc(transform.position);
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 position)
    {
        // �T�[�o�[�ňʒu���X�V
        transform.position = position;

        // �N���C�A���g�S�̂Ɉʒu�𔽉f
        UpdatePositionClientRpc(position);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        // ���̃N���C�A���g�ɂ݈̂ʒu�𔽉f
        if (!IsOwner)
        {
            transform.position = position;
        }
    }
}
