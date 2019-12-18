using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBounceSurfaceController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision) {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject.CompareTag("SnakeBodySpikes")) {
            Debug.Log("HEY!");
        }
    }

    void ApplyBounceToRigidBody(Rigidbody rb, Vector3 direction) {
        Vector3 newVelocity = rb.velocity;
        newVelocity.y = 0;
        rb.velocity = newVelocity;

        Vector3 force = direction * 200.0f;
        
        Debug.Log("Applying force: " + force);
        rb.AddForce(force, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("SnakeBodySpikes")) {
            ApplyBounceToRigidBody(collider.GetComponentInParent<Rigidbody>(), Vector3.up);
        }
    }
}
