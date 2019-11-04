using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMenuController : MonoBehaviour
{
    private bool ended = false;
    private GameObject endMenuContent;

    // Start is called before the first frame update
    void Start()
    {
        endMenuContent = gameObject.transform.GetChild(0).gameObject;
        endMenuContent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EndGame(int segmentCount) {
        endMenuContent.SetActive(true);
        Time.timeScale = 0;
    }
}
