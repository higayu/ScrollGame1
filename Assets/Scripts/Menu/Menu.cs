using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public UnityEngine.UI.Button title;


    private void Start()
    {

        if (title != null)
        {
            // Lambda �֐����g�p���āA�����t�����\�b�h��o�^
            title.onClick.AddListener(() => Scene_Method("Menu"));
        }

    }

    public void Scene_Method(string name)
    {
        SceneManager.LoadScene(name);
    }


}
