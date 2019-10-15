using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    private bool paused;
    private GameObject pauseMenuContent;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuContent = gameObject.transform.GetChild(0).gameObject;
        paused = pauseMenuContent.active;
        UpdatePausedState();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            UpdatePausedState();
        }
    }

    void UpdatePausedState() {

        pauseMenuContent.SetActive(paused);

        if (paused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
}
