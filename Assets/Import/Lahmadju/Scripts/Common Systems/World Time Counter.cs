using UnityEngine;
using UnityEngine.Events;

public class WorldTimeCounter : MonoBehaviour
{
    private float worldTimeCounter = 0f;
    private float indicator = 0f;

    private int seconds = 0;
    private int minutes = 0;

    [SerializeField] private UnityEvent<string> timeCounterChanged;
    private void Start()
    {
        enabled = false;
    }
    private void Update()
    {
        worldTimeCounter += Time.deltaTime;
        if (worldTimeCounter - 1f >= indicator)
        {
            indicator += 1f;
            seconds = (int)worldTimeCounter;
            if (seconds < 10)
            {
                Debug.Log(minutes + ":0" + seconds);
                timeCounterChanged.Invoke(minutes + ":0" + seconds);
                return;
            }
            if (seconds == 60)
            {
                minutes += 1;
                seconds = 0;
                indicator = 0f;
                worldTimeCounter = 0f;
                Debug.Log(minutes + ":0" + seconds);
                timeCounterChanged.Invoke(minutes + ":0" + seconds);
                return;
            }
            Debug.Log(minutes + ":" + seconds);
            timeCounterChanged.Invoke(minutes + ":" + seconds);
        }
    }
    public void StopTimeCount()
    {
        enabled = false;
    }
}