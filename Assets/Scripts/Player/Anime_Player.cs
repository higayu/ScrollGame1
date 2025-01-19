using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Anime_Player : MonoBehaviour
{


    public const int speed = 5; // 通常の移動速度
    public float jumpForce = 5f; // ジャンプ力
    public float dashSpeedMultiplier = 2f; // ダッシュ時の速度倍率
    public int dashDuration = 10; // ダッシュの持続時間フレーム数

    public const float normalGravityScale = 1f; // 通常時の重力スケール

    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false; // 地面にいるかどうか
    private float realSpeed = speed; // 実際の速度

    
    private bool isFacingRight = true;

    public AudioSource audioSource;// AudioSourceコンポーネント


    #region -------------------【 ドア 】------------------
    private bool isTouchingDoor = false; // ドアと接触しているかどうかを示すフラグ
    private string Door_Name;
    #endregion ----------------------------------------------

    #region //------------------------------【startメソッド】---------------------------------------------//
    // Start is called before the first frame update
    void Start()
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

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSourceコンポーネントが見つかりません。");
        }

    }
    #endregion //---------------------------------------------------------------------------//

    // Update is called once per frame
    #region  ----------【 アップデートイベント 】---------------------------------------------
    void Update()
    {

        #region  ----------【 オブジェクトの有無 】---------------------------------------------
        if (anim == null || rb == null || audioSource == null || audioSource == null)
        {
            //Debug.Log("音源なし");
            //return; // 必要なコンポーネントが見つからない場合は処理を中断
        }
        #endregion  ----------【  】---------------------------------------------


        #region  ----------【 移動処理 】---------------------------------------------
        // 入力を取得
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            anim.SetInteger("Speed", 1);
            xSpeed = realSpeed;
            Debug.Log("右に移動");

        } else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            anim.SetInteger("Speed", 1);
            xSpeed = -(float)speed;
            Debug.Log("左に移動");
        } else
        {
            anim.SetInteger("Speed", 0);
            xSpeed = 0.0f;
            realSpeed = speed; // 停止時に速度を通常に戻す
        }

        isFacingRight = transform.localScale.x > 0;


        //Debug.Log("スピード : " + xSpeed);
        // 横移動の速度を設定
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);
        //Debug.Log("移動後の処理" + rb.velocity);

        #endregion  ----------【移動末尾 】---------------------------------------------


        #region  ----------【 ジャンプ処理 】---------------------------------------------
        // スペースキーが押された場合、かつキャラクターが地面にいる場合にジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            anim.SetInteger("Jump", 1);
           // Debug.Log("ジャンプ確定！！！ isGround: " + isGround);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        } else if (Input.GetKeyDown(KeyCode.Space) && !isGround)
        {

            Debug.Log(anim.GetInteger("Jump") + ": ジャンプGET");

        }
        else if(!isGround && anim.GetInteger("Jump") == 0)
        {
            anim.SetInteger("Jump", 1);
        }
        #endregion  ----------【 ジャンプ処理末尾】---------------------------------------------



        if (isTouchingDoor && Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("ドアの前で上矢印キー入力");
            // ドアに関連する処理を実行
            SoundEffect.Instance.Door_Sound();
            Door_Method();
        }
    }
    #endregion --------------------------------------------------------------------------

  
    #region---- NPCが入ったら吸引を開始する例
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy1"))
        {
            Enemy_AI enemy = other.GetComponent<Enemy_AI>();

        }


        if (other.CompareTag("Door"))
        {
            isTouchingDoor = true;
            // Door クラスを取得
            Door door = other.GetComponent<Door>();
            if (door != null)
            {
                Door_Name = door.Scene_name; // Door クラスのプロパティを使用
                Debug.Log("ドアのScene名"+ Door_Name);
            }
        }
    }
    #endregion ---------------------------------------------------------------------------------------

    #region ----ドアの当たり判定 ------
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            isTouchingDoor = false;
        }
    }
    #endregion

    public void Door_Method()
    {
        SceneManager.LoadScene(Door_Name);
    }

 
    //----------------------------------------------------------------------------------//

    #region //-----------------------------【地面の当たり判定】----------------------------------------------//
    // 地面に接触したときの処理
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            Debug.Log("地面に着地しました。");
            anim.SetInteger("Jump", 0);
            rb.gravityScale = normalGravityScale; 
        }
    }

    // 地面から離れたときの処理
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
            Debug.Log("地面から離れました。");
        }
    }
    #endregion //---------------------------------------------------------------------------//

}
