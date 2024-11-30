using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataDisplay : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform contentArea;
    public Button BtnResume;

    void Start()
    {
        List<CollectedData> collectedData = DataContainer.CollectedDataList;

        foreach (var data in collectedData)
        {
            GameObject newRow = Instantiate(rowPrefab, contentArea);
            TextMeshProUGUI[] columns = newRow.GetComponentsInChildren<TextMeshProUGUI>();

            if (columns.Length >= 2)
            {
                columns[0].text = data.Action.ToString();
                columns[1].text = data.UserInput;
            }
        }

        BtnResume.onClick.AddListener(Resume);

    }

    private void Resume()
    {
        SceneManager.LoadScene("Questionaire");
    }
}
