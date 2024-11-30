using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionareController : MonoBehaviour
{
    public Button BtnSubmit;

    public List<ToggleGroup> ToggleGroups;

    // Start is called before the first frame update
    void Start()
    {
        BtnSubmit.onClick.AddListener(Submit);

        foreach(var toggleGroup in ToggleGroups)
        {
            foreach(var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener(delegate { Check(); } );
            }
        }
    }
    private void Check()
    {
        bool allAnswered = true;
        foreach(var group in ToggleGroups)
        {
            if(GetActiveToggle(group) == null)
            {
                allAnswered = false;
                break;
            }
        }

        BtnSubmit.interactable = allAnswered;
    }
    private void Submit()
    {
        List<string> userAnswers = new List<string>();
        int questionNumber = 1;

        foreach (ToggleGroup group in ToggleGroups)
        {
            Toggle activeToggle = GetActiveToggle(group);

            if (activeToggle != null)
                userAnswers.Add($"Question {questionNumber},{activeToggle.name}");
            
            questionNumber++;

        }

        SaveToCSV(userAnswers);

        SceneManager.LoadScene("Finish");
    }

    private Toggle GetActiveToggle(ToggleGroup group)
    {
        foreach (var toggle in group.ActiveToggles())
        {
            return toggle; 
        }
        return null;
    }
    private void SaveToCSV(List<string> userAnswers)
    {
        // Define the directory path for saving data
        string directoryPath = Path.Combine(Application.dataPath, "QuestioanreData");

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Find a unique file name by checking for existing files
        int fileIndex = 1;
        string filePath;

        do
        {
            filePath = Path.Combine(directoryPath, $"QuestionareData{fileIndex}.csv");
            fileIndex++;
        }
        while (File.Exists(filePath));

        // Write data to the CSV file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header row
            writer.WriteLine("Question,Answer");

            // Write each row of collected data
            foreach (var data in userAnswers)
            {
                writer.WriteLine($"{data}");
            }
        }

        Debug.Log($"Data saved to {filePath}");
    }
}
