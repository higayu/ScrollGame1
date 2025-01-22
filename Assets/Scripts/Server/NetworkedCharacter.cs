using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkedCharacter : NetworkBehaviour
{

    public const int speed = 5; // �ʏ�̈ړ����x
    public float jumpForce = 5f; // �W�����v��
    public float hobaringForce = 4f; // �z�o�����O��
    public float dashSpeedMultiplier = 2f; // �_�b�V�����̑��x�{��
    public int dashDuration = 10; // �_�b�V���̎������ԃt���[����

    public const float hoverGravityScale = 0.5f; // �z�o�����O���[�h���̏d�̓X�P�[��
    public const float normalGravityScale = 1f; // �ʏ펞�̏d�̓X�P�[��

    private bool isGround = false; // �n�ʂɂ��邩�ǂ���

    private bool isFacingRight = true;


    #region -------------------�y �h�A �z------------------
    private bool isTouchingDoor = false; // �h�A�ƐڐG���Ă��邩�ǂ����������t���O
    private string Door_Name;
    #endregion ----------------------------------------------


    private Animator anim = null;
    private Rigidbody2D rb = null;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Null�`�F�b�N
        if (anim == null)
        {
            Debug.LogError("Animator�R���|�[�l���g��������܂���B");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D�R���|�[�l���g��������܂���B");
        }
    }

    private void Update()
    {
        // �����̃v���C���[�̂ݓ��͂�����
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

        // �T�[�o�[�Ɉړ����N�G�X�g�𑗐M
        RequestMoveServerRpc(xSpeed, isFacingRight);

        // �W�����v����
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
        // �T�[�o�[�ňʒu���X�V
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);

        // �N���C�A���g�S�̂ɃA�j���[�V�����ƌ����𓯊�
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

            // �L�����N�^�[�̌����𔽓]
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        } else
        {
            // ���[�J���̃L�������������X�V
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
