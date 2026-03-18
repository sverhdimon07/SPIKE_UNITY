//using System.Collections;
//using UnityEngine;
using UnityEngine.UI;

public class CharacterUI
{
    private Image _bar;

    //private Coroutine _firstCoroutine;

    private readonly int _externalDataScale = 100;

    //private readonly float _fillSpeed = 0.1f;
    //private readonly float _delayBetweenSmoothRefreshStages = 0.005f;

    public void Initialize(Image bar)
    {
        _bar = bar;
    }

    public void Refresh(float valueLevel)
    {
        float barFullness = valueLevel / _externalDataScale;

        _bar.fillAmount = barFullness;
    }

    /*
    private void RefreshSmoothly(float valueLevel)
    {
        StopAllCoroutines();
        StartRefreshSmoothlyCoroutine(valueLevel);
    }

    private void StartRefreshSmoothlyCoroutine(float valueLevel)
    {
        _firstCoroutine = StartCoroutine(RefreshSmoothlyCoroutine(valueLevel));
    }

    private IEnumerator RefreshSmoothlyCoroutine(float valueLevel)
    {
        float barFullness = valueLevel / _externalDataScale;

        while (_bar.fillAmount != barFullness)
        {
            _bar.fillAmount = Mathf.Lerp(_bar.fillAmount, barFullness, _fillSpeed);

            yield return new WaitForSeconds(_delayBetweenSmoothRefreshStages);
        }
    }*/
}
