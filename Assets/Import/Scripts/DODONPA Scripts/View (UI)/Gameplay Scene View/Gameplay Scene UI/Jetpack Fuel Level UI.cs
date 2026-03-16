using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JetpackFuelLevelUI : MonoBehaviour
{
    [SerializeField] Image bar;

    private Coroutine firstCoroutine;
    
    private readonly int externalDataScale = 200;

    private readonly float fillSpeed = 0.04f;
    private readonly float delayBetweenSmoothRefreshStages = 0.005f;

    private void OnEnable()
    {
        //ViewEventBus.JetpackFuelLevelChanged += Refresh;
    }
    
    private void OnDisable()
    {
        //ViewEventBus.JetpackFuelLevelChanged -= Refresh;
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
