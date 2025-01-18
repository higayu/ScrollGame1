using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    public Transform target;
    Vector3 pos;

    public BGMController bgmController;

    // �J������Y���I�t�Z�b�g�i�Ώۂ���ʂ̉��ɔz�u���鋗�����w��j
    public float verticalOffset = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        pos = Camera.main.gameObject.transform.position; // �J�����̏����ʒu��ϐ�pos�ɓ����

        bgmController = FindObjectOfType<BGMController>();

        if (bgmController == null)
        {
            Debug.LogError("BGMController��������܂���B");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = target.transform.position; // cameraPos�Ƃ����ϐ������A�Ǐ]����Ώۂ̈ʒu������

        if (target != null)
        {
            // ���ɂ����ł͑Ώۂ�Ǐ]���鏈�������C���ɂȂ�
        }

        // �Ώۂ�X����0��菬�����ꍇ�A�J������X���ʒu�𐧌�
        if (target.transform.position.x < 0)
        {
            cameraPos.x = 0;
        }

        // Y���̈ʒu�𒲐� (���]���ăJ��������ɂ��炵�A�Ώۂ����Ɍ�����悤�ɂ���)
        cameraPos.y = target.transform.position.y - verticalOffset;

        // �J�����̉��s�����Œ�
        cameraPos.z = -10;

        // �J�����̈ʒu���X�V
        Camera.main.gameObject.transform.position = cameraPos;
    }
}
