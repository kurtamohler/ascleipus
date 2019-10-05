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


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(creationSound, 1.0f);
        meshRenderer = GetComponent<MeshRenderer>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void SetTarget(GameObject newTarget, bool weaponize = false) {
        // meshRenderer.material = deactiveMaterial;
        if (newTarget != null) {
            GetComponent<MeshRenderer>().material = activeMaterial;

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
            } else {
                Destroy(gameObject);
            }
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
        // Vector3 displacement = new Vector3(
        //     target.transform.position.x - transform.position.x,
        //     0,
        //     target.transform.position.z - transform.position.z
        // );

        Vector3 displacement = target.transform.position - transform.position;

        if (displacement.magnitude > followDistance) {
            float speedMult = displacement.magnitude / followDistance;

            // if (speedMult > 2.0f) {
            //     speedMult = 2.0f;
            // }

            float speed = target.GetComponent<Rigidbody>().velocity.magnitude * speedMult;

            if (speed > maxSpeed) {
                speed = maxSpeed;
            }

            rb.velocity = new Vector3(
                displacement.normalized.x * speed,
                rb.velocity.y,
                displacement.normalized.z * speed
            );

        } else {
            rb.velocity *= 0.99f;

        }

        // ApplyMaxSpeed();
    }

    private void FollowTargetOld() {
        // Vector3 displacement = new Vector3(
        //     target.transform.position.x - transform.position.x,
        //     0,
        //     target.transform.position.z - transform.position.z
        // );

        Vector3 displacement = target.transform.position - transform.position;

        if (displacement.magnitude > followDistance) {
            float speed = target.GetComponent<Rigidbody>().velocity.magnitude;

            if (speed > maxSpeed) {
                speed = maxSpeed;

            } else if (speed < minSpeed) {
                speed = minSpeed;
            }

            // float speed = maxSpeed;

            rb.velocity = new Vector3(
                displacement.normalized.x * speed,
                rb.velocity.y,
                displacement.normalized.z * speed
            );

            // Vector3 force = displacement.normalized * followForce;

            // rb.AddForce(force);


            // rb.AddForce()
        } else {
            rb.velocity *= 0.99f;

        }

        // ApplyMaxSpeed();
    }


    void ApplyMaxSpeed() {
        Vector3 horizVel = new Vector3(
            rb.velocity.x,
            0,
            rb.velocity.z
        );
        float horizSpeed = horizVel.magnitude;

        if (horizSpeed > maxSpeed) {
            float fixFactor = maxSpeed / horizSpeed;

            rb.velocity = new Vector3(
                rb.velocity.x * fixFactor,
                rb.velocity.y,
                rb.velocity.z * fixFactor
            );
        }
    }


    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("EnemyWeapon")) {
            if (target) {
                playerController.CutAtSegment(gameObject);

            // } else {
                // if (!weaponized) {
                //     Destroy(gameObject);
                // }
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
