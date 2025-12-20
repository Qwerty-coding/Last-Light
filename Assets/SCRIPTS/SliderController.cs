using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderController : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI sliderText= null;

    [SerializeField] private float maxSliderAmount = 100.0f;

    public void SliderChange(float value)
    {
        float LocalValue = value*maxSliderAmount;
        sliderText.text=LocalValue.ToString("0");

    }


}
