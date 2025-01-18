using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{

    public float speed = 2f;
    public LayerMask groundLayer; // �n�ʂƔ��肷�郌�C���[
    private Rigidbody2D rb;

    public float jumpForce = 5f; // �W�����v��

    public float detectionRadius = 5f; // �v���C���[���o���a
    public float fleeDistance = 3f; // �v���C���[���瓦���鋗��
    private Animator anim;
    private bool isGround = false; // �n�ʂɂ��邩�ǂ���
    public float normalGravityScale = 1f; // �ʏ�̏d�̓X�P�[


    private GameObject player;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (anim == null)
        {
            Debug.LogError("Animator�R���|�[�l���g��������܂���B");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D�R���|�[�l���g��������܂���B");
        }

        // �v���C���[��T��
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {

        Move();
        DetectPlayer();
    }



    private void Move()
    {
        // NPC�̊�{�I�ȃp�g���[���ړ��Ȃǂ�����
        anim.SetFloat("Speed", Mathf.Abs(speed));
        rb.velocity = new Vector2(speed, rb.velocity.y);

        // �ǂ�[�ŕ�����ς��鏈���Ȃǂ�ǉ�
        if (ShouldChangeDirection())
        {
            speed = -speed; // ���]
            Flip();
        }
    }

    private bool ShouldChangeDirection()
    {
        // �ǂɓ�����A�܂��͒n�ʂ̒[�ɓ��B�����������ς���
        // �n�ʂ̒[�𔻒肷�邽�߂�Raycast���g�p
        Vector2 rayOrigin = transform.position; // NPC�̌��݈ʒu����Raycast�𔭎�
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, groundLayer);

        // Raycast���n�ʂɃq�b�g���Ȃ������ꍇ�A�܂��͑O���ɕǂ�����ꍇ�ɕ�����ύX
        bool isAtEdge = hit.collider == null;

        // �O����Raycast��ł��A�ǂɏՓ˂��邩�ǂ������`�F�b�N
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 0.1f, groundLayer);

        return isAtEdge || wallHit.collider != null; // �n�ʂ��Ȃ��ꍇ�܂��͕ǂɓ�����ꍇ�ɕ�����ύX
    }


    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // ���]
        transform.localScale = localScale;
    }

    private void DetectPlayer()
    {
        // �v���C���[�����o���āA��苗���ȓ��Ȃ瓦����A�U������Ȃǂ̍s��������
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < detectionRadius)
            {
                if (distanceToPlayer < fleeDistance)
                {
                    // �v���C���[���瓦����
                    FleeFromPlayer();
                    if( anim.GetInteger("Speed") == 0)
                    {
                        anim.SetInteger("Speed", 1);
                    }

                } 
                else if (distanceToPlayer >6f) 
                {
                    // �v���C���[�Ɍ������čU����߂Â����������
                    ApproachPlayer();
                    if (anim.GetInteger("Speed") == 0)
                    {
                        anim.SetInteger("Speed", 1);
                    }
                }
                else
                {
                    if (anim.GetInteger("Speed") == 1)
                    {
                        anim.SetInteger("Speed", 0);
                    }
                }
            }
        }
    }

    private void FleeFromPlayer()
    {
        Debug.Log("�v���C���[���瓦����I");
        // �v���C���[�Ɣ��Ε����Ɉړ�
        Vector2 fleeDirection = (transform.position - player.transform.position).normalized;
        rb.velocity = fleeDirection * speed;
    }

    private void ApproachPlayer()
    {
        Debug.Log("�v���C���[�ɋ߂Â��I");
        // �v���C���[�Ɍ������Ĉړ�
        Vector2 approachDirection = (player.transform.position - transform.position).normalized;
        rb.velocity = approachDirection * speed;
    }

    #region //-----------------------------�y�n�ʂ̓����蔻��z----------------------------------------------//
    // �n�ʂɐڐG�����Ƃ��̏���
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            Debug.Log("�n�ʂɒ��n���܂����B");
            anim.SetInteger("Jump", 0);
            rb.gravityScale = normalGravityScale;
        }
    }

    // �n�ʂ��痣�ꂽ�Ƃ��̏���
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
            Debug.Log("�n�ʂ��痣��܂����B");
        }
    }
    #endregion //---------------------------------------------------------------------------//
}
