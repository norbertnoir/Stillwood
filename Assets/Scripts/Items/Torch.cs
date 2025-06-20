using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] GameObject light;
    [SerializeField] ParticleSystem particle;

    public float fuel = 1;

    public float degradationTime = 15;

    bool isActive = true;

    public void SetActive(bool value)
    {
        isActive = value;
        light.SetActive(isActive);

        var main = particle.main;

        main.loop = isActive;

        if (isActive)
        {
            particle.Play();
        }
    }


    private void Update()
    {
        UpdateFuel();
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
