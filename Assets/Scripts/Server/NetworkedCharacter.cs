using Unity.Netcode;
using UnityEngine;

public struct AnimationValue : INetworkSerializable
{
    public int Speed;
    public int Jump;
    public int Suikomi;

    public AnimationValue(int speed, int jump, int suikomi)
    {
        Speed = speed;
        Jump = jump;
        Suikomi = suikomi;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Speed);
        serializer.SerializeValue(ref Jump);
        serializer.SerializeValue(ref Suikomi);
    }
}

public struct MyCustomModel : INetworkSerializable
{
    public Vector3 position;
    public bool isOnGround;
    public AnimationValue animationValue;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref isOnGround);
        serializer.SerializeValue(ref animationValue);
    }
}


public class NetworkedCharacter : NetworkBehaviour
{
    private NetworkVariable<MyCustomModel> networkData = new NetworkVariable<MyCustomModel>(
        new MyCustomModel()
        {
            position = Vector3.zero,
            isOnGround = false,
            animationValue = new AnimationValue(0, 0, 0)
        },
        NetworkVariableReadPermission.Everyone
    );

    public const int speed = 5;
    public float jumpForce = 5f;

    private bool isGround = false; // 地面に接しているかどうか
    private bool isHovering = false;
    private Animator anim;         // アニメーター
    private Rigidbody2D rb;        // Rigidbody2D 
    public const float normalGravityScale = 1.0f;// 通常の重力スケール
    public const float hoverGravityScale = 0.5f;//jumpが２のホバリングモードの時

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsOwner)
        {
            HandleMovement(); // 座標変更処理を追加
            MyCustomModel data = networkData.Value;
            data.position = transform.position;
            data.isOnGround = isGround; // 地面の状態を反映
            data.animationValue = GetAnimationValue();
            networkData.Value = data;

            // アニメーションの更新をホスト側でも適用
            UpdateAnimation(data.animationValue);
        } else
        {
            transform.position = networkData.Value.position; // 座標反映処理
            UpdateAnimation(networkData.Value.animationValue);
        }
    }


    private void HandleMovement()
    {
        // 入力を取得
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // 右向き
            xSpeed = speed;
        } else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // 左向き
            xSpeed = -speed;
        }

        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // ジャンプ力を適用
        }
        else if (Input.GetKeyDown(KeyCode.Space)|| isHovering)
        {
            isHovering = true;
            rb.gravityScale = hoverGravityScale; // ホバリング中の重力を設定
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // ジャンプまたはホバリングの動きを適用
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        // 横移動の速度を設定
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"衝突したオブジェクトのタグ: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            isHovering = false;
            rb.gravityScale = normalGravityScale; // 重力をリセット
            Debug.Log("地面に着地");
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"離れたオブジェクトのタグ: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
            Debug.Log("地面から離れた");
        }
    }


    private AnimationValue GetAnimationValue()
    {
        AnimationValue animationValue = new AnimationValue();

        // 移動状態の判定
        float horizontalKey = Input.GetAxis("Horizontal");
        if (horizontalKey != 0)
        {
            animationValue.Speed = 1; // 移動中
        } else
        {
            animationValue.Speed = 0; // 停止中
        }

        // ジャンプ状態の判定
        if (!isGround)
        {
            if (isHovering)
            {
                animationValue.Jump = 2; // 空中ホバリング中
            } else
            {
                animationValue.Jump = 1; // 空中にいる
            }
        } else
        {
            animationValue.Jump = 0; // 地面にいる
        }

        // デバッグ出力
        if (animationValue.Jump >=1)
        {
           // Debug.Log($"[GetAnimationValue] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}");
        }
       
        // 吸い込みなどの特殊状態（必要に応じて）
        animationValue.Suikomi = 0;

        return animationValue;
    }


    private void UpdateAnimation(AnimationValue animationValue)
    {
        anim.SetInteger("Speed", animationValue.Speed);
        anim.SetInteger("Jump", animationValue.Jump);
        anim.SetInteger("Suikomi", animationValue.Suikomi);

        // デバッグ出力
        if (anim.GetInteger("Jump") == 1)
        {
             Debug.Log($"[ジャンプ中] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}, Suikomi: {animationValue.Suikomi}");
        } else if (anim.GetInteger("Jump") == 2)
        {
            Debug.Log($"[ホバリングモード中] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}, Suikomi: {animationValue.Suikomi}");
        } else
        {
            Debug.Log($"[歩行中] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}, Suikomi: {animationValue.Suikomi}");
        }
    }

}


