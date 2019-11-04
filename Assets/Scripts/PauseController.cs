using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    private bool paused;
    private GameObject pauseMenuContent;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        pauseMenuContent = gameObject.transform.GetChild(0).gameObject;
        paused = pauseMenuContent.active;
        UpdatePausedState();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.IsActive() && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))) {
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

    public void SetPausedState(bool state) {
        paused = state;
        UpdatePausedState();
    }
}
