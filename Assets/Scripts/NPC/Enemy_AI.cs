using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    public string Name;
    public float moveSpeed = 1f;             // �ړ����x
    public float changeDirectionInterval = 2f; // ������ύX����Ԋu
    protected float movementDirection;         // �ړ������i1: �E, -1: ���j
    protected Rigidbody2D rb;
    protected Animator anim;
    protected int _HP = 10;
    protected bool isBeingSucked = false;      // �z�������ǂ����̃t���O
    private bool isGround = false; // �n�ʂɂ��邩�ǂ���
    public const float normalGravityScale = 1f; // �ʏ펞�̏d�̓X�P�[��

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(ChangeDirectionRoutine());
    }

    protected void Update()
    {
        if (!isBeingSucked)  // �z�����łȂ��ꍇ�ɂ݈̂ړ�
        {
            Move();
        } else
        {
            //Debug.Log("�z�����܂ꒆ");
        }
    }

    protected void IS_Falling()
    {
        // �Ⴆ�΁AY���W�� -50 ����������ꍇ�ɃL�����N�^�[�����������Ɣ���
        if (transform.position.y <= -50)
        {
            _HP = 0;
            Debug.Log(Name + "�����E�̍����܂ŗ����܂����B Y���W�F" + transform.position.y);
            CheckHP(); // HP��0�ɂȂ�����폜���������s
        }
    }


    protected void Move()
    {
        // ���݂̈ړ������Ɋ�Â���x���݂̂ňړ�����
        rb.velocity = new Vector2(movementDirection * moveSpeed, rb.velocity.y);
        anim.SetInteger("Speed", 1);

        // �ړ������Ɋ�Â��ăL�����N�^�[�̌�����ݒ�
        if (movementDirection > 0)
        {
            // �E����
            transform.localScale = new Vector3(1, 1, 1);
        } else if (movementDirection < 0)
        {
            // ������
            transform.localScale = new Vector3(-1, 1, 1);
        }
        IS_Falling();
    }

    protected IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            // �����_���ɉE�����Ɉړ�������ύX�i1��-1���擾�j
            movementDirection = GetRandomDirection();

            // �w�肳�ꂽ���ԑҋ@
            yield return new WaitForSeconds(changeDirectionInterval);
        }
    }

    protected float GetRandomDirection()
    {
        // �����_���ɉE�i1�j�����i-1�j��I��
        return Random.value > 0.5f ? 1f : -1f;
    }


    // �_���[�W���󂯂鏈��
    public void TakeDamage(int damage)
    {
        _HP -= damage;
        Debug.Log("Enemy HP: " + _HP);
        CheckHP();
    }

    // HP��0�ȉ��ɂȂ�����I�u�W�F�N�g���폜
    protected void CheckHP()
    {
        if (_HP <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Enemy1 ���j�󂳂�܂����I");
        }
    }

    // �z������鏈��
    public void StartSuction(Transform player, float suikomiForce)
    {
        isBeingSucked = true;   // �z�����̃t���O�𗧂Ă�
        StartCoroutine(SuctionCoroutine(suikomiForce,player));
    }

    protected IEnumerator SuctionCoroutine(float suikomiForce, Transform playerTransform)
    {
            // �z���͂��v�Z
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * suikomiForce;
            // �v���C���[�Ɉ�苗���܂ŋ߂Â�����z������
            if (Vector2.Distance(playerTransform.position, transform.position) < 0.5f)
            {
                Debug.Log("�z�����݊����I");
                Destroy(gameObject); // NPC���폜
                yield break;
            }
            yield return null;  // ���̃t���[���܂őҋ@
        
        Debug.Log("�z�����݂��イ��傤");
    }

    public void StopSuction()
    {
        isBeingSucked = false; // �z�����݃t���O������
        rb.velocity = Vector2.zero; // �z���͂�����
        Debug.Log($"{Name} �̋z�����݂���������܂����B");
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
       else if (collision.gameObject.CompareTag("StarBlock"))// �ǂȂǂɏՓ˂����ꍇ�͕�����ς���
       {
            movementDirection = -movementDirection; // �������t�ɂ���
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
