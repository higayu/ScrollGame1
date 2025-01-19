using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkPlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // プレイヤーの移動速度
    private Vector3 targetPosition; // サーバーから同期された目標位置

    private void Start()
    {
        if (!IsOwner)
        {
            // サーバー同期位置を初期化
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

        // サーバーに移動リクエストを送信
        RequestMoveServerRpc(move);
    }

    [ServerRpc]
    private void RequestMoveServerRpc(Vector3 move)
    {
        transform.position += move;

        // クライアント全体に位置を反映
        UpdatePositionClientRpc(transform.position);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        // サーバーから送られてきた位置を目標位置として設定
        targetPosition = position;
    }

    private void SmoothMove()
    {
        // 現在位置をサーバーから送られてきた目標位置に補間
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
    }
}
