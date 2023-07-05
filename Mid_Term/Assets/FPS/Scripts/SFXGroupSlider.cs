using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXGroupSlider : MonoBehaviour
{

    public AudioMixer audioMixer; // Reference to the AudioMixer script
    public Slider slider;
    public int index;


    private void Start()
    {
        
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        audioMixer.sfxGroup[index] = value;
    }
}
