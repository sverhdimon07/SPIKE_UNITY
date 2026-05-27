using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimelineEvents : MonoBehaviour
{
    [SerializeField] private CharacterAI characterAI;
    //[SerializeField] private CharacterAnimator characterAnimator;

    [SerializeField] private float fisrtDelayTime = 24.5f;
    [SerializeField] private float secondDelayTime = 5f;
    [SerializeField] private float thirdDelayTime = 0f;
    [SerializeField] private float fourthDelayTime = 27f;
    [SerializeField] private float fivthDelayTime = 39f;
    [SerializeField] private float sixthDelayTime;
    [SerializeField] private float seventhDelayTime;
    [SerializeField] private float eighthDelayTime;
    [SerializeField] private float ninethDelayTime;
    [SerializeField] private float tenthDelayTime;

    [SerializeField] private UnityEvent OnSceneStarting;
    [SerializeField] private UnityEvent OnFirstDelayEnding;
    [SerializeField] private UnityEvent OnSecondDelayEnding;
    [SerializeField] private UnityEvent OnThirdDelayEnding;
    [SerializeField] private UnityEvent OnFourthDelayEnding;
    [SerializeField] private UnityEvent OnFivthDelayEnding;
    [SerializeField] private UnityEvent OnSixthDelayEnding;
    [SerializeField] private UnityEvent OnSeventhDelayEnding;
    [SerializeField] private UnityEvent OnEighthDelayEnding;
    [SerializeField] private UnityEvent OnNinethDelayEnding;
    [SerializeField] private UnityEvent OnTenthDelayEnding;

    private void Awake()
    {
        OnSceneStarting?.Invoke();

        StartCoroutine(FirstDelayCoroutine());
        StartCoroutine(SecondDelayCoroutine());
        StartCoroutine(ThirdDelayCoroutine());
        StartCoroutine(FourthDelayCoroutine());
        StartCoroutine(FivthDelayCoroutine());
        StartCoroutine(SixthDelayCoroutine());
        StartCoroutine(SeventhDelayCoroutine());
        StartCoroutine(EighthDelayCoroutine());
        StartCoroutine(NinethDelayCoroutine());
        StartCoroutine(TenthDelayCoroutine());

        characterAI.DisableAI();
    }

    IEnumerator FirstDelayCoroutine()
    {
        yield return new WaitForSeconds(fisrtDelayTime);

        OnFirstDelayEnding?.Invoke();
    }

    IEnumerator SecondDelayCoroutine()
    {
        yield return new WaitForSeconds(secondDelayTime);

        OnSecondDelayEnding?.Invoke();
    }

    IEnumerator ThirdDelayCoroutine()
    {
        yield return new WaitForSeconds(thirdDelayTime);

        OnThirdDelayEnding?.Invoke();
    }

    IEnumerator FourthDelayCoroutine()
    {
        yield return new WaitForSeconds(fourthDelayTime);

        OnFourthDelayEnding?.Invoke();
    }

    IEnumerator FivthDelayCoroutine()
    {
        yield return new WaitForSeconds(fivthDelayTime);

        OnFivthDelayEnding?.Invoke();
    }

    IEnumerator SixthDelayCoroutine()
    {
        yield return new WaitForSeconds(sixthDelayTime);

        OnSixthDelayEnding?.Invoke();
    }

    IEnumerator SeventhDelayCoroutine()
    {
        yield return new WaitForSeconds(seventhDelayTime);

        OnSeventhDelayEnding?.Invoke();
    }

    IEnumerator EighthDelayCoroutine()
    {
        yield return new WaitForSeconds(eighthDelayTime);

        OnEighthDelayEnding?.Invoke();
    }

    IEnumerator NinethDelayCoroutine()
    {
        yield return new WaitForSeconds(ninethDelayTime);

        OnNinethDelayEnding?.Invoke();
    }

    IEnumerator TenthDelayCoroutine()
    {
        yield return new WaitForSeconds(tenthDelayTime);

        OnTenthDelayEnding?.Invoke();
    }
}
