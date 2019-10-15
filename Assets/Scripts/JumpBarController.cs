using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBarController : MonoBehaviour
{
    [SerializeField] private float impulse;
    [SerializeField] private float timeoutSeconds = 0.5f;
    [SerializeField] private float lastJumpTime;


    private SortedList touchedObjectIDs;


    // Start is called before the first frame update
    void Start()
    {
        touchedObjectIDs = new SortedList();
        lastJumpTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider) {
        float curTime = Time.time;

        if (curTime - lastJumpTime >= timeoutSeconds) {
            Destroy(gameObject);
        } else if (collider.CompareTag("Player") || collider.CompareTag("SnakeBody")) {
            int objectID = collider.GetInstanceID();

            if (!touchedObjectIDs.Contains(objectID)) {
                collider.GetComponent<Rigidbody>().AddForce(Vector3.up * impulse, ForceMode.Impulse);
                touchedObjectIDs.Add(objectID, null);

                lastJumpTime = curTime;
            }
        }
    }
}
