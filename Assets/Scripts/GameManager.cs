using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        double screenRefreshRate = Screen.currentResolution.refreshRateRatio.value;
        Debug.Log("Screen refresh rate: " + screenRefreshRate);
        // parse to int
        int refreshRate = (int)screenRefreshRate;
        Application.targetFrameRate = refreshRate;

    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
