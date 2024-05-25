using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public GameObject Pause_Panel;

    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Pause_Panel.SetActive(false);
        isPaused = false;
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        Pause_Panel.SetActive(true);
        isPaused = true;
    }
}
