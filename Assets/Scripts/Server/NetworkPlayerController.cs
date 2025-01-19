using UnityEngine;
using Unity.Netcode;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Globalization;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkPlayerController : NetworkBehaviour
{
    public const int speed = 5; // 通常の移動速度
    public float jumpForce = 5f; // ジャンプ力

    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false; // 地面にいるかどうか

    private bool isFacingRight = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Nullチェック
        if (anim == null)
        {
            Debug.LogError("Animatorコンポーネントが見つかりません。");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2Dコンポーネントが見つかりません。");
        }
    }

    private void Update()
    {
        // 自分のプレイヤーのみ入力を処理
        if (IsOwner)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            isFacingRight = true;
            anim.SetInteger("Speed", 1);
            xSpeed = speed;
        } else if (horizontalKey < 0)
        {
            isFacingRight = false;
            anim.SetInteger("Speed", 1);
            xSpeed = -speed;
        } else
        {
            anim.SetInteger("Speed", 0);
            xSpeed = 0.0f;
        }

        // ローカルでジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            anim.SetInteger("Jump", 1);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // サーバーに移動リクエストを送信
        RequestMoveServerRpc(xSpeed);
    }

    [ServerRpc]
    private void RequestMoveServerRpc(float xSpeed)
    {
        // サーバーで位置を更新
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);

        // クライアント全体にアニメーションと向きを同期
        UpdateAnimationAndFacingClientRpc(xSpeed != 0, isFacingRight);
    }

    [ClientRpc]
    private void UpdateAnimationAndFacingClientRpc(bool isMoving, bool facingRight)
    {
        if (!IsOwner)
        {
            anim.SetInteger("Speed", isMoving ? 1 : 0);
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            anim.SetInteger("Jump", 0);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
        }
    }
}
