using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    public float deleteTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        // 2�b��ɃI�u�W�F�N�g���폜
        Destroy(gameObject, deleteTime);
    }

    // ���̃I�u�W�F�N�g�ƏՓ˂������ɌĂ΂��
    void OnCollisionEnter2D(Collision2D collision)
    {
        // �Փ˂����I�u�W�F�N�g�̃^�O�� "StarBlock" �Ȃ�폜
        if (collision.gameObject.tag == "StarBlock")
        {
            Debug.Log("�X�^�[�u���b�N�ɏՓˁI�I");
            Destroy(gameObject);
        } else if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("�n�ʂɏՓˁI�I");
            Destroy(gameObject);
        } else if (collision.gameObject.tag == "Enemy1")
        {
            Debug.Log("���h���f�B�ɏՓˁI�I");

            // Enemy1 �ɃZ�b�g���ꂽ WanderingAI �X�N���v�g���擾
            WanderingAI enemyAI = collision.gameObject.GetComponent<WanderingAI>();

            if (enemyAI != null)
            {
                // �_���[�W��^����i5�|�C���g�̃_���[�W�j
                enemyAI.TakeDamage(10);
            }

            // ���g�̃I�u�W�F�N�g�͏Փˌ�ɍ폜
            Destroy(gameObject);
        }
    }

}
