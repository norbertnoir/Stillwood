using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundAreaPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.clip = audioClip[Random.Range(0, audioClip.Length)];
            GameObject ddol = GameObject.FindGameObjectWithTag("DontDestroyOnLoad");
            if (ddol != null)
            {
                Settings settings = ddol.GetComponent<Settings>();
                if (settings != null)
                {
                    audioSource.volume = settings.soundVolume / 100f;
                }
            }
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }
}
