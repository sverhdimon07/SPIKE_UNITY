using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class PlayerUI
{
    //public static int Counter;

    private readonly Image _healthBar;
    private readonly Image _weaponLongRangeCooldownBar;
    private readonly TMP_Text _deathMessageText;
    private readonly TMP_Text _counterText;
    
    private readonly int _externalDataScale = 100;

    public PlayerUI(Image healthBar, Image weaponLongRangeCooldownBar, TMP_Text deathMessageText, TMP_Text counterText)
    {
        _healthBar = healthBar;
        _weaponLongRangeCooldownBar = weaponLongRangeCooldownBar;
        _deathMessageText = deathMessageText;
        _counterText = counterText;
        _counterText.text = 0.ToString();
    }

    public void RefreshHealthBar(float valueLevel)
    {
        float barFullness = valueLevel / _externalDataScale;

        _healthBar.fillAmount = barFullness;
    }

    public void RefreshWeaponLongRangeCooldownBarOnEmpty()
    {
        _weaponLongRangeCooldownBar.fillAmount = 0f;
    }

    public async Task RefreshWeaponLongRangeCooldownBarOnFull()
    {
        await Task.Delay(600);

        _weaponLongRangeCooldownBar.fillAmount = 1f;
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

    public void RefreshCounterText()
    {
        /*
        if (CharacterControllerNewBoss.Spawned == true)
        {
            Debug.Log(_counterText.text);
            return;
        }*/
        int counter = int.Parse(_counterText.text);
        counter += 1;
        //Counter = counter;

        _counterText.text = counter.ToString();

        /*
        if (_counterText.text == "3")
        {
            BossSpawned.Invoke();

            //CharacterControllerNewBoss.Spawned = true;
        }*/
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
