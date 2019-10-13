using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private Vector3 openOffset;
    [SerializeField] private float speed;
    [SerializeField] private bool makeButtonsPermanent;

    private float acceptableDistance = 0.001f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    bool open = false;

    // Start is called before the first frame update
    void Start()
    {
        closedPosition = this.transform.position;
        openPosition = closedPosition + openOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (buttons.Count > 0) {
            CheckButtons();
        }
        UpdatePosition();
    }

    void CheckButtons() {
        bool allButtonsPressed = true;

        foreach (GameObject button in buttons) {
            if (!button.GetComponent<ButtonController>().IsPressed()) {
                allButtonsPressed = false;
            }
        }

        open = allButtonsPressed;

        if (allButtonsPressed && makeButtonsPermanent) {
            foreach (GameObject button in buttons) {
                button.GetComponent<ButtonController>().SetPermanent(true);
            }

        }
    }

    void UpdatePosition() {
        if (open) {
            MoveToPosition(openPosition);

        } else {
            MoveToPosition(closedPosition);

        }
    }

    void MoveToPosition(Vector3 position) {
        if ((transform.position - position).magnitude > acceptableDistance) {
                transform.position = Vector3.Slerp(transform.position, position, speed*Time.deltaTime);
        }
    }
}
