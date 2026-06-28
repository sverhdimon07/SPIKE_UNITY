using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    public Vector3 Position;

    public Quaternion Rotation;

    public float Health;
}

[System.Serializable]
public class SavedCharactersData
{
    public CharacterSaveData[] Characters;
}
