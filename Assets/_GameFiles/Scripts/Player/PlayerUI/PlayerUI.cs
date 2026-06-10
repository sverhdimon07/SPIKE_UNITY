using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
public sealed class PlayerUI
{
    public static int Counter;

    public static UnityAction BossSpawned;

    private readonly Image _healthBar;
    private readonly Image _weaponLongRangeCooldownBar;
    private readonly TMP_Text _deathMessageText;
    private readonly TMP_Text _counterText;

    //private Coroutine _firstCoroutine;

    private readonly int _externalDataScale = 100;

    //private readonly float _fillSpeed = 0.1f;
    //private readonly float _delayBetweenSmoothRefreshStages = 0.005f;

    public PlayerUI(Image healthBar, Image weaponLongRangeCooldownBar, TMP_Text deathMessageText, TMP_Text counterText)
    {
        _healthBar = healthBar;
        _weaponLongRangeCooldownBar = weaponLongRangeCooldownBar;
        _deathMessageText = deathMessageText;
        _counterText = counterText;

        _counterText.text = 0.ToString();

        CharacterHealth.DiedSomeone += RefreshCounterText;
    }
    ~PlayerUI() 
    {
        CharacterHealth.DiedSomeone -= RefreshCounterText;
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

    public void RefreshCounterText()
    {
        if (CharacterControllerNewBoss.Spawned == true)
        {
            Debug.Log(_counterText.text);
            return;
        }
        int counter = int.Parse(_counterText.text);
        counter += 1;
        Counter = counter;

        _counterText.text = counter.ToString();

        if (_counterText.text == "3")
        {
            BossSpawned.Invoke();

            CharacterControllerNewBoss.Spawned = true;
        }
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
