using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public sealed class PlayerUI
{
    private Image _healthBar;
    private Image _weaponLongRangeCooldownBar;
    private TMP_Text _deathMessageText;

    //private Coroutine _firstCoroutine;

    private readonly int _externalDataScale = 100;

    //private readonly float _fillSpeed = 0.1f;
    //private readonly float _delayBetweenSmoothRefreshStages = 0.005f;

    public void Initialize(Image healthBar, Image weaponLongRangeCooldownBar, TMP_Text deathMessageText)
    {
        _healthBar = healthBar;
        _weaponLongRangeCooldownBar = weaponLongRangeCooldownBar;
        _deathMessageText = deathMessageText;
    }

    public void RefreshHealthBar(float valueLevel)
    {
        float barFullness = valueLevel / _externalDataScale;

        _healthBar.fillAmount = barFullness;
    }

    public void RefreshWeaponLongRangeCooldownBar()
    {
        if (_weaponLongRangeCooldownBar.fillAmount == 1f)
        {
            _weaponLongRangeCooldownBar.fillAmount = 0f;

            WeaponLongRangeCooldownBarCoroutine();
        }
    }

    public void RefreshDeathMessageText()
    {
        if (_deathMessageText.enabled == true)
        {
            _deathMessageText.enabled = false;
        }
        else if (_deathMessageText.enabled == false)
        {
            _deathMessageText.enabled = true;
        }
    }

    private async Task WeaponLongRangeCooldownBarCoroutine()
    {
        //yield return new WaitForSeconds(2.34f);
        await Task.Delay(2340);

        _weaponLongRangeCooldownBar.fillAmount = 1f;
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
