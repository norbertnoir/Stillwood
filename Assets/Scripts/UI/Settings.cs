using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public int musicVolume = 70;
    public int soundVolume = 70;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
    }
}
