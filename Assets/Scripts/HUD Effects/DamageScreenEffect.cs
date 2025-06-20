using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DamageScreenEffects : MonoBehaviour
{
    public static DamageScreenEffects instance;
    [SerializeField] float timeToSideDisplay = 1.4f;

    [Space]
    [SerializeField, Range(0, 100)] float hpToMinColor;
    [SerializeField, Range(0, 100)] float hpToMaxColor;

    [Header("Blood")]
    [SerializeField] float timeToStartHideBlood = 1;
    [SerializeField] float timeToHideBlood = 0.4f;
    [Space]
    [SerializeField] Color bloodMinColor;
    [SerializeField] Color bloodMaxColor;



    [Header("Blood Texture")]


    [Header("Blood")]
    [SerializeField] float timeToStartHideTexture = 1;
    [SerializeField] float timeToHideTexture = 0.4f;

    [SerializeField, Range(0, 100)] float hpToMinColorTexture;
    [SerializeField, Range(0, 100)] float hpToMaxColorTexture;

    [Space]
    [SerializeField] Color textureMinColor;
    [SerializeField] Color textureMaxColor;


    [Space]
    [SerializeField] Image bloodEffectTexture;
    [SerializeField] Image redVinete;

    [Space]


    Coroutine hideBloodColor;
    Coroutine hideBloodTexture;


    [SerializeField] float testHp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TakeHit(float _hp)
    {
        float _colorIntensity = Mathf.InverseLerp(hpToMinColor, hpToMaxColor, _hp);
        if (_colorIntensity > 0)
        {
            if (hideBloodColor != null)
                StopCoroutine(hideBloodColor);

            SetBloodColorIntensity(_colorIntensity);

            if (_colorIntensity > 0.75)
            {
                hideBloodColor = StartCoroutine(HideBlood(timeToStartHideBlood * 2, _colorIntensity));
            }
            else
            {
                hideBloodColor = StartCoroutine(HideBlood(timeToStartHideBlood, _colorIntensity));
            }
        }


        float _textureIntensity = Mathf.InverseLerp(hpToMinColorTexture, hpToMaxColorTexture, _hp);


        if (_textureIntensity > 0)
        {
            if (hideBloodTexture != null)
                StopCoroutine(hideBloodTexture);

            SetTexturebloodIntensity(_textureIntensity);

            if (_colorIntensity > 0.75)
            {
                hideBloodTexture = StartCoroutine(HideBloodTexture(timeToStartHideTexture * 2, _textureIntensity));
            }
            else
            {
                hideBloodTexture = StartCoroutine(HideBloodTexture(timeToStartHideTexture, _textureIntensity));
            }
        }

    }


    void SetBloodColorIntensity(float _intensity)
    {
        redVinete.color = Color.Lerp(bloodMinColor, bloodMaxColor, _intensity);
    }


    IEnumerator HideBlood(float _timeToStart, float _startIntensity)
    {
        float _curTime = timeToHideBlood;
        yield return new WaitForSeconds(_timeToStart);

        while (true)
        {
            _curTime -= Time.deltaTime;

            float _curIntesity = Mathf.Lerp(0, _startIntensity, _curTime / timeToHideBlood);

            SetBloodColorIntensity(_curIntesity);

            if (_curTime <= 0)
                break;

            yield return new WaitForEndOfFrame();
        }
    }


    #region blood Texture

    void SetTexturebloodIntensity(float _intensity)
    {
        bloodEffectTexture.color = Color.Lerp(textureMinColor, textureMaxColor, _intensity);
    }

    IEnumerator HideBloodTexture(float _timeToStart, float _startIntensity)
    {
        float _curTime = timeToHideTexture;
        yield return new WaitForSeconds(_timeToStart);

        while (true)
        {
            _curTime -= Time.deltaTime;

            float _curIntesity = Mathf.Lerp(0, _startIntensity, _curTime / timeToHideTexture);

            SetTexturebloodIntensity(_curIntesity);

            if (_curTime <= 0)
                break;

            yield return new WaitForEndOfFrame();
        }
    }

    #endregion
}
