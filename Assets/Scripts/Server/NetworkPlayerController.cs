using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkPlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // �v���C���[�̈ړ����x
    private Vector3 targetPosition; // �T�[�o�[���瓯�����ꂽ�ڕW�ʒu

    private void Start()
    {
        if (!IsOwner)
        {
            // �T�[�o�[�����ʒu��������
            targetPosition = transform.position;
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            HandleMovement();
        } else
        {
            SmoothMove();
        }
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;

        // �T�[�o�[�Ɉړ����N�G�X�g�𑗐M
        RequestMoveServerRpc(move);
    }

    [ServerRpc]
    private void RequestMoveServerRpc(Vector3 move)
    {
        transform.position += move;

        // �N���C�A���g�S�̂Ɉʒu�𔽉f
        UpdatePositionClientRpc(transform.position);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        // �T�[�o�[���瑗���Ă����ʒu��ڕW�ʒu�Ƃ��Đݒ�
        targetPosition = position;
    }

    private void SmoothMove()
    {
        // ���݈ʒu���T�[�o�[���瑗���Ă����ڕW�ʒu�ɕ��
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
    }
}
