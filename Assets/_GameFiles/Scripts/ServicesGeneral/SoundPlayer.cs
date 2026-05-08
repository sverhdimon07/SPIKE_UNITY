using UnityEngine;

public class SoundPlayer : MonoBehaviour //не sealed
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); //надо понять, как оставлять сцены в памяти и перманентно их не удалять, чтобы у нас постоянно не инитились сценные системы, чтобы они проинитились в самом начале и оставались на протяжении всего нахождения игрока в приложении;
    }
}
