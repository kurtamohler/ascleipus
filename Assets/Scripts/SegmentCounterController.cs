﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SegmentCounterController : MonoBehaviour
{
    private int count;
    private int totalPickups;
    private TextMeshProUGUI text;


    // Start is called before the first frame update
    void Start()
    {
        //totalPickups = GameObject.FindWithTag("FoodPickup").;
        totalPickups = GameObject.FindGameObjectsWithTag("FoodPickup").Length;
        text = GetComponent<TextMeshProUGUI>();
        UpdateCount(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCount(int newCount) {
        count = newCount;
        UpdateText();
    }

    private void UpdateText() {
        text.SetText("length: " + count + " / " + totalPickups);
    }
}
