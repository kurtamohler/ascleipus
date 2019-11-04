using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalPointController : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    private GameObject endPortal;

    private Vector3 target;
    private float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        endPortal = GameObject.Find("EndPortal");
        target = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player && playerController.IsActive()) {
            target = player.transform.position;

        } else {
            target = endPortal.transform.position;
        }

    }

    void FixedUpdate() {
        MoveToTarget();
    }

    void MoveToTarget() {
        Vector3 offset = target - transform.position;

        transform.position += offset * Time.deltaTime * speed;
    }
}
