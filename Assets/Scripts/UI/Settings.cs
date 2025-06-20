using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public int musicVolume = 70;
    public int soundVolume = 70;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
