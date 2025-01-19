using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    public float Speed = 5f;

    private void Update()
    {
        // クライアントが自身のキャラクターを操作
        if (IsOwner)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0) * Speed * Time.deltaTime;
        transform.position += movement;
    }
}
