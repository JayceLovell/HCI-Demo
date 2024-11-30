using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleBehavior : MonoBehaviour
{
    public Button BeginButton;
    // Start is called before the first frame update
    void Start()
    {
        BeginButton.onClick.AddListener(Begin);
    }

    private void Begin()
    {
        SceneManager.LoadScene("Demo");
    }
}
