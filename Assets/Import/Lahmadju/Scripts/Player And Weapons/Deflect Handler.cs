using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeflectHandler : MonoBehaviour
{
    private const string BOSS_SWORD_TAG = "BossSword";

    private bool handlerActivated = false;
    private bool secondDefectHandler = true;

    [SerializeField] private UnityEvent playerBlockActivated;
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag(BOSS_SWORD_TAG))
        {
            return;
        }
        if (secondDefectHandler == true)
        {
            if (handlerActivated == true)
            {
                playerBlockActivated.Invoke();
                secondDefectHandler = false;
                Invoke(nameof(MakeSecondDefectHandlerTrue), 1f);
            }
        }

    }
    public void MakeHandlerAcivatedTrue()
    {
        handlerActivated = true;
    }
    public void MakeHandlerAcivatedFalse()
    {
        handlerActivated = false;
    }
    private void MakeSecondDefectHandlerTrue()
    {
        secondDefectHandler = true;
    }
}