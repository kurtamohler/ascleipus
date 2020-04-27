using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyController : MonoBehaviour
{
    public GameObject target;

    private Rigidbody rb;
    public float followDistance = 2;


    // private float maxSpeed = 20;
    private float maxSpeed = 17;
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
    private float timeAwayFromTargetBeforeCut = 0.5f;


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
            prevTargetFilterPos = newTarget.transform.position;

            // maxSpeed = playerController.maxSpeed * 2.0f;


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

    // Pros:
    //      Very stable due to gravitation towards the follow distance. So if we are less than follow
    //          distance, we drift away from target, which makes collisions unlikely.
    //      Interesting flying effect.
    //
    // Cons:
    //      Poor path preservation. It's like pulling on a string. This is because we aren't using
    //          integrating on the target's speed.
    //      It's impossible for the whole snake to fall into a hole because segments push themselves away
    //          from each other.
    //      Very small gravitational effect.
    private void FollowTarget3D() {
        // Vector3 horizDisplacement = new Vector3(
        //     target.transform.position.x - transform.position.x,
        //     0,
        //     target.transform.position.z - transform.position.z
        // );

        Vector3 horizDisplacement = target.transform.position - transform.position;

        bool isWithinFollowDist = horizDisplacement.magnitude <= followDistance;

        Vector3 horizFollowDisplacement = (horizDisplacement - horizDisplacement.normalized * followDistance);
        
        float horizFollowDist = horizFollowDisplacement.magnitude;

        float horizSpeed = (horizFollowDist <= 0.5f) ? (maxSpeed*horizFollowDist/0.5f): maxSpeed;
        
        Vector3 horizVelocity = horizFollowDisplacement.normalized * horizSpeed;

        rb.velocity = horizVelocity;

        // rb.velocity = new Vector3(
        //     horizVelocity.x,
        //     (rb.velocity.y+horizVelocity.y)*0.5f,
        //     horizVelocity.z
        // );
    }

    // Pros:
    //      Very stable due to gravitation towards the follow distance. So if we are less than follow
    //          distance, we drift away from target, which makes collisions unlikely.
    //
    // Cons:
    //      Poor path preservation. It's like pulling on a string. This is because we aren't using
    //          integrating on the target's speed.
    //      It's impossible for the whole snake to fall into a hole because segments push themselves away
    //          from each other on the horizontal plane.
    private void FollowTarget2D() {
        Vector3 horizDisplacement = new Vector3(
            target.transform.position.x - transform.position.x,
            0,
            target.transform.position.z - transform.position.z
        );

        bool isWithinFollowDist = horizDisplacement.magnitude <= followDistance;

        Vector3 horizFollowDisplacement = (horizDisplacement - horizDisplacement.normalized * followDistance);
        
        float horizFollowDist = horizFollowDisplacement.magnitude;

        float horizSpeed = (horizFollowDist <= 0.5f) ? (maxSpeed*horizFollowDist/0.5f): maxSpeed;
        
        Vector3 horizVelocity = horizFollowDisplacement.normalized * horizSpeed;

        rb.velocity = new Vector3(
            horizVelocity.x,
            rb.velocity.y,
            horizVelocity.z
        );
    }

    private void FollowTarget2DSpeedMatch() {
        Vector3 horizDisplacement = new Vector3(
            target.transform.position.x - transform.position.x,
            0,
            target.transform.position.z - transform.position.z
        );

        bool isWithinFollowDist = horizDisplacement.magnitude <= followDistance;

        Vector3 horizFollowDisplacement = (horizDisplacement - horizDisplacement.normalized * followDistance);
        
        float horizFollowDist = horizFollowDisplacement.magnitude;

        float horizSpeed = (horizFollowDist <= 0.5f) ?
            (maxSpeed*horizFollowDist/0.5f):
            (target.GetComponent<Rigidbody>().velocity.magnitude * horizDisplacement.magnitude / followDistance);

            // float speedMult = displacement.magnitude / followDistance;

            // float speed = target.GetComponent<Rigidbody>().velocity.magnitude * speedMult;

        
        Vector3 horizVelocity = horizFollowDisplacement.normalized * horizSpeed;

        rb.velocity = new Vector3(
            horizVelocity.x,
            rb.velocity.y,
            horizVelocity.z
        );
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

        // curMoveDirection = horizVel.normalized;
    }

    private void FollowTargetVert() {
        float dist = target.transform.position.y - transform.position.y;
        float distMag = Mathf.Abs(dist);
        float distDir = dist / distMag;

        if (distMag > followDistance) {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + distDir * followDistance,
                transform.position.z
            );

            rb.velocity = new Vector3(
                rb.velocity.x,
                0,
                rb.velocity.z
            );
        }
    }

    private void FollowTarget() {
        FollowTargetYCorrection();
        // FollowTarget2D();
        // FollowTargetVert();
        // FollowTarget2DSpeedMatch();

        // FollowTarget3D();
        CutIfTooFar();
    }




    private float FirstOrderLag(float val, float prevFilterVal, float k) {
        return (k*val) + (1.0f-k)*prevFilterVal;
    }
    private Vector3 FirstOrderLag3D(Vector3 vector, Vector3 prevFilterVector, float k) {
        Vector3 filterVector = new Vector3(
            FirstOrderLag(vector.x, prevFilterVector.x, k),
            FirstOrderLag(vector.y, prevFilterVector.y, k),
            FirstOrderLag(vector.z, prevFilterVector.z, k)
        );

        return filterVector;
    }



    private Vector3 prevTargetFilterPos;

    // Pros:
    //      Relatively good at preserving the shape of the path
    //
    // Cons:
    //      Instability due to collisions with target--need to slow down more quickly upon approach
    private void FollowTargetYCorrection() {
        prevTargetFilterPos = FirstOrderLag3D(target.transform.position, prevTargetFilterPos, 0.4f);

        Vector3 displacement = prevTargetFilterPos - transform.position;
        Vector3 horizDisplacement = displacement;
        horizDisplacement.y = 0.0f;

        if (displacement.magnitude > followDistance) {
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

        } else {
            rb.velocity *= 0.99f;

            // if (displacement.magnitude < followDistance || displacement.magnitude > 1.0f) {
            //     float horizVelFactor = ((displacement.magnitude - 1.0f) / (followDistance-1.0f));

            //     rb.velocity = new Vector3(
            //         displacement.normalized.x * horizVelFactor,
            //         rb.velocity.y,
            //         displacement.normalized.z * horizVelFactor
            //     );
            // }

        }

        float yDisp = displacement.y;

        float yDispMag = Mathf.Abs(yDisp);
        float yDispDir = yDisp / yDispMag;

        if (yDispMag > followDistance) {
            float targetSpeedY = Mathf.Abs(target.GetComponent<Rigidbody>().velocity.y);
            float ySpeed = targetSpeedY * 1.1f * ((yDispMag > 2.0f) ? 1.0f : (yDispMag - 1.0f));

            float curYSpeed = Mathf.Abs(rb.velocity.y);
            float curYVelDir = rb.velocity.y / curYSpeed;

            if ( ((curYVelDir * yDispDir) < 0.0f) || (curYSpeed < ySpeed)) {
                float yVel = ySpeed * yDispDir;
                rb.velocity = new Vector3(
                    rb.velocity.x,
                    yVel,
                    rb.velocity.z
                );
            }
        }
    }

    private void FollowTargetOrig() {
        Vector3 displacement = target.transform.position - transform.position;

        if (displacement.magnitude > followDistance) {
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

        } else {
            rb.velocity *= 0.99f;

            // if (displacement.magnitude < followDistance || displacement.magnitude > 1.0f) {
            //     float horizVelFactor = ((displacement.magnitude - 1.0f) / (followDistance-1.0f));

            //     rb.velocity = new Vector3(
            //         displacement.normalized.x * horizVelFactor,
            //         rb.velocity.y,
            //         displacement.normalized.z * horizVelFactor
            //     );
            // }

        }
    }

    void CutIfTooFar() {
        Vector3 displacement = target.transform.position - transform.position;
        Vector3 horizDisplacement = new Vector3(
            displacement.x,
            0,
            // displacement.y,
            displacement.z
        );

        float horizDist = horizDisplacement.magnitude;
        float vertDist = Mathf.Abs(displacement.y);

        if ((horizDist >= 6.0f) /*|| (vertDist >= 10.0f)*/) {
            // playerController.CutAtSegment(gameObject);
            if (Time.time - lastTimeWithinTargetRange > timeAwayFromTargetBeforeCut) {
                playerController.CutAtSegment(gameObject);
            }

        } else {
            lastTimeWithinTargetRange = Time.time;

        }

        // if (horizDisplacement.magnitude > followDistance * 1.5f) {
        //     if (Time.time - lastTimeWithinTargetRange > timeAwayFromTargetBeforeCut) {
        //         playerController.CutAtSegment(gameObject);
        //     }

        // } else {
        //     lastTimeWithinTargetRange = Time.time;
        // }

    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("EnemyWeapon")) {
            if (target) {
                playerController.CutAtSegment(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        GameObject collisionObject = collision.gameObject;
        if (weaponized) {
            if (collisionObject.CompareTag("SnakeBody")) {

                SnakeBodyController otherSnakeBodyController = collisionObject.GetComponent<SnakeBodyController>();

                if (otherSnakeBodyController.HasTarget()) {
                    playerController.CutAtSegment(collisionObject);
                }

            } else if (collisionObject.CompareTag("Enemy")) {
                Destroy(collisionObject);
            }
        // } else {
        //     if (collisionObject.CompareTag("SpikeBounceSurface")) {
        //         if (target) {
        //             playerController.CutAtSegment(gameObject);
        //         }
        //     }
        }
    }
}
