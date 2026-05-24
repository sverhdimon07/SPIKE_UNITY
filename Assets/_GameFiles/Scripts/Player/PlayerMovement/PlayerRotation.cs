using UnityEngine;
using UnityEngine.Events;

public sealed class PlayerRotation
{
    /*
    private Quaternion _lastRotation; //дкъ окюбмшу онбнпнрнб

    public PlayerRotation(Quaternion lastRotation)
    {
        _lastRotation = lastRotation;
    }*/

    public static UnityAction<Quaternion> Rotated;

    public void Rotate(Vector3 direction) //хмйюосякъжхъ
    {
        Quaternion nextRotation = Quaternion.LookRotation(direction); //ядекюрэ онбнпнрш окюбмшлх

        Rotated(nextRotation);
    }
}
