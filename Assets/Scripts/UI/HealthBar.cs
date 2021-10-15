using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Vector3 offset;

    
    public void SetHealth(float health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.value);
        
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position + offset;
    }

    public void Disable()
    {
        GetComponentInParent<Transform>().gameObject.SetActive(false);
    }
    public void Enable()
    {
        GetComponentInParent<Transform>().gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

}
