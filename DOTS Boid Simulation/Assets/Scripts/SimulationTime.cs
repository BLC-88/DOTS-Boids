using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationTime : MonoBehaviour
{
    bool paused;
    FPSDisplay fpsDisplay;

    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject resumeButton;

    void Start()
    {
        Time.timeScale = 1f;
        fpsDisplay = FindObjectOfType<FPSDisplay>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FlipPauseState();
        }
    }

    public void Pause()
    {
        paused = true;
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
        fpsDisplay.measureFPS = false;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        paused = false;
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);
        fpsDisplay.StartCoroutine(fpsDisplay.SpawnDelay());
        Time.timeScale = 1f;
    }

    public void FlipPauseState()
    {
        paused = !paused;
        if (paused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }
}
