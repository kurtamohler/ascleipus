using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalPointController : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player) {
            transform.position = player.transform.position;
        }
    }    // Start is called before the first frame update
}
