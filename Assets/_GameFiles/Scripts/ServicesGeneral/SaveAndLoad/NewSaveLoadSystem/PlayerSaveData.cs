using UnityEngine;

[System.Serializable]
public class PlayerSaveData //хороший вопрос - как называть
{
    public Vector3 Position;

    public Quaternion Rotation;

    public float Health;

    public int Score;
    public Quaternion FormRotation;
    public bool IsFighter;
    public bool IsFlightLocked;
    public bool IsFalling;
    public float FallSpeed;
}
