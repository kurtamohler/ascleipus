using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEnemyController : MonoBehaviour
{
    public float sightDistance = 20.0f;
    public float speed = 5.0f;
    public float force = 1000.0f;

    public float rotSpeed = 2.0f;

    private GameObject player;
    private Vector3 startPos;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!FollowPlayer()) {
            HeadHome();
        }
    }

    void MoveToward(Vector3 direction) {
        if (rb.velocity.magnitude < speed) {
            rb.AddForce(
                direction * force * Time.deltaTime
            );
        }
    }

    void RotateToward(Vector3 direction) {

        Quaternion lookRot = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            Time.deltaTime * rotSpeed
        );
    }

    bool FollowPlayer() {
        Vector3 displacement = player.transform.position - transform.position;
        bool followingPlayer = false;

        if (displacement.magnitude <= sightDistance) {
            followingPlayer = true;
            
            displacement.y = 0;

            RotateToward(displacement.normalized);
            MoveToward(transform.forward);

        }

        return followingPlayer;
    }


    void HeadHome() {

    }
}
