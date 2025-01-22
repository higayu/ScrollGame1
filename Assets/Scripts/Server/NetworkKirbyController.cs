using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NetworkKirbyController : NetworkBehaviour
{
    public const int speed = 5;
    public float jumpForce = 5f;
    public float dashSpeedMultiplier = 2f;
    public int dashDuration = 10;

    public const float hoverGravityScale = 0.5f;
    public const float normalGravityScale = 1f;

    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false;
    private float realSpeed = speed;

    public Vector2 boxSize = new Vector2(2f, 1f);
    public float suikomiForce = 0.1f;
    private bool isSucking = false;
    private bool isHoubaru = false;
    public float closeDistance = 1f;
    private int Suikomi_Count = 0;

    private bool isFacingRight = true;

    public AudioSource audioSource;
    public AudioClip suikomiSound;
    public AudioClip hobaringSound;
    public AudioClip starSound;

    public float power = 100f;
    public GameObject cannonBall;
    public Transform shootPoint;

    private bool isTouchingDoor = false;
    private string Door_Name;
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();
    public NetworkVariable<bool> IsFacingRight = new NetworkVariable<bool>();

    public NetworkVariable<float> NetworkSpeed = new NetworkVariable<float>();
    public NetworkVariable<int> NetworkJumpState = new NetworkVariable<int>();


    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (anim == null || rb == null)
        {
            Debug.LogError("�K�v�ȃR���|�[�l���g��������܂���B");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource�R���|�[�l���g��������܂���B");
        }
    }


    private void Update()
    {
        if (IsOwner)
        {
            // �I�[�i�[�͓��͂��������A�ʒu���T�[�o�[�ɑ��M
            HandleInput();
            Position.Value = rb.position; // �T�[�o�[���ňʒu�𓯊�
            IsFacingRight.Value = isFacingRight;

            // �T�[�o�[�ɃA�j���[�V������Ԃ𑗐M
            UpdateAnimationStateServerRpc(anim.GetFloat("Speed"), anim.GetInteger("Jump"));
        } else
        {
            // �N���C�A���g�̓l�b�g���[�N�ϐ�����ʒu�ƌ����𔽉f
            rb.position = Position.Value; // �l�b�g���[�N�œ������ꂽ�ʒu��K�p
            transform.localScale = new Vector3(IsFacingRight.Value ? 1 : -1, 1, 1);
        }

        // �n�ʂɐڂ��Ă��Ȃ��ꍇ�̓W�����v�܂��͗����A�j���[�V������ݒ�
        if (!isGround && anim.GetInteger("Jump") != 2) // �z�o�����O���łȂ��ꍇ
        {
            anim.SetInteger("Jump", 1); // �W�����v�܂��͗�����Ԃɐݒ�
        }
    }


    private void HandleInput()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            isFacingRight = true;
            xSpeed = realSpeed;
        } else if (horizontalKey < 0)
        {
            isFacingRight = false;
            xSpeed = -realSpeed;
        }

        RequestMoveServerRpc(xSpeed, isFacingRight);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {
                RequestJumpServerRpc(false);
            } else if (!isHoubaru)
            {
                RequestJumpServerRpc(true);
            }
        }

        if (Input.GetKey(KeyCode.S) && !isHoubaru)
        {
            RequestSuctionServerRpc(true);
        } else if (Input.GetKeyUp(KeyCode.S))
        {
            RequestSuctionServerRpc(false);
        }

        if (Input.GetKeyDown(KeyCode.V) && isHoubaru)
        {
            RequestShootServerRpc(isFacingRight);
        }
    }

    [ServerRpc]
    private void RequestMoveServerRpc(float xSpeed, bool facingRight)
    {
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);
        UpdateAnimationAndFacingClientRpc(xSpeed != 0, facingRight);
    }

    [ServerRpc]
    private void RequestJumpServerRpc(bool isHovering)
    {
        if (isHovering)
        {
            rb.gravityScale = hoverGravityScale; // �z�o�����O���̏d�͂�ݒ�
            anim.SetInteger("Jump", 2); // �z�o�����O�̃A�j���[�V����
        } else
        {
            rb.gravityScale = normalGravityScale; // �ʏ�̏d��
            anim.SetInteger("Jump", 1); // �W�����v�̃A�j���[�V����
        }

        // �W�����v�܂��̓z�o�����O�̓�����K�p
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }


    [ServerRpc]
    private void RequestSuctionServerRpc(bool isSuction)
    {
        if (isSuction)
        {
            SuikomiStarBlock();
            UpdateSuctionAnimationClientRpc(true);
        } else
        {
            UpdateSuctionAnimationClientRpc(false);
        }
    }

    [ServerRpc]
    private void RequestShootServerRpc(bool facingRight)
    {
        Shoot(facingRight);
    }

    [ClientRpc]
    private void UpdateAnimationAndFacingClientRpc(bool isMoving, bool facingRight)
    {
        if (anim == null)
        {
            Debug.LogError("Animator is null. Please ensure the Animator component is attached.");
            return;
        }

        anim.SetInteger("Speed", isMoving ? 1 : 0);
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdateAnimationStateServerRpc(float speed, int jumpState)
    {
        NetworkSpeed.Value = speed;
        NetworkJumpState.Value = jumpState;
    }


    [ClientRpc]
    private void UpdateJumpAnimationClientRpc(bool isHovering)
    {
        anim.SetInteger("Jump", isHovering ? 2 : 1); // 2: �z�o�����O, 1: �ʏ�W�����v
    }

    private void UpdateAnimationAndFacing(int speed, int jumpState, bool facingRight)
    {
        anim.SetInteger("Speed", speed);
        anim.SetInteger("Jump", jumpState);
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }



    [ClientRpc]
    private void UpdateSuctionAnimationClientRpc(bool isSuction)
    {
        anim.SetInteger("suikomi", isSuction ? 1 : 0);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            anim.SetInteger("Jump", 0); // �n��A�j���[�V�����Ƀ��Z�b�g
            rb.gravityScale = normalGravityScale; // �d�͂����Z�b�g
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
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
        if (isOnGround)
        {
            anim.SetInteger("Jump", 0);
            rb.gravityScale = normalGravityScale;
        }
    }

    private void SuikomiStarBlock()
    {
        Vector2 boxCenter = (Vector2)transform.position + (isFacingRight ? Vector2.right : Vector2.left) * 1f;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        foreach (Collider2D collider in colliders)
        {
            string[] SuikomiTags = { "Enemy1", "Enemy2", "StarBlock" };

            if (SuikomiTags.Contains(collider.tag))
            {
                Rigidbody2D targetRb = collider.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    Vector2 direction = (transform.position - collider.transform.position).normalized;
                    targetRb.velocity = direction * suikomiForce;

                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < closeDistance)
                    {
                        Destroy(collider.gameObject);
                        isHoubaru = true;
                        UpdateSuctionAnimationClientRpc(false);
                    }
                }
            }
        }
    }

    private void Shoot(bool isFacingRight)
    {
        GameObject newBullet = Instantiate(cannonBall, shootPoint.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody2D>().AddForce((isFacingRight ? Vector3.right : Vector3.left) * power);
    }
}
