using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float brakeAccel;

    public GameObject jumpBarPrefab;

    private SegmentCounterController segmentCounterController;
    private float curMaxSpeed;

    private Rigidbody rb;
    private Vector3 curMoveDirection;

    public GameObject snakeBodyPrefab;
    // private List<GameObject> bodySegments;
    private LinkedList<GameObject> bodySegments;

    bool hasThrowPowerup = true;
    bool hasBrakePowerup = true;
    float throwSpeed = 30.0f;
    float throwStartDist = 1.5f;

    // The first segment can only be thrown if it is close to the player
    float maxDistFromPlayerForThrow = 2.0f;

    float deathHeight = -20.0f;

    bool isActive;

    BodySpikesController spikesController;

    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
        curMoveDirection = new Vector3(1,0,0);
        rb = GetComponent<Rigidbody>();

        bodySegments = new LinkedList<GameObject>();

        for (int i = 0; i < 0; i++) {
            CreateNewBodySegment();
        }

        curMaxSpeed = maxSpeed;

        segmentCounterController = GameObject.Find("SegmentCounter").GetComponent<SegmentCounterController>();

        spikesController = gameObject.GetComponent<BodySpikesController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActive) {
            float vertInput = Input.GetAxis("Vertical");
            float horizInput = Input.GetAxis("Horizontal");

            if (vertInput != 0 || horizInput != 0) {
                float vertForce = vertInput * Time.deltaTime * speed;
                float horizForce = horizInput * Time.deltaTime * speed;

                Vector3 force = new Vector3(
                    horizForce,
                    0,
                    vertForce
                );

                force *= rb.mass;

                rb.AddForce(force);
            }

            ApplyMaxSpeed();
        }
    }

    void ApplyMaxSpeed() {
        Vector3 horizVel = new Vector3(
            rb.velocity.x,
            0,
            rb.velocity.z
        );
        float horizSpeed = horizVel.magnitude;

        if (horizSpeed > curMaxSpeed) {
            float fixFactor = curMaxSpeed / horizSpeed;

            horizVel = new Vector3(
                rb.velocity.x * fixFactor,
                0,
                rb.velocity.z * fixFactor
            );

            rb.velocity = new Vector3(
                horizVel.x,
                rb.velocity.y,
                horizVel.z 
            );
        }

        curMoveDirection = horizVel.normalized;
    }


    void Update() {
        if (isActive) {
            if (transform.position.y < deathHeight) {
                Die();
            }

            /*
            if (Input.GetKeyDown(KeyCode.C)) {
                CreateNewBodySegment();
            }
            */

            if (Input.GetKeyDown(KeyCode.Space)) {
                // ShootBodySegment();
                // Jump();
                // DropJumpBar();
                if (!spikesController.IsExtended()) {
                    //spikes.SetActive(true);

                    if (bodySegments.Count > 0) {
                        LinkedListNode<GameObject> nextBodySegmentNode = bodySegments.First;
                        spikesController.StartExtending(nextBodySegmentNode);
                    } else {
                        spikesController.StartExtending();

                    }

                    
                    /*
                    foreach (GameObject bodySegment in bodySegments) {
                        bodySegment.GetComponent<BodySpikesController>().StartExtending();
                        //bodySegment.GetComponent<BodySpikesController>().SetActive(true);
                    }
                    */
                }
            }

            if (hasBrakePowerup) {
                if (Input.GetKey(KeyCode.LeftShift)) {
                    ApplyBrake();
                }
            }
        }
    }

    public bool IsActive() {
        return isActive;
    }

    public void SetActive(bool active) {
        isActive = active;
    }

    void ApplyBrake() {
        Vector3 horizVel = new Vector3(
            rb.velocity.x,
            0,
            rb.velocity.z
        );

        Vector3 accel = -horizVel.normalized * brakeAccel * horizVel.magnitude;

        rb.AddForce(accel, ForceMode.Acceleration);
    }

    void AddBodySegment(GameObject newSegment) {
        Vector3 position = transform.position;
        GameObject target = gameObject;

        position = target.transform.position;

        position.y += 1f;

        SnakeBodyController newSegmentController = newSegment.GetComponent<SnakeBodyController>();

        newSegmentController.SetTarget(target);
        newSegmentController.followDistance = 1.0f;

        newSegment.transform.position = position;

        if (bodySegments.Count > 0) {
            bodySegments.First.Value.GetComponent<SnakeBodyController>().SetTarget(newSegment);
        }

        bodySegments.AddFirst(newSegment);
        segmentCounterController.UpdateCount(bodySegments.Count);
    }

    void CreateNewBodySegment() {
        GameObject newSegment = Instantiate(snakeBodyPrefab);

        AddBodySegment(newSegment);
    }

    void ShootBodySegment() {
        if (bodySegments.Count > 0) {
            GameObject shootSegment = bodySegments.First.Value;
            SnakeBodyController shootSegmentController = shootSegment.GetComponent<SnakeBodyController>();

            shootSegmentController.SetTarget(null, true);

            bodySegments.RemoveFirst();

            // Change the first segment's target to follow player
            if (bodySegments.Count > 0) {
                bodySegments.First.Value.GetComponent<SnakeBodyController>().target = gameObject;
            }

            // If player has throw powerup, then place the segment in front of
            // the player and give it a forward velocity
            if (hasThrowPowerup) {
                shootSegment.transform.position = gameObject.transform.position + curMoveDirection * throwStartDist;

                Vector3 shootVel = new Vector3(
                    rb.velocity.x,
                    0,
                    rb.velocity.z
                );

                shootVel += curMoveDirection * throwSpeed;

                shootSegment.GetComponent<Rigidbody>().velocity = shootVel;
            } else {
                shootSegment.GetComponent<Rigidbody>().velocity *= 1.5f;
            }
        }
    }
    
    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("FoodPickup")) {
            CreateNewBodySegment();
            Destroy(collider.gameObject);
        } else if (collider.CompareTag("EnemyWeapon")) {
            Die();
        }
    }

    private void EatSnakeBody(GameObject segment) {
        SnakeBodyController segmentController = segment.GetComponent<SnakeBodyController>();

        if (segmentController.HasTarget()) {

        } else {
            AddBodySegment(segment);
        }
    }

    void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("SnakeBody")) {
            EatSnakeBody(collision.gameObject);

        } else if (collision.gameObject.CompareTag("EnemyWeapon")) {
            Die();
        }
    }

    void Die() {

        foreach (GameObject bodySegment in bodySegments) {
            bodySegment.GetComponent<SnakeBodyController>().SetTarget(null);
        }

        gameObject.SetActive(false);

        Invoke("ReloadScene", 3);
    }

    void ReloadScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Destroy the specified segment and detach all segments after it
    public void CutAtSegment(GameObject segment) {
        LinkedListNode<GameObject> curNode = bodySegments.First;
        bool segmentFound = false;

        while (curNode != null) {
            LinkedListNode<GameObject> nextNode = curNode.Next;

            if (segmentFound || GameObject.ReferenceEquals(segment, curNode.Value)) {
                curNode.Value.GetComponent<SnakeBodyController>().SetTarget(null);
                bodySegments.Remove(curNode);
                segmentFound = true; 
            }

            curNode = nextNode;
        }

        segmentCounterController.UpdateCount(bodySegments.Count);
    }

    void Jump() {
        rb.AddForce(Vector3.up*200, ForceMode.Impulse);
    }

    void DropJumpBar() {
        Instantiate(jumpBarPrefab, transform.position - Vector3.up*0.5f, Quaternion.identity);
    }
}
