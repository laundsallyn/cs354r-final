using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class CheckSetting : MonoBehaviour
{
    public AudioMixer masterMixer;
    void Awake()
    {
        if (PlayerPrefs.GetInt("BGM") == 1)
        {
            Camera.main.GetComponent<AudioSource>().Play();
            // AudioListener.pause = false;
        }
        else
        {
            Camera.main.GetComponent<AudioSource>().Pause();
            // AudioListener.pause = true;
        }

        if(PlayerPrefs.GetInt("SFX")==1)
        {
            masterMixer.SetFloat("sfxVol",0.0f);
        }
        else
        {
            masterMixer.SetFloat("sfxVol",-80.0f);
        }
    }

}
