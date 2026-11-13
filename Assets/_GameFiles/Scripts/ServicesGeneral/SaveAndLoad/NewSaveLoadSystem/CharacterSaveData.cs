using UnityEngine;

[System.Serializable]
public class CharacterPositionData
{
    public string CharacterType;

    public Vector3 Position;

    public Quaternion Rotation;
}

[System.Serializable]
public class CharacterSaveData : CharacterPositionData
{
    public float Health;
}

[System.Serializable]
public class SavedCharactersData
{
    public CharacterSaveData[] Characters;
    public CharacterPositionData Boss;
}
