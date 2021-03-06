﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    private Combat combat;
    private RectTransform greenBar;

    void Awake()
    {
        combat = GetComponent<Combat>();
        greenBar = transform.Find("Canvas").GetChild(2).GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        //Activate Canvas object
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void Update()
    {
        greenBar.localScale = new Vector3(combat.health/(float)combat.maxHealth, 1, 1);
    }
}
