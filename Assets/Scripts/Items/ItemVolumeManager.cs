using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVolumeManager : MonoBehaviour
{
    public static ItemVolumeManager instance;
    [SerializeField] List<GameObject> itemsVolume;
    [SerializeField] List<float> volumeTimes;

    private void Awake()
    {
        if(ItemVolumeManager.instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        volumeTimes = new List<float>(new float[itemsVolume.Count]);
    }

    private void Update()
    {
        for (int i = 0; i < volumeTimes.Count; i++)
        {
            if (volumeTimes[i] > 0)
            {
                volumeTimes[i] -= Time.deltaTime;
                if (volumeTimes[i] <= 0f)
                {
                    itemsVolume[i].SetActive(false);
                }
            }
        }
    }

    public void StartVolume(int itemId, float duration)
    {
        if (itemId < 0 || itemId >= itemsVolume.Count) return;

        volumeTimes[itemId] = duration;
        itemsVolume[itemId].SetActive(true);
    }
}