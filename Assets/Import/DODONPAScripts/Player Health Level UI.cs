using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthLevelUI : MonoBehaviour
{
    [SerializeField] Image bar;

    private Coroutine firstCoroutine;

    private readonly int externalDataScale = 100;

    private readonly float fillSpeed = 0.1f;
    private readonly float delayBetweenSmoothRefreshStages = 0.005f;

    private void OnEnable()
    {
        //ViewEventBus.PlayerHealthLevelChanged += Refresh;
    }

    private void OnDisable()
    {
        //ViewEventBus.PlayerHealthLevelChanged -= Refresh;
    }

    private void StartSmoothRefreshCoroutine(float fuelLevel)
    {
        firstCoroutine = StartCoroutine(RefreshSmoothly(fuelLevel));
    }

    private void Refresh(float fuelLevel)
    {
        StopAllCoroutines();
        StartSmoothRefreshCoroutine(fuelLevel);
    }

    private IEnumerator RefreshSmoothly(float fuelLevel)
    {
        float barFullness = fuelLevel / externalDataScale;

        while (bar.fillAmount != barFullness)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, barFullness, fillSpeed);

            yield return new WaitForSeconds(delayBetweenSmoothRefreshStages);
        }
    }
}
