using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isBeingSucked = false;  // �z�����܂�Ă����Ԃ��ǂ���
    protected Rigidbody2D rb;
    protected string Name = "�u���b�N";

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("�X�^�[�u���b�N�ɂԂ�������I");
        }
    }

    // �z�����܂�Ă���ꍇ�A�^�[�Q�b�g�̈ʒu�Ɍ������Ĉړ�����
    public void MoveTowardsTarget(Vector3 targetPosition)
    {
        // �z�����܂��ʒu�Ɍ������Ĉړ�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5f);
    }

    // �z������鏈��
    public void StartSuction(Transform player, float suikomiForce)
    {
        if (player == null)
        {
            Debug.LogError("Player is null!");
            return;
        }
        if (suikomiForce == null)
        {
            Debug.LogError("SuikomiForce is null!");
            return;
        }
        isBeingSucked = true;   // �z�����̃t���O�𗧂Ă�
        StartCoroutine(SuctionCoroutine(suikomiForce,player));
    }

    protected IEnumerator SuctionCoroutine(float suikomiForce,Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player is null in SuctionCoroutine!");
            yield break;
        }
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
    }

    public void StopSuction()
    {
        isBeingSucked = false; // �z�����݃t���O������
        rb.velocity = Vector2.zero; // �z���͂�����
        Debug.Log($"{Name} �̋z�����݂���������܂����B");
    }

}
