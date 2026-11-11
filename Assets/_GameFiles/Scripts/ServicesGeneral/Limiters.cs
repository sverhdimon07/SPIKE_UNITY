using UnityEngine;

public class Limiters : MonoBehaviour
{
    public void PlaceAround(Vector3 groundCenter, float requiredRadius)
    {
        BoxCollider[] walls = GetComponentsInChildren<BoxCollider>(true);
        if (walls.Length == 0)
            return;

        Vector3 center = GetWallCenter(walls);
        float innerRadius = float.PositiveInfinity;
        float highestWallBottom = float.NegativeInfinity;
        foreach (BoxCollider wall in walls)
        {
            Vector3 wallCenter = wall.transform.TransformPoint(wall.center);
            Vector3 halfDepth = wall.transform.TransformVector(Vector3.forward * (wall.size.z * 0.5f));
            float clearance = Mathf.Abs(Vector3.Dot(wallCenter - center, halfDepth.normalized)) - halfDepth.magnitude;
            innerRadius = Mathf.Min(innerRadius, clearance);

            float halfHeight = Mathf.Abs(wall.transform.TransformVector(Vector3.up * (wall.size.y * 0.5f)).y);
            highestWallBottom = Mathf.Max(highestWallBottom, wallCenter.y - halfHeight);
        }

        if (innerRadius > 0f && requiredRadius > innerRadius)
        {
            float expansion = requiredRadius / innerRadius;
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(expansion, 1f, expansion));
            center = GetWallCenter(walls);
        }

        center.y = highestWallBottom;
        transform.position += groundCenter - center;
    }

    private static Vector3 GetWallCenter(BoxCollider[] walls)
    {
        Vector3 center = Vector3.zero;
        foreach (BoxCollider wall in walls)
            center += wall.transform.TransformPoint(wall.center);

        return center / walls.Length;
    }
}
