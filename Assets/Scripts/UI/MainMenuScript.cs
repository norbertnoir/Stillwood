using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;

    private void Start()
    {
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        soundVolumeSlider.onValueChanged.AddListener(SetSoundVolume);
    }

    public void ChangeScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

        // If running in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetMusicVolume(float volume)
    {
        GameObject ddol = GameObject.FindGameObjectWithTag("DontDestroyOnLoad");
        if (ddol != null)
        {
            Settings settings = ddol.GetComponent<Settings>();
            if (settings != null)
            {
                settings.musicVolume = (int)volume;
            }
        }
    }

    public void SetSoundVolume(float volume)
    {
        GameObject ddol = GameObject.FindGameObjectWithTag("DontDestroyOnLoad");
        if (ddol != null)
        {
            Settings settings = ddol.GetComponent<Settings>();
            if (settings != null)
            {
                settings.soundVolume = (int)volume;
            }
        }
    }



}
