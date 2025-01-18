using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour
{
    public string targetTag = "Enemy1"; // �擾�������I�u�W�F�N�g�̃^�O�B�f�t�H���g��"Untagged"

    // Start is called before the first frame update
    void Start()
    {
        Enemys_check();
    }

    private void Enemys_check()
    {
        // �V�[�����̂��ׂẴQ�[���I�u�W�F�N�g���擾
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        // �^�O�Ńt�B���^�����O
        List<GameObject> taggedObjects = new List<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.CompareTag(targetTag))
            {
                taggedObjects.Add(obj);
            }
        }

        // ���ʂ��f�o�b�O�\��
        foreach (var obj in taggedObjects)
        {
            Debug.Log($"Detected object with tag {targetTag}: {obj.name}");
            Enemy_AI enemy = obj.GetComponent<Enemy_AI>();
            if (enemy != null)
            {
                Debug.Log(enemy.Name);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
            
    }
}
