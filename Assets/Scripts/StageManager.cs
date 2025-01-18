using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public string[] Alpha_Ary = new string[] {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };

    public GameObject npcPrefab; // NPC�̃v���n�u
    public Vector2 spawnPosition = new Vector2(21.37f, -2.55f); // �X�|�[������ʒu
    public float spawnInterval = 5f; // NPC�𐶐�����Ԋu�i�b�j

    private void Start()
    {
        // NPC�����Ԋu�Ő�������R���[�`�����J�n
        StartCoroutine(SpawnNPCs());
    }

    private IEnumerator SpawnNPCs()
    {
        while (true)
        {
            // ��莞�ԑҋ@
            yield return new WaitForSeconds(spawnInterval);

            // NPC���w�肳�ꂽ�ʒu�ɐ���
            GameObject npc = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);

            // �X�N���v�g���擾����Name�t�B�[���h�ɖ��O��ݒ�
            Enemy_AI npcController = npc.GetComponent<Enemy_AI>();
            if (npcController != null)
            {
                npcController.Name += Alpha_Ary[Random.Range(0, 26)]; // �����_���Ȗ��O��ݒ�
                Debug.Log(npcController.Name);
            }

            Debug.Log("NPC���X�|�[�����܂���: " + spawnPosition);


        }
    }
}
