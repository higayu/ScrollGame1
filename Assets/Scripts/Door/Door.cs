using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string Scene_name;

    private void OnCollisionStay2D(Collision2D collision)
    {
        //collision�ɑ��葤�̏�񂪊i�[�����B
        // Debug.Log(collision.gameObject.name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�R���C�_�[������������ŏ��ɌĂ΂��
        //collision�ɑ��葤�̏�񂪊i�[�����B
       // Debug.Log(collision.gameObject.name);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //�R���C�_�[�����ꂽ���ɌĂ΂��
       // Debug.Log(collision.gameObject.name);
    }

    //////////////////////
    ////  Trigger�n�@////
    ////////////////////
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�R���C�_�[������������ŏ��ɌĂ΂��
        //collision�ɑ��葤�̏�񂪊i�[�����B
        //Debug.Log("�Ԃ����Ă���I�u�W�F�N�g���F"+collision.name);
        //Debug.Log("�h�A�ɐڐG");

        //if (Input.GetKeyDown(KeyCode.UpArrow))//����L�[�̓���
        //{
        //    destination();
        //}
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //�R���C�_�[���������Ă���ƌp�����ČĂ΂��
        //Debug.Log(collision.name);
        //Debug.Log("�Ԃ����Ă���I�u�W�F�N�g���F" + collision.name);


        //if (Input.GetKeyDown(KeyCode.UpArrow))//����L�[�̓���
        //{
        //    Debug.Log("�h�A�̑O�ŏ���L�[����");
        //    destination();
        //}

        //if (collision.gameObject.CompareTag("Player")) // Player �^�O�Ŕ���
        //{
        //    Debug.Log("�Ԃ����Ă���I�u�W�F�N�g���F" + collision.name);

        //    if (Input.GetKeyDown(KeyCode.UpArrow)) // ����L�[�̓���
        //    {
        //        Debug.Log("�h�A�̑O�ŏ���L�[����");
        //        destination();
        //    }

        //    if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    {
        //        Debug.Log("�h�A�̑O�ō����L�[����");
        //    }

        //    if (Input.GetKeyDown(KeyCode.RightArrow))
        //    {
        //        Debug.Log("�h�A�̑O�ŉE���L�[����");
        //    }
        //}

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //�R���C�_�[�������ꂽ���ɌĂ΂��
        //Debug.Log(collision.name);
    }


}
