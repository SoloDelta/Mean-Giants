using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXGroupSlider : MonoBehaviour
{   

    public AudioMixer audioMixer; 
    public Slider slider;
    public int index;


    private void Start()
    {
        
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        
    }
}
