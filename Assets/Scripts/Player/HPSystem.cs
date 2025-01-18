using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//UI�g���Ƃ��͖Y�ꂸ�ɁI
using UnityEngine.UI;

public class HPSystem : MonoBehaviour
{

    [SerializeField] GameObject image;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] int max_hp = 0;

    private float currentHP = 0;

    public float CurrntHP{
        get { return currentHP; } 
        set {  currentHP = value; }
    }


    void Start()
    {
        HP_Set(100f,max_hp);
    }

    //()�̒��g�͈����A���̂Ƃ��납�琔�l�𓾂�{}�̒��Ŏg��
    public void HP_Set(float current,int MaxHP)
    {
        max_hp = MaxHP;
        CurrntHP = current;

        //Image�Ƃ����R���|�[�l���g��fillAmount���擾���đ��삷��
        image.GetComponent<Image>().fillAmount = current / max_hp;
        Text.text = ((current / max_hp)*100).ToString();
    }

    public void HPDown(float damage)
    {
        CurrntHP = CurrntHP - damage;
        Debug.Log("Damage���� : �c��̗� :"+CurrntHP);

        if (CurrntHP < 0)
        {
            CurrntHP = 0;
        }

        //Image�Ƃ����R���|�[�l���g��fillAmount���擾���đ��삷��
        image.GetComponent<Image>().fillAmount = CurrntHP / max_hp;
        Text.text = ((CurrntHP / max_hp) * 100).ToString();
    }
}