using TMPro;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private HPSystem hpSystem; // �C���X�y�N�^�[����A�T�C��

    public void Set_Damage()
    {
        Debug.Log("Damage����");

        if (hpSystem == null)
        {
            Debug.LogError("HPSystem���C���X�y�N�^�[�ŃA�T�C������Ă��܂���B");
        }

        hpSystem.HPDown(20);
        Debug.Log("Damage����");
    }


}
