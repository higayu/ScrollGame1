using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Copy_Ability
{
    Normal,
    Sword,
    Fire,
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance = null;

    public int HP = 0;
    public int MaxHP = 0;
    public Copy_Ability copy_Ability = Copy_Ability.Normal;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }

}
