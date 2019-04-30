using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
    float volume;
    // Use this for initialization
    void Start()
    {
        slider = GameObject.FindGameObjectWithTag("SetVolume").GetComponent<Slider>();
        if (PlayerPrefs.HasKey("Music"))
        {
            volume = PlayerPrefs.GetFloat("Music");
            slider.value = volume;
        }
    }

    //Play Global
    private static MusicManager instance = null;
    public static MusicManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    //Play Gobal End
    

    public void SetLevel(float sliderValue)
    {
        volume = slider.value;
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Music", volume);
    }
}
