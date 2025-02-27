using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkedCharacter : NetworkBehaviour
{

    public const int speed = 5; // 通常の移動速度
    public float jumpForce = 5f; // ジャンプ力
    public float hobaringForce = 4f; // ホバリング力
    public float dashSpeedMultiplier = 2f; // ダッシュ時の速度倍率
    public int dashDuration = 10; // ダッシュの持続時間フレーム数

    public const float hoverGravityScale = 0.5f; // ホバリングモード時の重力スケール
    public const float normalGravityScale = 1f; // 通常時の重力スケール

    private bool isGround = false; // 地面にいるかどうか

    private bool isFacingRight = true;


    #region -------------------【 ドア 】------------------
    private bool isTouchingDoor = false; // ドアと接触しているかどうかを示すフラグ
    private string Door_Name;
    #endregion ----------------------------------------------


    private Animator anim = null;
    private Rigidbody2D rb = null;

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

        // サーバーに移動リクエストを送信
        RequestMoveServerRpc(xSpeed, isFacingRight);

        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            RequestJumpServerRpc();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
           RequestHobaringServerRpc();
        }
    }

    [ServerRpc]
    private void RequestMoveServerRpc(float xSpeed, bool facingRight)
    {
        // サーバーで位置を更新
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);

        // クライアント全体にアニメーションと向きを同期
        UpdateAnimationAndFacingClientRpc(xSpeed != 0, facingRight);
    }

    [ServerRpc]
    private void RequestJumpServerRpc()
    {
        if (isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            UpdateJumpAnimationClientRpc();
        }
    }

    [ServerRpc]
    private void RequestHobaringServerRpc()
    {
        if (!isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, hobaringForce);
            UpdateHobaringAnimationClientRpc();
        }
    }

    [ClientRpc]
    private void UpdateAnimationAndFacingClientRpc(bool isMoving, bool facingRight)
    {
        if (!IsOwner)
        {
            anim.SetInteger("Speed", isMoving ? 1 : 0);

            // キャラクターの向きを反転
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        } else
        {
            // ローカルのキャラも向きを更新
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }
    }

    [ClientRpc]
    private void UpdateJumpAnimationClientRpc()
    {
        anim.SetInteger("Jump", 1);
    }

    [ClientRpc]
    private void UpdateHobaringAnimationClientRpc()
    {
        anim.SetInteger("Jump", 2);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            UpdateGroundStateServerRpc(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            UpdateGroundStateServerRpc(false);
        }
    }

    [ServerRpc]
    private void UpdateGroundStateServerRpc(bool isOnGround)
    {
        isGround = isOnGround;
        UpdateGroundStateClientRpc(isOnGround);
    }

    [ClientRpc]
    private void UpdateGroundStateClientRpc(bool isOnGround)
    {
        isGround = isOnGround;
        anim.SetInteger("Jump", isOnGround ? 0 : 1);
    }
}
