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
        Interact,
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

            vidoeCounter.SetText(CurrentVideoIndex+"/"+(videos.Count+1));
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
                case "SpeicalAttack":
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
                case "Interact":
                    videoActionMap[video] = CollectedData.Actions.Interact;
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

        PlayNextVideo();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWaitingForInput && Input.anyKeyDown)           
        {                      
            string inputKey = Input.inputString;

            collectedData.Add(new CollectedData
            {
                Action = currentAction,
                UserInput = inputKey
            });

            InputHasBeenRecieved = true;

            StartCoroutine(DelayBeforeChangingVideo());
        }
    }
    //private void EndOfVideo(VideoPlayer vp)
    //{
    //    if (InputHasBeenRecieved)
    //    {
    //        if (currentVideoIndex >= videos.Count)
    //        {
    //            SaveData();
    //            SceneManager.LoadScene("Finish");
    //        }
    //        else
    //            PlayNextVideo();            
    //    }
    //    else
    //    {            
    //        IsWaitingForInput = true;
    //        InputHasBeenRecieved = false;
    //    }
       
    //}
    private void PlayNextVideo()
    {

        if (CurrentVideoIndex < videos.Count)
        {
            videoPlayer.clip = videos[CurrentVideoIndex];
            currentAction = videoActionMap[videoPlayer.clip];
            videoPlayer.Play();
            CurrentVideoIndex++;
            IsWaitingForInput = false;
            InputHasBeenRecieved = false;

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
        string directoryPath = Path.Combine(Application.dataPath, "InputData");

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
        PlayNextVideo();
    }
}