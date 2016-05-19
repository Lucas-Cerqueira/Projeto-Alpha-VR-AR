using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    private Combat combat;
    private RectTransform greenBar;

    void Awake()
    {
        combat = GetComponent<Combat>();
        greenBar = transform.GetChild(0).GetChild(2).GetComponent<RectTransform>();
    }

    void Update()
    {
        greenBar.localScale = new Vector3(combat.health/(float)combat.maxHealth, 1, 1);
    }
}
