using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyController : MonoBehaviour
{
    public GameObject target;

    private Rigidbody rb;
    public float followDistance = 2;
    private float maxSpeed = 20;
    private float minSpeed = 5f;

    private float followForce = 100;

    private bool weaponized = false;
    private float deweaponizeSpeed = 5;

    public AudioClip creationSound;

    private AudioSource audioSource;

    private MeshRenderer meshRenderer;

    public Material activeMaterial;
    public Material deactiveMaterial;
    public Material weaponizedMaterial;

    private PlayerController playerController;

    private float deathHeight = -20.0f;

    private float lastTimeWithinTargetRange;
    private float timeAwayFromTargetBeforeCut = 5.0f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(creationSound, 1.0f);
        meshRenderer = GetComponent<MeshRenderer>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        lastTimeWithinTargetRange = Time.time;
    }

    public void SetTarget(GameObject newTarget, bool weaponize = false) {
        if (newTarget != null) {
            GetComponent<MeshRenderer>().material = activeMaterial;
            lastTimeWithinTargetRange = Time.time;


        } else {
            if (weaponize) {
                GetComponent<MeshRenderer>().material = weaponizedMaterial;
            } else {
                GetComponent<MeshRenderer>().material = deactiveMaterial;
            }
        }

        target = newTarget;
        weaponized = weaponize;
    }

    public bool HasTarget() {
        return (target != null);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target) {
            FollowTarget();
        }
    }

    void Update() {
        if (transform.position.y <= deathHeight) {
            if (target) {
                playerController.CutAtSegment(gameObject);
            }

            Destroy(gameObject);
        }

        if (weaponized) {
            if (rb.velocity.magnitude < deweaponizeSpeed) {
                weaponized = false;
                if (target) {
                    GetComponent<MeshRenderer>().material = activeMaterial;
                } else {
                    GetComponent<MeshRenderer>().material = deactiveMaterial;
                }
            }
        }
    }

    private void FollowTarget() {
        Vector3 displacement = target.transform.position - transform.position;

        if (displacement.magnitude > followDistance) {
            if (displacement.magnitude <= followDistance * 1.5f) {
                lastTimeWithinTargetRange = Time.time;
            }

            if (Time.time - lastTimeWithinTargetRange > timeAwayFromTargetBeforeCut) {
                playerController.CutAtSegment(gameObject);

            } else {
                float speedMult = displacement.magnitude / followDistance;

                float speed = target.GetComponent<Rigidbody>().velocity.magnitude * speedMult;

                if (speed > maxSpeed) {
                    speed = maxSpeed;
                }

                rb.velocity = new Vector3(
                    displacement.normalized.x * speed,
                    rb.velocity.y,
                    displacement.normalized.z * speed
                );
            }

        } else {
            lastTimeWithinTargetRange = Time.time;
            rb.velocity *= 0.99f;

        }
    }

    private void FollowTargetOld() {
        Vector3 displacement = target.transform.position - transform.position;

        if (displacement.magnitude > followDistance) {
            float speed = target.GetComponent<Rigidbody>().velocity.magnitude;

            if (speed > maxSpeed) {
                speed = maxSpeed;

            } else if (speed < minSpeed) {
                speed = minSpeed;
            }

            rb.velocity = new Vector3(
                displacement.normalized.x * speed,
                rb.velocity.y,
                displacement.normalized.z * speed
            );

        } else {
            rb.velocity *= 0.99f;
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("EnemyWeapon")) {
            if (target) {
                playerController.CutAtSegment(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (weaponized) {
            if (collision.gameObject.CompareTag("SnakeBody")) {

                SnakeBodyController otherSnakeBodyController = collision.gameObject.GetComponent<SnakeBodyController>();

                if (otherSnakeBodyController.HasTarget()) {
                    playerController.CutAtSegment(collision.gameObject);
                }

            } else if (collision.gameObject.CompareTag("Enemy")) {
                Destroy(collision.gameObject);

            }
        }
    }
}
