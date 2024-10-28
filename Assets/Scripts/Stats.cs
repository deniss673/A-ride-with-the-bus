using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    Slider staminaBar;

    void Start()
    {
        staminaBar = GetComponentInChildren<Slider>();
    }

    void Update()
    {
        if (staminaBar == null)
        {
            staminaBar = GetComponentInChildren<Slider>();
        }
        
    }

    public void ChangeSlider(float value)
    {
        staminaBar.value = value;
    }
}
