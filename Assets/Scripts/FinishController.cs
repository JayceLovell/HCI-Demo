using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishController : MonoBehaviour
{
    public Button AgainButton;
    public Button FinishButton;

    // Start is called before the first frame update
    void Start()
    {
        AgainButton.onClick.AddListener(Again);
        FinishButton.onClick.AddListener(Finish);
    }
    private void Again()
    {
        SceneManager.LoadScene("Title");
    }
    private void Finish()
    {
        Application.Quit();
    }
}
