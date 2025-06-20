using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Leszy : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Camera mainCamera;
    [SerializeField] float minDistance = 50f;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float stayTime = 10f;
    [SerializeField] float stayTimeMax = 60f;
    [SerializeField] float lookTimeTreshold = 2f;
    [SerializeField] float delayMin = 0.2f;
    [SerializeField] float delayMax = 0.5f;

    [SerializeField] LayerMask groundLayerMask;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;


    private float stayTimeCounter;
    private float lookTimeCounter;
    private bool isActive = false;

    private void Start()
    {
        GameObject ddol = GameObject.FindGameObjectWithTag("DontDestroyOnLoad");
        if (ddol != null)
        {
            Settings settings = ddol.GetComponent<Settings>();
            if (settings != null)
            {
                audioSource.volume = settings.soundVolume / 100f;
            }
        }
        StartCoroutine(cycle());
        gameObject.transform.position = new Vector3(0, -200, 0);
    }

    private IEnumerator cycle()
    {
        while (true)
        {
            Vector3 spawnPos = Vector3.zero;
            bool isValidPosition = false;
            int attempts = 0;
            stayTimeCounter = 0f;
            lookTimeCounter = 0f;


            while (!isValidPosition && (attempts < 20))
            {
                attempts++;
                Vector2 circle = Random.insideUnitCircle * Random.Range(minDistance, maxDistance);
                Vector3 position = new Vector3(circle.x, 800, circle.y) + player.position;

                Debug.DrawRay(position, Vector3.down * 1000, Color.red, 10f);
                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 10000, groundLayerMask))
                {
                    position.y = hit.point.y;
                    spawnPos = position;
                    isValidPosition = true;
                }
            }

            if (!isValidPosition)
            {
                Debug.LogWarning("Failed to find a valid spawn position for Leszy.");
                yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                continue;
            }
            Debug.Log("Leszy spawned at: " + spawnPos);

            transform.position = spawnPos;
            isActive = true;
            stayTimeCounter = Random.Range(stayTime, stayTimeMax);
            lookTimeCounter = 0f;
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.Play();

            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Ignore vertical component
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
            while (stayTimeCounter <  stayTime && (lookTimeCounter < lookTimeTreshold))
            {
                stayTimeCounter += Time.deltaTime;
                if (IsPlayerLookingAtLeszy())
                {
                    lookTimeCounter += Time.deltaTime;
                }
                else
                {
                    lookTimeCounter = 0f; // Reset if player is not looking
                }

                yield return null;
            }
            Debug.Log("Leszy finished stay time or player stopped looking.");

            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            gameObject.transform.position = new Vector3(0, -200, 0);
            isActive = false;
            yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
        }
    }

    bool IsPlayerLookingAtLeszy()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, mainCamera.transform.forward);
        if (angle < 30f) // Adjust the angle threshold as needed
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
