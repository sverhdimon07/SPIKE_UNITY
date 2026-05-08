using UnityEngine;

public sealed class PlayerRotation
{
    public void Initialize() { } //
    
    public void RotateWithinFrame(Transform renderAndSkeletonPoint, Vector3 requiredActualDirection)
    {
        renderAndSkeletonPoint.rotation = Quaternion.LookRotation(requiredActualDirection); //ядекюрэ онбнпнрш окюбмшлх
    }
}
