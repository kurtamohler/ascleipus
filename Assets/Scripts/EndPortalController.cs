using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPortalController : MonoBehaviour
{
    float segmentCountTimeout = 2.0f;
    float timeLastSegmentCount;

    bool segmentCountStarted = false;

    int segmentCount = 0;

    private EndMenuController endMenuController;


    // Start is called before the first frame update
    void Start()
    {
        endMenuController = GameObject.Find("EndMenu").GetComponent<EndMenuController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (segmentCountStarted) {
            float timeNow = Time.time;
            float timeSinceLastCount = timeNow - timeLastSegmentCount;
            // Debug.Log("now: " + timeNow);

            // Debug.Log("time since: " + timeSinceLastCount);
            // Debug.Log("timout: " + segmentCountTimeout);

            if (timeSinceLastCount >= segmentCountTimeout) {
                segmentCountStarted = false;
                // GameObject.Find("PauseMenu").GetComponent<PauseController>().SetPausedState(true);
                endMenuController.EndGame(segmentCount);

                Debug.Log("You got " + segmentCount + " segments");
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            collider.GetComponent<PlayerController>().SetActive(false);
            timeLastSegmentCount = Time.time;
            segmentCountStarted = true;
            Debug.Log("start: " + timeLastSegmentCount);

        } else if (segmentCountStarted && collider.CompareTag("SnakeBody")) {
            timeLastSegmentCount = Time.time;
            segmentCount++;
        }
    }
}
