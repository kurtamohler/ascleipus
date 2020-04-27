using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySpikesController : MonoBehaviour
{
    public GameObject spikes;
    public float extendSize;
    float extendRate = 16f;
    float pauseTime = 0.5f;

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

    float extendNextTime = 0;
    bool watchForNextExtend = false;
    float extendNextDelay = 0.10f;

    LinkedListNode<GameObject> nextBodySegmentNode = null;

    // Start is called before the first frame update
    void Start()
    {
        origScale = spikes.transform.localScale;
        origScaleMag = origScale.magnitude;
        curScaleMag = origScaleMag;
        targetScale = origScale * extendSize;
        targetScaleMag = curScaleMag * extendSize;

        extendMult = 1.0f + extendRate;
        contractMult = 1.0f / extendMult;
        spikes.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isExtending) {
            Extend();
        } else if (isContracting) {
            Contract();
        }
        
        /*
        if (extendNextCounter > 0) {
            extendNextCounter -= Time.deltaTime;
            if (extendNextCounter <= 0) {
                ExtendNextSegment();
            }
        }   
        */
        
        if (watchForNextExtend) {
            if (Time.time >= extendNextTime) {
                ExtendNextSegment(extendNextTime+extendNextDelay);
                watchForNextExtend = false;
            }
        }
        
    }

    void Extend() {
        // float mult = extendMult * Time.deltaTime;
        float mult = 1.0f + (Time.deltaTime * extendRate);

        spikes.transform.localScale *= mult;
        curScaleMag *= mult;

        if (curScaleMag >= targetScaleMag) {
            //ExtendNextSegment();
            // Since we may have overshot the target, reset to equal the target
            spikes.transform.localScale = targetScale;
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

            spikes.transform.localScale *= mult;
            curScaleMag *= mult;

            if (curScaleMag <= origScaleMag) {
                spikes.transform.localScale = origScale;
                isContracting = false;
                isExtending = false;
                spikes.SetActive(false);
            }
        }
    }

    void ExtendNextSegment(float extendNextTime) {
        if (nextBodySegmentNode != null) {
            LinkedListNode<GameObject> nextNextBodySegmentNode = nextBodySegmentNode.Next;

            nextBodySegmentNode.Value.GetComponent<BodySpikesController>().StartExtending(nextNextBodySegmentNode);
        }
    }

    public void StartExtending(LinkedListNode<GameObject> nextSpikeControllerNode = null, float extendNextTime = 0) {
        // extendNextCounter = 0.15f;
        this.nextBodySegmentNode = nextSpikeControllerNode;
        spikes.SetActive(true);
        isExtending = true;
        isContracting = false;

        if (nextBodySegmentNode != null) {
            if (extendNextTime == 0) {
                this.extendNextTime = Time.time + extendNextDelay;
            } else {
                this.extendNextTime = extendNextTime;
            }
            this.watchForNextExtend = true;
        }

    }

    public bool IsExtended() {
        return isContracting || isExtending;
    }

}
