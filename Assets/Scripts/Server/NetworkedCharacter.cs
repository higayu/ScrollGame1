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

    private bool isGround = false; // �n�ʂɐڂ��Ă��邩�ǂ���
    private bool isHovering = false;
    private Animator anim;         // �A�j���[�^�[
    private Rigidbody2D rb;        // Rigidbody2D 
    public const float normalGravityScale = 1.0f;// �ʏ�̏d�̓X�P�[��
    public const float hoverGravityScale = 0.5f;//jump���Q�̃z�o�����O���[�h�̎�

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsOwner)
        {
            HandleMovement(); // ���W�ύX������ǉ�
            MyCustomModel data = networkData.Value;
            data.position = transform.position;
            data.isOnGround = isGround; // �n�ʂ̏�Ԃ𔽉f
            data.animationValue = GetAnimationValue();
            networkData.Value = data;

            // �A�j���[�V�����̍X�V���z�X�g���ł��K�p
            UpdateAnimation(data.animationValue);
        } else
        {
            transform.position = networkData.Value.position; // ���W���f����
            UpdateAnimation(networkData.Value.animationValue);
        }
    }


    private void HandleMovement()
    {
        // ���͂��擾
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // �E����
            xSpeed = speed;
        } else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // ������
            xSpeed = -speed;
        }

        // �W�����v����
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // �W�����v�͂�K�p
        }
        else if (Input.GetKeyDown(KeyCode.Space)|| isHovering)
        {
            isHovering = true;
            rb.gravityScale = hoverGravityScale; // �z�o�����O���̏d�͂�ݒ�
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // �W�����v�܂��̓z�o�����O�̓�����K�p
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        // ���ړ��̑��x��ݒ�
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"�Փ˂����I�u�W�F�N�g�̃^�O: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            isHovering = false;
            rb.gravityScale = normalGravityScale; // �d�͂����Z�b�g
            Debug.Log("�n�ʂɒ��n");
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"���ꂽ�I�u�W�F�N�g�̃^�O: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
            Debug.Log("�n�ʂ��痣�ꂽ");
        }
    }


    private AnimationValue GetAnimationValue()
    {
        AnimationValue animationValue = new AnimationValue();

        // �ړ���Ԃ̔���
        float horizontalKey = Input.GetAxis("Horizontal");
        if (horizontalKey != 0)
        {
            animationValue.Speed = 1; // �ړ���
        } else
        {
            animationValue.Speed = 0; // ��~��
        }

        // �W�����v��Ԃ̔���
        if (!isGround)
        {
            if (isHovering)
            {
                animationValue.Jump = 2; // �󒆃z�o�����O��
            } else
            {
                animationValue.Jump = 1; // �󒆂ɂ���
            }
        } else
        {
            animationValue.Jump = 0; // �n�ʂɂ���
        }

        // �f�o�b�O�o��
        if (animationValue.Jump >=1)
        {
           // Debug.Log($"[GetAnimationValue] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}");
        }
       
        // �z�����݂Ȃǂ̓����ԁi�K�v�ɉ����āj
        animationValue.Suikomi = 0;

        return animationValue;
    }


    private void UpdateAnimation(AnimationValue animationValue)
    {
        anim.SetInteger("Speed", animationValue.Speed);
        anim.SetInteger("Jump", animationValue.Jump);
        anim.SetInteger("Suikomi", animationValue.Suikomi);

        // �f�o�b�O�o��
        if (anim.GetInteger("Jump") == 1)
        {
             Debug.Log($"[�W�����v��] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}, Suikomi: {animationValue.Suikomi}");
        } else if (anim.GetInteger("Jump") == 2)
        {
            Debug.Log($"[�z�o�����O���[�h��] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}, Suikomi: {animationValue.Suikomi}");
        } else
        {
            Debug.Log($"[���s��] Speed: {animationValue.Speed}, Jump: {animationValue.Jump}, Suikomi: {animationValue.Suikomi}");
        }
    }

}


