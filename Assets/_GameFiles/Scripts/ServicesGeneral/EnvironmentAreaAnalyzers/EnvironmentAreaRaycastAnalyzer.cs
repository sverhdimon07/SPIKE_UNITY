using UnityEngine;

public sealed class EnvironmentAreaRaycastAnalyzer<RequiredInterface, ExceptionType> : IEnvironmentAreaAnalyzer<RequiredInterface, ExceptionType> where RequiredInterface : class where ExceptionType : class
{
    public void Initialize(Vector3 actingEntityPosition, Vector2 actingEntityDirection, float toolDistanceToActingEntity, float toolLength, float toolHeight)
    {
        //
    }

    public RequiredInterface Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        return null;
        //
    }
}
