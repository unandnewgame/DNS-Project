using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("brightness", 1f);
        AudioListener.volume = volumeSlider.value;
    }

    public void OnVolumeChange()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void OnBrightnessChange()
    {
        RenderSettings.ambientLight = Color.white * brightnessSlider.value;
        PlayerPrefs.SetFloat("brightness", brightnessSlider.value);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
