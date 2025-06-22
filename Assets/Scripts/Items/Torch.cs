using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] GameObject light;
    [SerializeField] ParticleSystem particle;
    [SerializeField] GameObject torch;
    [SerializeField] Flags flags;
    [SerializeField] Light torchLight;

    public float fuel = 1;

    public float degradationTime = 15;

    bool isActive = true;
    bool isGathered = true;

    public void SetActive(bool value)
    {
        isActive = value;
        light.SetActive(isActive);
        fuel = 1;

        var main = particle.main;

        main.loop = isActive;

        if (isActive)
        {
            particle.Play();
        }
    }


    private void Update()
    {
        if (flags.isTorch == false)
        {
            torch.SetActive(false);
            isGathered = true;
            return;
        }
        else
        {
            torch.SetActive(true);
            if (isGathered)
            {
                isGathered = false;
                SetActive(true);
            }
        }
        if (!isActive)
            return;
        UpdateFuel();
        torchLight.intensity = Mathf.Max(fuel, 0.3f);
    }


    private void UpdateFuel()
    {
        if (fuel >= 0)
        {
            fuel -= (1 / degradationTime) * Time.deltaTime;
            if(isActive != true)
                SetActive(true);
        }
        else
        {
            if(isActive != false)
                SetActive(false);
        }

    }

    public void AddFuel(float amount = 0.25f)
    {
        fuel += amount;
    }

}
