using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private bool isPermanent = true;

    private Material buttonUnpressedMaterial;

    private int pressedCount;
    private GameObject topUnpressedButton;

    // Start is called before the first frame update
    void Start()
    {
        topUnpressedButton = transform.Find("TopUnpressed").gameObject;
        pressedCount = 0;
        buttonUnpressedMaterial = GetComponent<Material>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player") || collider.CompareTag("SnakeBody")) {
            topUnpressedButton.SetActive(false);
            pressedCount++;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (!isPermanent) {
            if (collider.CompareTag("Player") || collider.CompareTag("SnakeBody")) {
                pressedCount--;

                if (pressedCount == 0) {
                    topUnpressedButton.SetActive(true);
                }
            }
        }
    }

    public bool IsPressed() {
        return pressedCount > 0;
    }
}
