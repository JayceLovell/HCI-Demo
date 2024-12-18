using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


[Serializable]
public class CollectedData
{
    public enum Actions
    {
        Attack,
        Block,
        Dodge,
        SpeicalAttack,
        Move,
        Aim,
        Taunt,
        Fire,
        ToggleWeapons,
        Jump,
        Finish
    }

    public Actions Action;
    public string UserInput;
}
public class DemoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RenderTexture renderTexture;

    public GameObject AskForInput;
    public GameObject InputRecieved;

    public TextMeshProUGUI vidoeCounter;

    public List<CollectedData> collectedData=new List<CollectedData>();

    private Dictionary<VideoClip, CollectedData.Actions> videoActionMap = new Dictionary<VideoClip, CollectedData.Actions>();

    public List<VideoClip> videos = new List<VideoClip>();

    public bool IsWaitingForInput
    {
        get
        {
            return isWaitingForInput;
        }
        set
        {
            isWaitingForInput = value;

            AskForInput.SetActive(value);
        }
    }
    public bool InputHasBeenRecieved
    {
        get
        {
            return inputHasBeenRecieved;
        }
        set
        {
            inputHasBeenRecieved = value;

            InputRecieved.SetActive(value);

            if(value)
                IsWaitingForInput = false;
        }
    }
    public int CurrentVideoIndex
    {
        get
        {
            return currentVideoIndex;
        }
        set
        {
            currentVideoIndex = value;
            
        }
    }

    private bool isWaitingForInput = false;
    private bool inputHasBeenRecieved=false;

    private int currentVideoIndex = 0;
    private CollectedData.Actions currentAction;

    // Start is called before the first frame update
    void Start()
    {
        var loadedVideos = Resources.LoadAll<VideoClip>("Videos");
        foreach (var video in loadedVideos)
        {
            switch (video.name)
            {
                case "Attack":
                    videoActionMap[video] = CollectedData.Actions.Attack;
                    break;
                case "Block":
                    videoActionMap[video] = CollectedData.Actions.Block;
                    break;
                case "Dodge":
                    videoActionMap[video] = CollectedData.Actions.Dodge;
                        break;
                case "SpecialAttack":
                    videoActionMap[video] = CollectedData.Actions.SpeicalAttack;
                        break;
                case "Move":
                    videoActionMap[video] = CollectedData.Actions.Move;
                        break;
                case "Aim":
                    videoActionMap[video] = CollectedData.Actions.Aim;
                        break;
                case "Taunt":
                    videoActionMap[video] = CollectedData.Actions.Taunt;
                        break;
                case "Fire":
                    videoActionMap[video] = CollectedData.Actions.Fire;
                        break;
                case "ToggleWeapons":
                    videoActionMap[video] = CollectedData.Actions.ToggleWeapons;
                        break;
                case "Jump":
                    videoActionMap[video] = CollectedData.Actions.Jump;
                    break;
            }
        }

        videos.AddRange(videoActionMap.Keys);

        if (videos.Count == 0)
        {
            Debug.LogError("No videos found in Resources/Videos!");
            Application.Quit();
        }

       // videoPlayer.loopPointReached += EndOfVideo;
        videoPlayer.targetTexture = renderTexture;

        CurrentVideoIndex = 0;
        PlayNextVideo();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWaitingForInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SaveInput("LeftClick");
            }
            else if (Input.GetMouseButtonDown(1))
            {
                SaveInput("RightClick");
            }
            else if (Input.GetMouseButtonDown(2))
            {
                SaveInput("MiddleClick");
            }
            else if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
            {
                SaveInput("Scroll");
            }
            else if (Input.anyKeyDown)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        SaveInput(key.ToString());
                        break;
                    }
                }
            }
        }
    }
    private void SaveInput(string input)
    {
        collectedData.Add(new CollectedData
        {
            Action = currentAction,
            UserInput = input
        });

        Debug.Log("Action " + currentAction.ToString() + " to match Input " + input);

        InputHasBeenRecieved = true;

        CurrentVideoIndex++;

        StartCoroutine(DelayBeforeChangingVideo());
    }
    private void PlayNextVideo()
    {

        if (CurrentVideoIndex < videos.Count)
        {
            videoPlayer.clip = videos[CurrentVideoIndex];
            currentAction = videoActionMap[videoPlayer.clip];
            videoPlayer.Play();            
            IsWaitingForInput = false;
            InputHasBeenRecieved = false;
            vidoeCounter.SetText((CurrentVideoIndex + 1) + "/" + (videos.Count));
            StartCoroutine(WaitForFiveSecond());
        }
        else
        {
            currentAction = CollectedData.Actions.Finish;
        }

    }
    private void SaveData()
    {

        // Define the directory path for saving data
        string directoryPath = Path.Combine(Application.dataPath, "../InputData");

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
            filePath = Path.Combine(directoryPath, $"InputData{fileIndex}.csv");
            fileIndex++;
        }
        while (File.Exists(filePath));

        // Write data to the CSV file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header row
            writer.WriteLine("Action,User Input");

            // Write each row of collected data
            foreach (var data in collectedData)
            {
                writer.WriteLine($"{data.Action},{data.UserInput}");
            }
        }

        Debug.Log($"Data saved to {filePath}");
    }

    private IEnumerator WaitForFiveSecond()
    {
        yield return new WaitForSeconds(5f);
        IsWaitingForInput = true;
    }
    private IEnumerator DelayBeforeChangingVideo()
    {
        yield return new WaitForSeconds(8f);

        if (currentVideoIndex >= videos.Count)
        {
            DataContainer.CollectedDataList = collectedData;
            SaveData();
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Results");
        }
        else
            PlayNextVideo();
    }
}