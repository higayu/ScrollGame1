using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Anime_Player : MonoBehaviour
{


    public const int speed = 5; // �ʏ�̈ړ����x
    public float jumpForce = 5f; // �W�����v��
    public float dashSpeedMultiplier = 2f; // �_�b�V�����̑��x�{��
    public int dashDuration = 10; // �_�b�V���̎������ԃt���[����

    public const float normalGravityScale = 1f; // �ʏ펞�̏d�̓X�P�[��

    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false; // �n�ʂɂ��邩�ǂ���
    private float realSpeed = speed; // ���ۂ̑��x

    
    private bool isFacingRight = true;

    public AudioSource audioSource;// AudioSource�R���|�[�l���g


    #region -------------------�y �h�A �z------------------
    private bool isTouchingDoor = false; // �h�A�ƐڐG���Ă��邩�ǂ����������t���O
    private string Door_Name;
    #endregion ----------------------------------------------

    #region //------------------------------�ystart���\�b�h�z---------------------------------------------//
    // Start is called before the first frame update
    void Start()
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

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource�R���|�[�l���g��������܂���B");
        }

    }
    #endregion //---------------------------------------------------------------------------//

    // Update is called once per frame
    #region  ----------�y �A�b�v�f�[�g�C�x���g �z---------------------------------------------
    void Update()
    {

        #region  ----------�y �I�u�W�F�N�g�̗L�� �z---------------------------------------------
        if (anim == null || rb == null || audioSource == null || audioSource == null)
        {
            //Debug.Log("�����Ȃ�");
            //return; // �K�v�ȃR���|�[�l���g��������Ȃ��ꍇ�͏����𒆒f
        }
        #endregion  ----------�y  �z---------------------------------------------


        #region  ----------�y �ړ����� �z---------------------------------------------
        // ���͂��擾
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            anim.SetInteger("Speed", 1);
            xSpeed = realSpeed;
            Debug.Log("�E�Ɉړ�");

        } else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            anim.SetInteger("Speed", 1);
            xSpeed = -(float)speed;
            Debug.Log("���Ɉړ�");
        } else
        {
            anim.SetInteger("Speed", 0);
            xSpeed = 0.0f;
            realSpeed = speed; // ��~���ɑ��x��ʏ�ɖ߂�
        }

        isFacingRight = transform.localScale.x > 0;


        //Debug.Log("�X�s�[�h : " + xSpeed);
        // ���ړ��̑��x��ݒ�
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);
        //Debug.Log("�ړ���̏���" + rb.velocity);

        #endregion  ----------�y�ړ����� �z---------------------------------------------


        #region  ----------�y �W�����v���� �z---------------------------------------------
        // �X�y�[�X�L�[�������ꂽ�ꍇ�A���L�����N�^�[���n�ʂɂ���ꍇ�ɃW�����v
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            anim.SetInteger("Jump", 1);
           // Debug.Log("�W�����v�m��I�I�I isGround: " + isGround);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        } else if (Input.GetKeyDown(KeyCode.Space) && !isGround)
        {

            Debug.Log(anim.GetInteger("Jump") + ": �W�����vGET");

        }
        else if(!isGround && anim.GetInteger("Jump") == 0)
        {
            anim.SetInteger("Jump", 1);
        }
        #endregion  ----------�y �W�����v���������z---------------------------------------------



        if (isTouchingDoor && Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("�h�A�̑O�ŏ���L�[����");
            // �h�A�Ɋ֘A���鏈�������s
            SoundEffect.Instance.Door_Sound();
            Door_Method();
        }
    }
    #endregion --------------------------------------------------------------------------

  
    #region---- NPC����������z�����J�n�����
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy1"))
        {
            Enemy_AI enemy = other.GetComponent<Enemy_AI>();

        }


        if (other.CompareTag("Door"))
        {
            isTouchingDoor = true;
            // Door �N���X���擾
            Door door = other.GetComponent<Door>();
            if (door != null)
            {
                Door_Name = door.Scene_name; // Door �N���X�̃v���p�e�B���g�p
                Debug.Log("�h�A��Scene��"+ Door_Name);
            }
        }
    }
    #endregion ---------------------------------------------------------------------------------------

    #region ----�h�A�̓����蔻�� ------
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
