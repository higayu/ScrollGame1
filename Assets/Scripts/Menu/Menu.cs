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
            // Lambda 関数を使用して、引数付きメソッドを登録
            title.onClick.AddListener(() => Scene_Method("Menu"));
        }

    }

    public void Scene_Method(string name)
    {
        SceneManager.LoadScene(name);
    }


}
