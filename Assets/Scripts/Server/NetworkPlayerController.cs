using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkPlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // プレイヤーの移動速度

    private void Update()
    {
        // 自分のプレイヤーのみ制御可能
        if (IsOwner)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        // キーボード入力で移動
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.position += move;

        // サーバーに位置を同期
        UpdatePositionServerRpc(transform.position);
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 position)
    {
        // サーバーで位置を更新
        transform.position = position;

        // クライアント全体に位置を反映
        UpdatePositionClientRpc(position);
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        // 他のクライアントにのみ位置を反映
        if (!IsOwner)
        {
            transform.position = position;
        }
    }
}
