using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private Quaternion initRotation;


    public void SetHealth(float health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.value);
        
    }

    private void Start()
    {
        initRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = initRotation;
    }

}
