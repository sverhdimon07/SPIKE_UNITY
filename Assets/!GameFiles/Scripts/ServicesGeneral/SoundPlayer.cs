using UnityEngine;

public class SoundPlayer : MonoBehaviour //не sealed
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
