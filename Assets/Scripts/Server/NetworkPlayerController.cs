using UnityEngine;
using Unity.Netcode;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Globalization;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkPlayerController : NetworkBehaviour
{
    public const int speed = 5; // �ʏ�̈ړ����x
    public float jumpForce = 5f; // �W�����v��

    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false; // �n�ʂɂ��邩�ǂ���

    private bool isFacingRight = true;

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

        // ���[�J���ŃW�����v����
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            anim.SetInteger("Jump", 1);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // �T�[�o�[�Ɉړ����N�G�X�g�𑗐M
        RequestMoveServerRpc(xSpeed);
    }

    [ServerRpc]
    private void RequestMoveServerRpc(float xSpeed)
    {
        // �T�[�o�[�ňʒu���X�V
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);

        // �N���C�A���g�S�̂ɃA�j���[�V�����ƌ����𓯊�
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
