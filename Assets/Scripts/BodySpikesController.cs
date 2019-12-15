using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySpikesController : MonoBehaviour
{
    public float extendSize;
    public float extendRate;
    public float pauseTime;

    Vector3 origScale;
    Vector3 targetScale;
    float origScaleMag;
    float targetScaleMag;
    float curScaleMag;

    float extendMult;
    float contractMult;

    bool isExtending = false;
    bool isContracting = false;

    float pauseCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        origScale = gameObject.transform.localScale;
        origScaleMag = origScale.magnitude;
        curScaleMag = origScaleMag;
        targetScale = origScale * extendSize;
        targetScaleMag = curScaleMag * extendSize;

        extendMult = 1.0f + extendRate;
        contractMult = 1.0f / extendMult;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExtending) {
            Extend();
        } else if (isContracting) {
            Contract();
        }
        
    }

    void Extend() {
        // float mult = extendMult * Time.deltaTime;
        float mult = 1.0f + (Time.deltaTime * extendRate);

        transform.localScale *= mult;
        curScaleMag *= mult;

        if (curScaleMag >= targetScaleMag) {
            // Since we may have overshot the target, reset to equal the target
            transform.localScale = targetScale;
            curScaleMag = targetScaleMag;

            isContracting = true;
            isExtending = false;
            pauseCounter = pauseTime;
        }
    }

    void Contract() {
        // Wait for time to count down before contracting
        if (pauseCounter > 0.0f) {
            pauseCounter -= Time.deltaTime;
        } else {
            float mult = 1.0f / (1.0f + (Time.deltaTime * extendRate));

            transform.localScale *= mult;
            curScaleMag *= mult;

            if (curScaleMag <= origScaleMag) {
                transform.localScale = origScale;
                isContracting = false;
                isExtending = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void StartExtending() {
        isExtending = true;
        isContracting = false;
    }
}
