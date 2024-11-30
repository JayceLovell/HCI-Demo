using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MoveOnVideoEnd : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.loopPointReached += MoveOn;
    }

    private void MoveOn(VideoPlayer vp)
    {
        SceneManager.LoadScene("Demo");
    }
}
